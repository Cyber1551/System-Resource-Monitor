using CommunityToolkit.Mvvm.ComponentModel;
using System_Resource_Monitor.Services;

namespace System_Resource_Monitor.ViewModels;

public sealed partial class ShellViewModel : ObservableObject, IDisposable
{
    private readonly ICpuService _cpu;
    private readonly IMemoryService _memory;
    private readonly IDiskService _disk;

    private readonly CancellationTokenSource _cts = new();
    private readonly DateTime _started = DateTime.UtcNow;

    public CpuViewModel Cpu { get; }
    public MemoryViewModel Memory { get; }
    public DiskViewModel Disk { get; }
    public HistoryChartViewModel History { get; }
    public ProcessListViewModel Processes { get; }

    [ObservableProperty]
    private string _statusText = "SAMPLING...";

    public ShellViewModel(
        ICpuService cpu,
        IMemoryService memory,
        IDiskService disk,
        CpuViewModel cpuVm,
        MemoryViewModel memoryVm,
        DiskViewModel diskVm,
        HistoryChartViewModel historyVm,
        ProcessListViewModel processesVm)
    {
        _cpu = cpu;
        _memory = memory;
        _disk = disk;

        Cpu = cpuVm;
        Memory = memoryVm;
        Disk = diskVm;
        History = historyVm;
        Processes = processesVm;

        // This loop will own its own lifecycle via the cancellation token.
        // ShellViewModel is created on the UI thread
        _ = RunAsync(_cts.Token);
    }

    private async Task RunAsync(CancellationToken ct)
    {
        var timer = new PeriodicTimer(TimeSpan.FromSeconds(1));
        var tick = 0;
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
                History.Push(cpuSample.TotalPercent, memSample.UsedPercent);

                // Process enumeration is the most expensive sampling step, so refresh it every other tick (~2 seconds) instead of every tick.
                // Sampling runs on the thread pool; the snapshot is then applied on the UI thread because ApplySnapshot mutates the bound ObservableCollection.
                if (tick++ % 2 == 0)
                {
                    var snapshot = await Task.Run(Processes.Sample, ct);
                    Processes.ApplySnapshot(snapshot);
                }

                var uptime = DateTime.UtcNow - _started;
                StatusText = $"HOST {Environment.MachineName.ToUpperInvariant()}" +
                             $@"    UPTIME {uptime:hh\:mm\:ss}" +
                             $"    LAST SAMPLE {DateTime.Now:HH:mm:ss}";
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
