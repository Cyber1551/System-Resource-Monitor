using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using System_Resource_Monitor.Services;

namespace System_Resource_Monitor.ViewModels;

public sealed partial class ShellViewModel : ObservableObject
{
    private readonly ICpuService _cpu;
    private readonly DispatcherTimer _timer;

    public CpuViewModel Cpu { get; }

    public ShellViewModel(ICpuService cpu, CpuViewModel cpuVm)
    {
        _cpu = cpu;
        Cpu = cpuVm;

        _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
        _timer.Tick += (_, _) => Cpu.ApplySample(_cpu.Sample());
        _timer.Start();
    }
}
