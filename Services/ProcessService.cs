using System.Diagnostics;
using System_Resource_Monitor.Models;

namespace System_Resource_Monitor.Services;

public sealed class ProcessService : IProcessService
{
    // Keeps the last cumulative CPU time and the clock instant we observed each PID at, so the next Sample() can compute a delta.
    private readonly Dictionary<int, (TimeSpan Cpu, DateTime At)> _previous = new();
    private readonly int _coreCount = Environment.ProcessorCount;

    public IReadOnlyList<ProcessInfo> Sample()
    {
        var now = DateTime.UtcNow;
        var result = new List<ProcessInfo>(256);
        var seen = new HashSet<int>();

        foreach (var p in Process.GetProcesses())
        {
            try
            {
                TimeSpan cpu;
                long mem;
                string name;
                try
                {
                    cpu = p.TotalProcessorTime;
                    mem = p.WorkingSet64;
                    name = p.ProcessName;
                }
                catch
                {
                    // Access denied or the process exited between enumeration and the property read.
                    continue;
                }

                seen.Add(p.Id);

                double cpuPercent = 0;
                if (_previous.TryGetValue(p.Id, out var prev))
                {
                    var cpuDelta = (cpu - prev.Cpu).TotalMilliseconds;
                    var wallDelta = (now - prev.At).TotalMilliseconds;
                    if (wallDelta > 0)
                    {
                        // % of system-wide CPU = (CPU time used) / (elapsed * cores).
                        cpuPercent = cpuDelta / (wallDelta * _coreCount) * 100.0;
                    }
                }

                _previous[p.Id] = (cpu, now);

                result.Add(new ProcessInfo
                {
                    Pid = p.Id,
                    Name = name,
                    CpuPercent = Math.Clamp(cpuPercent, 0, 100),
                    MemoryMb = mem / 1024.0 / 1024.0
                });
            }
            finally
            {
                p.Dispose();
            }
        }

        // Drop bookkeeping for processes that have exited so the dictionary doesn't grow without bound.
        foreach (var pid in _previous.Keys.Where(k => !seen.Contains(k)).ToList())
        {
            _previous.Remove(pid);
        }

        return result;
    }
}
