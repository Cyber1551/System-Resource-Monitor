using CommunityToolkit.Mvvm.ComponentModel;
using System_Resource_Monitor.Models;

namespace System_Resource_Monitor.ViewModels;

public sealed partial class MemoryViewModel : ObservableObject
{
    [ObservableProperty]
    private double _totalGb;

    [ObservableProperty]
    private double _usedGb;

    [ObservableProperty]
    private double _availableGb;

    [ObservableProperty]
    private double _usedPercent;

    public void ApplySample(MemorySample sample)
    {
        TotalGb = sample.TotalGb;
        UsedGb = sample.UsedGb;
        AvailableGb = sample.AvailableGb;
        UsedPercent = sample.UsedPercent;
    }
}
