using CommunityToolkit.Mvvm.ComponentModel;
using System_Resource_Monitor.Services;

namespace System_Resource_Monitor.ViewModels;

public sealed partial class ShellViewModel : ObservableObject, IDisposable
{
    private readonly ICpuService _cpu;
    private readonly IMemoryService _memory;
    private readonly IDiskService _disk;

    private readonly CancellationTokenSource _cts = new();

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

        // This loop will own its own lifecycle via the cancellation token.
        // ShellViewModel is created on the UI thread
        _ = RunAsync(_cts.Token);
    }

    private async Task RunAsync(CancellationToken ct)
    {
        var timer = new PeriodicTimer(TimeSpan.FromSeconds(1));
        try
        {
            while (await timer.WaitForNextTickAsync(ct))
            {
                // PerformanceCounter.NextValue() was causing brief blocks,
                // so pushing the actual samples onto the thread pool prevents this.
                var (cpuSample, memSample, diskSample) = await Task.Run(() => (_cpu.Sample(), _memory.Sample(), _disk.Sample()), ct);
                
                Cpu.ApplySample(cpuSample);
                Memory.ApplySample(memSample);
                Disk.ApplySample(diskSample);
            }
        }
        catch (OperationCanceledException)
        {
            // Expected on shutdown.
        }
    }

    public void Dispose()
    {
        _cts.Cancel();
        _cts.Dispose();

        _cpu.Dispose();
        _memory.Dispose();
        _disk.Dispose();
    }
}
