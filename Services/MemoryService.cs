using System.Diagnostics;
using System.Management;
using System_Resource_Monitor.Models;

namespace System_Resource_Monitor.Services;

public sealed class MemoryService : IMemoryService
{
    private readonly PerformanceCounter _availableMb = new("Memory", "Available MBytes", readOnly: true);

    private readonly double _totalGb;

    public MemoryService()
    {
        _totalGb = QueryTotalPhysicalGb();

        // Prime the counter so the first real Sample() returns a meaningful value.
        _availableMb.NextValue();
    }

    public MemorySample Sample()
    {
        var availableGb = _availableMb.NextValue() / 1024.0;
        var usedGb = Math.Max(0, _totalGb - availableGb);
        return new MemorySample(_totalGb, usedGb, availableGb);
    }

    // Total physical memory doesn't change at runtime, so we can resolve it once via WMI rather than per-tick.
    private static double QueryTotalPhysicalGb()
    {
        using var searcher = new ManagementObjectSearcher("SELECT TotalVisibleMemorySize FROM Win32_OperatingSystem");

        foreach (var mo in searcher.Get())
        {
            // TotalVisibleMemorySize is reported in KB.
            var kb = Convert.ToDouble(mo["TotalVisibleMemorySize"]);
            return kb / 1024.0 / 1024.0;
        }

        return 0.0;
    }

    public void Dispose() => _availableMb.Dispose();
}
