using System.Diagnostics;
using System_Resource_Monitor.Models;

namespace System_Resource_Monitor.Services;

public sealed class CpuService : ICpuService
{
    private readonly PerformanceCounter _total;
    private readonly PerformanceCounter[] _cores;

    public CpuService()
    {
        _total = new PerformanceCounter("Processor", "% Processor Time", "_Total", readOnly: true);

        var coreCount = Environment.ProcessorCount;
        _cores = new PerformanceCounter[coreCount];
        for (var i = 0; i < coreCount; i++)
        {
            _cores[i] = new PerformanceCounter("Processor", "% Processor Time", i.ToString(), readOnly: true);
        }

        // The first NextValue() of a rate counter always returns 0 because it needs two samples to compute a delta.
        _total.NextValue();
        foreach (var c in _cores) 
        {
            c.NextValue();
        }
    }

    public CpuSample Sample()
    {
        var total = _total.NextValue();
        var per = new double[_cores.Length];
        for (var i = 0; i < _cores.Length; i++) 
        {
            per[i] = _cores[i].NextValue();
        }
        
        return new CpuSample(total, per);
    }

    public void Dispose()
    {
        _total.Dispose();
        foreach (var c in _cores)
        {
            c.Dispose();
        }
    }
}
