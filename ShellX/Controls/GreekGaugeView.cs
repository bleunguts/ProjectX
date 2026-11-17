using ShellX.Fx;
using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;

namespace ShellX.Controls;

/// <summary>A Greek dial (see <see cref="GreekGaugeRenderer"/>) whose arc sweeps to each new value.</summary>
public class GreekGaugeView : ContentView
{
    private const string AnimationName = "sweep";
    private const uint SweepMs = 800;

    public static readonly BindableProperty FractionProperty = BindableProperty.Create(
        nameof(Fraction), typeof(double), typeof(GreekGaugeView), 0.5,
        propertyChanged: (bindable, oldValue, newValue) => ((GreekGaugeView)bindable).SweepTo((double)newValue));

    public static readonly BindableProperty ValueTextProperty = Property(nameof(ValueText), "0.00");
    public static readonly BindableProperty LabelProperty = Property(nameof(Label), string.Empty);

    private readonly SKCanvasView _canvas = new();
    private double _shown;

    public GreekGaugeView()
    {
        _canvas.PaintSurface += OnPaintSurface;
        Content = _canvas;
        InputTransparent = true;
    }

    public double Fraction { get => (double)GetValue(FractionProperty); set => SetValue(FractionProperty, value); }
    public string ValueText { get => (string)GetValue(ValueTextProperty); set => SetValue(ValueTextProperty, value); }
    public string Label { get => (string)GetValue(LabelProperty); set => SetValue(LabelProperty, value); }

    private static BindableProperty Property<T>(string name, T defaultValue) => BindableProperty.Create(
        name, typeof(T), typeof(GreekGaugeView), defaultValue,
        propertyChanged: (bindable, _, _) => ((GreekGaugeView)bindable)._canvas.InvalidateSurface());

    private void SweepTo(double target)
    {
        var from = _shown;
        this.AbortAnimation(AnimationName);
        new Animation(t =>
        {
            _shown = CountUp.Value(from, target, t);
            _canvas.InvalidateSurface();
        }, 0, 1).Commit(this, AnimationName, length: SweepMs, easing: Easing.Linear, finished: (_, _) =>
        {
            _shown = target;
            _canvas.InvalidateSurface();
        });
    }

    private void OnPaintSurface(object? sender, SKPaintSurfaceEventArgs e)
    {
        float density = Width > 0 ? e.Info.Width / (float)Width : 1;
        GreekGaugeRenderer.Draw(e.Surface.Canvas, e.Info.Width, e.Info.Height, density,
            new GreekGaugeRenderer.Options(_shown, ValueText, Label, SKColors.White));
    }
}
