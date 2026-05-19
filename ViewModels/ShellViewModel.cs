using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using System_Resource_Monitor.Services;

namespace System_Resource_Monitor.ViewModels;

public sealed partial class ShellViewModel : ObservableObject
{
    private readonly ICpuService _cpu;
    private readonly IMemoryService _memory;
    private readonly IDiskService _disk;
    private readonly DispatcherTimer _timer;

    public CpuViewModel Cpu { get; }
    public MemoryViewModel Memory { get; }
    public DiskViewModel Disk { get; }

    public ShellViewModel(
        ICpuService cpu,
        IMemoryService memory,
        IDiskService disk,
        CpuViewModel cpuVm,
        MemoryViewModel memoryVm,
        DiskViewModel diskVm)
    {
        _cpu = cpu;
        _memory = memory;
        _disk = disk;
        Cpu = cpuVm;
        Memory = memoryVm;
        Disk = diskVm;

        _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
        _timer.Tick += (_, _) =>
        {
            Cpu.ApplySample(_cpu.Sample());
            Memory.ApplySample(_memory.Sample());
            Disk.ApplySample(_disk.Sample());
        };
        _timer.Start();
    }
}
