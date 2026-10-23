using ShellX.OptionsPricing;
using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;

namespace ShellX.Controls;

/// <summary>A 3D surface chart (see <see cref="SurfaceRenderer"/>). Drag to rotate.</summary>
public class Surface3DView : ContentView
{
    public static readonly BindableProperty SurfaceProperty = Property<HeatMapData?>(nameof(Surface), null);
    public static readonly BindableProperty ZLabelProperty = Property(nameof(ZLabel), string.Empty);
    public static readonly BindableProperty XLabelProperty = Property(nameof(XLabel), "t");
    public static readonly BindableProperty YLabelProperty = Property(nameof(YLabel), "Spot");
    public static readonly BindableProperty AzimuthProperty = Property(nameof(Azimuth), -50d);
    public static readonly BindableProperty ElevationProperty = Property(nameof(Elevation), 40d);

    private readonly SKCanvasView _canvas = new() { EnableTouchEvents = true };
    private SKPoint _lastTouch;
    private bool _dragging;

    public Surface3DView()
    {
        _canvas.PaintSurface += OnPaintSurface;
        _canvas.Touch += OnTouch;
        Content = _canvas;
    }

    public HeatMapData? Surface { get => (HeatMapData?)GetValue(SurfaceProperty); set => SetValue(SurfaceProperty, value); }
    public string ZLabel { get => (string)GetValue(ZLabelProperty); set => SetValue(ZLabelProperty, value); }
    public string XLabel { get => (string)GetValue(XLabelProperty); set => SetValue(XLabelProperty, value); }
    public string YLabel { get => (string)GetValue(YLabelProperty); set => SetValue(YLabelProperty, value); }
    public double Azimuth { get => (double)GetValue(AzimuthProperty); set => SetValue(AzimuthProperty, value); }
    public double Elevation { get => (double)GetValue(ElevationProperty); set => SetValue(ElevationProperty, value); }

    private static BindableProperty Property<T>(string name, T defaultValue) => BindableProperty.Create(
        name, typeof(T), typeof(Surface3DView), defaultValue,
        propertyChanged: (bindable, _, _) => ((Surface3DView)bindable)._canvas.InvalidateSurface());

    private void OnTouch(object? sender, SKTouchEventArgs e)
    {
        switch (e.ActionType)
        {
            case SKTouchAction.Pressed:
                _dragging = true;
                _lastTouch = e.Location;
                break;
            case SKTouchAction.Moved when _dragging:
                Azimuth += (e.Location.X - _lastTouch.X) * 0.5;
                Elevation = Math.Clamp(Elevation + (e.Location.Y - _lastTouch.Y) * 0.5, 0, 90);
                _lastTouch = e.Location;
                break;
            case SKTouchAction.Released:
            case SKTouchAction.Cancelled:
                _dragging = false;
                break;
        }
        e.Handled = true;
    }

    private void OnPaintSurface(object? sender, SKPaintSurfaceEventArgs e)
    {
        float density = Width > 0 ? e.Info.Width / (float)Width : 1;
        var textColor = Application.Current?.RequestedTheme == AppTheme.Dark ? SKColors.White : SKColors.Black;

        SurfaceRenderer.Draw(e.Surface.Canvas, e.Info.Width, e.Info.Height, density,
            new SurfaceRenderer.Options(Surface, Azimuth, Elevation, XLabel, YLabel, ZLabel, textColor));
    }
}
