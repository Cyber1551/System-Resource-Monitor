namespace System_Resource_Monitor.Models;

public sealed record CpuSample(double TotalPercent, IReadOnlyList<double> PerCorePercent);
