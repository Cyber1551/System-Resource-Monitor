using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using System_Resource_Monitor.Models;

namespace System_Resource_Monitor.ViewModels;

public sealed partial class CpuViewModel : ObservableObject
{
    [ObservableProperty]
    private double _totalPercent;

    public ObservableCollection<CoreUsage> Cores { get; } = [];

    public CpuViewModel()
    {
        for (var i = 0; i < Environment.ProcessorCount; i++)
        {
            Cores.Add(new CoreUsage { Index = i });  
        }
    }

    public void ApplySample(CpuSample sample)
    {
        TotalPercent = sample.TotalPercent;

        for (var i = 0; i < sample.PerCorePercent.Count && i < Cores.Count; i++)
        {
            Cores[i].Percent = sample.PerCorePercent[i];
        }
    }
}

public sealed partial class CoreUsage : ObservableObject
{
    [ObservableProperty]
    private int _index;

    // IsHigh is recomputed every time Percent changes so the per-core bar can swap its brush to WarnBrush via a XAML DataTrigger without a converter.
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsHigh))]
    private double _percent;

    public bool IsHigh => Percent > 80;
}
