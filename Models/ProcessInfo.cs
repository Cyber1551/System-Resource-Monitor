using CommunityToolkit.Mvvm.ComponentModel;

namespace System_Resource_Monitor.Models;

public sealed partial class ProcessInfo : ObservableObject
{
    [ObservableProperty]
    private int _pid;

    [ObservableProperty]
    private string _name = "";

    [ObservableProperty]
    private double _cpuPercent;

    [ObservableProperty]
    private double _memoryMb;
}
