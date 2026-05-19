using CommunityToolkit.Mvvm.ComponentModel;
using System_Resource_Monitor.Models;

namespace System_Resource_Monitor.ViewModels;

public sealed partial class DiskViewModel : ObservableObject
{
    [ObservableProperty]
    private double _readMBs;

    [ObservableProperty]
    private double _writeMBs;

    public void ApplySample(DiskSample sample)
    {
        ReadMBs = sample.ReadBytesPerSec / 1024.0 / 1024.0;
        WriteMBs = sample.WriteBytesPerSec / 1024.0 / 1024.0;
    }
}
