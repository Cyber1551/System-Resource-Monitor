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

    private readonly ObservableCollection<double> _cpuValues = [];
    private readonly ObservableCollection<double> _ramValues = [];

    public ISeries[] Series { get; }
    public Axis[] XAxes { get; }
    public Axis[] YAxes { get; }

    public HistoryChartViewModel()
    {
        // Match Themes/DarkTheme.xaml: muted text, hairline border, amber accent, and a desaturated teal as the secondary line
        var muted = new SKColor(0x5E, 0x65, 0x73);
        // Separator color sits just barely above the background
        var separator = new SKColor(0x16, 0x19, 0x1F);
        var accent = new SKColor(0xFF, 0xB0, 0x00);
        var accentAlt = new SKColor(0x5A, 0x9F, 0xB5);

        XAxes =
        [
            new Axis
            {
                IsVisible = false,
                SeparatorsPaint = null
            }
        ];

        YAxes =
        [
            new Axis
            {
                MinLimit = 0,
                MaxLimit = 100,
                // Step the grid every 25% (0/25/50/75/100) instead of the auto-derived 20% spacing so there are fewer lines.
                MinStep = 25,
                ForceStepToMin = true,
                Labeler = v => $"{v:F0}%",
                LabelsPaint = new SolidColorPaint(muted) { SKTypeface = SKTypeface.FromFamilyName("Cascadia Mono") },
                TextSize = 10,
                SeparatorsPaint = new SolidColorPaint(separator) { StrokeThickness = 1 }
            }
        ];

        Series =
        [
            new LineSeries<double>
            {
                Name = "CPU",
                Values = _cpuValues,
                GeometrySize = 0,
                Stroke = new SolidColorPaint(accent) { StrokeThickness = 1.5f },
                Fill = new SolidColorPaint(accent.WithAlpha(20))
            },
            new LineSeries<double>
            {
                Name = "MEM",
                Values = _ramValues,
                GeometrySize = 0,
                Stroke = new SolidColorPaint(accentAlt) { StrokeThickness = 1.5f },
                Fill = new SolidColorPaint(accentAlt.WithAlpha(20))
            }
        ];
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
