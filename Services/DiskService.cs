using System.Diagnostics;
using System_Resource_Monitor.Models;

namespace System_Resource_Monitor.Services;

public sealed class DiskService : IDiskService
{
    private readonly PerformanceCounter _read = new("PhysicalDisk", "Disk Read Bytes/sec", "_Total", readOnly: true);

    private readonly PerformanceCounter _write = new("PhysicalDisk", "Disk Write Bytes/sec", "_Total", readOnly: true);

    public DiskService()
    {
        // Rate counters return 0 on the first call; prime them so the first real Sample() reports the actual interval throughput.
        _read.NextValue();
        _write.NextValue();
    }

    public DiskSample Sample() => new(_read.NextValue(), _write.NextValue());

    public void Dispose()
    {
        _read.Dispose();
        _write.Dispose();
    }
}
