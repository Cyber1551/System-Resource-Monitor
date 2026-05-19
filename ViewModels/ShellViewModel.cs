using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using System_Resource_Monitor.Services;

namespace System_Resource_Monitor.ViewModels;

public sealed partial class ShellViewModel : ObservableObject
{
    private readonly ICpuService _cpu;
    private readonly DispatcherTimer _timer;

    [ObservableProperty]
    private string _statusText = "Sampling...";

    public ShellViewModel(ICpuService cpu)
    {
        _cpu = cpu;

        _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
        _timer.Tick += (_, _) => UpdateStatus();
        _timer.Start();
    }

    private void UpdateStatus()
    {
        var sample = _cpu.Sample();
        var cores = string.Join(", ", sample.PerCorePercent.Select(p => $"{p:F0}%"));
        StatusText = $"CPU total: {sample.TotalPercent:F1}%\nCores: {cores}";
    }
}
