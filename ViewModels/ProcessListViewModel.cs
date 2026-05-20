using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using System_Resource_Monitor.Models;
using System_Resource_Monitor.Services;

namespace System_Resource_Monitor.ViewModels;

public sealed partial class ProcessListViewModel : ObservableObject
{
    private readonly IProcessService _processes;

    // Keyed lookup so ApplySnapshot can find the existing row for a PID in O(1) instead of rebuilding the collection.
    private readonly Dictionary<int, ProcessInfo> _byPid = new();

    public ObservableCollection<ProcessInfo> Items { get; } = [];

    // Bound by the DataGrid so column-header clicks can re-sort without mutating Items directly.
    public ICollectionView ItemsView { get; }

    public ProcessListViewModel(IProcessService processes)
    {
        _processes = processes;

        ItemsView = CollectionViewSource.GetDefaultView(Items);
        ItemsView.SortDescriptions.Add(
            new SortDescription(nameof(ProcessInfo.CpuPercent), ListSortDirection.Descending));
    }

    // Read against the underlying service, so this is safe to call from any thread.
    public IReadOnlyList<ProcessInfo> Sample() => _processes.Sample();

    // Mutates Items, which is bound to the DataGrid via ItemsView. Must run on the UI thread or ObservableCollection raises CollectionChanged from the wrong thread and the DataGrid throws.
    public void ApplySnapshot(IReadOnlyList<ProcessInfo> snapshot)
    {
        // Update existing PIDs in place; add rows for new PIDs.
        foreach (var s in snapshot)
        {
            if (_byPid.TryGetValue(s.Pid, out var existing))
            {
                existing.CpuPercent = s.CpuPercent;
                existing.MemoryMb = s.MemoryMb;
            }
            else
            {
                _byPid[s.Pid] = s;
                Items.Add(s);
            }
        }

        // Drop rows whose PID is no longer in the snapshot.
        var alive = snapshot.Select(p => p.Pid).ToHashSet();
        for (var i = Items.Count - 1; i >= 0; i--)
        {
            if (!alive.Contains(Items[i].Pid))
            {
                _byPid.Remove(Items[i].Pid);
                Items.RemoveAt(i);
            }
        }

        ItemsView.Refresh();
    }
}
