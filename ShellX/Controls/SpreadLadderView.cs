using ShellX.Fx;
using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;

namespace ShellX.Controls;

/// <summary>The spread ladder (see <see cref="SpreadLadderRenderer"/>) as a MAUI view.</summary>
public class SpreadLadderView : ContentView
{
    public static readonly BindableProperty BidProperty = Property(nameof(Bid), 0d);
    public static readonly BindableProperty AskProperty = Property(nameof(Ask), 0d);
    public static readonly BindableProperty HasPriceProperty = Property(nameof(HasPrice), false);
    public static readonly BindableProperty TrailProperty = Property<IReadOnlyList<LadderTick>>(nameof(Trail), []);

    private readonly SKCanvasView _canvas = new();

    public SpreadLadderView()
    {
        _canvas.PaintSurface += OnPaintSurface;
        Content = _canvas;
    }

    public double Bid { get => (double)GetValue(BidProperty); set => SetValue(BidProperty, value); }
    public double Ask { get => (double)GetValue(AskProperty); set => SetValue(AskProperty, value); }
    public bool HasPrice { get => (bool)GetValue(HasPriceProperty); set => SetValue(HasPriceProperty, value); }
    public IReadOnlyList<LadderTick> Trail { get => (IReadOnlyList<LadderTick>)GetValue(TrailProperty); set => SetValue(TrailProperty, value); }

    private static BindableProperty Property<T>(string name, T defaultValue) => BindableProperty.Create(
        name, typeof(T), typeof(SpreadLadderView), defaultValue,
        propertyChanged: (bindable, _, _) => ((SpreadLadderView)bindable)._canvas.InvalidateSurface());

    private void OnPaintSurface(object? sender, SKPaintSurfaceEventArgs e)
    {
        float density = Width > 0 ? e.Info.Width / (float)Width : 1;
        var dark = Application.Current?.RequestedTheme == AppTheme.Dark;
        var textColor = dark ? SKColors.White : SKColors.Black;
        var background = dark ? SKColor.Parse("#1C2128") : SKColor.Parse("#F4F6F8");

        SpreadLadderRenderer.Draw(e.Surface.Canvas, e.Info.Width, e.Info.Height, density,
            new SpreadLadderRenderer.Options(Bid, Ask, Trail, HasPrice, textColor, background));
    }
}
