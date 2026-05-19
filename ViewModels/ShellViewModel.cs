using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using System_Resource_Monitor.Services;

namespace System_Resource_Monitor.ViewModels;

public sealed partial class ShellViewModel : ObservableObject
{
    private readonly ICpuService _cpu;
    private readonly IMemoryService _memory;
    private readonly DispatcherTimer _timer;

    public CpuViewModel Cpu { get; }
    public MemoryViewModel Memory { get; }

    public ShellViewModel(
        ICpuService cpu,
        IMemoryService memory,
        CpuViewModel cpuVm,
        MemoryViewModel memoryVm)
    {
        _cpu = cpu;
        _memory = memory;
        Cpu = cpuVm;
        Memory = memoryVm;

        _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
        _timer.Tick += (_, _) =>
        {
            Cpu.ApplySample(_cpu.Sample());
            Memory.ApplySample(_memory.Sample());
        };
        _timer.Start();
    }
}
