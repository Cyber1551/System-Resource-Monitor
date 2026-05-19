namespace System_Resource_Monitor.Models;

public sealed record MemorySample(double TotalGb, double UsedGb, double AvailableGb)
{
    public double UsedPercent => TotalGb > 0 
        ? UsedGb / TotalGb * 100.0 
        : 0.0;
}
