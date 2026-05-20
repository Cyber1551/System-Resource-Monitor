using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace System_Resource_Monitor.ViewModels;

public sealed partial class HistoryChartViewModel : ObservableObject
{
    private const int MaxPoints = 60;

    private readonly ObservableCollection<double> _cpuValues = new();
    private readonly ObservableCollection<double> _ramValues = new();

    public ISeries[] Series { get; }
    public Axis[] XAxes { get; } = { new Axis { IsVisible = false } };
    public Axis[] YAxes { get; }

    public HistoryChartViewModel()
    {
        var muted = new SKColor(0x8A, 0x93, 0xA6);
        var border = new SKColor(0x2A, 0x2F, 0x3A);
        var accent = new SKColor(0x4F, 0x8C, 0xFF);
        var accentAlt = new SKColor(0x7C, 0x5C, 0xFF);

        YAxes = new[]
        {
            new Axis
            {
                MinLimit = 0,
                MaxLimit = 100,
                Labeler = v => $"{v:F0}%",
                LabelsPaint = new SolidColorPaint(muted),
                SeparatorsPaint = new SolidColorPaint(border) { StrokeThickness = 1 }
            }
        };

        Series = new ISeries[]
        {
            new LineSeries<double>
            {
                Name = "CPU",
                Values = _cpuValues,
                GeometrySize = 0,
                Stroke = new SolidColorPaint(accent) { StrokeThickness = 2 },
                Fill = new SolidColorPaint(accent.WithAlpha(40))
            },
            new LineSeries<double>
            {
                Name = "RAM",
                Values = _ramValues,
                GeometrySize = 0,
                Stroke = new SolidColorPaint(accentAlt) { StrokeThickness = 2 },
                Fill = new SolidColorPaint(accentAlt.WithAlpha(40))
            }
        };
    }

    public void Push(double cpuPercent, double ramPercent)
    {
        Append(_cpuValues, cpuPercent);
        Append(_ramValues, ramPercent);
    }

    private static void Append(ObservableCollection<double> buffer, double value)
    {
        buffer.Add(value);
        while (buffer.Count > MaxPoints) buffer.RemoveAt(0);
    }
}
