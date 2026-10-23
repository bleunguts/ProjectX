using System.Diagnostics;
using ShellX.OptionsPricing;
using ShellX.Surface3D;
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
    public static readonly BindableProperty AutoRotateProperty = BindableProperty.Create(
        nameof(AutoRotate), typeof(bool), typeof(Surface3DView), true,
        propertyChanged: (bindable, _, _) => ((Surface3DView)bindable).UpdateTimer());

    private static readonly TimeSpan FrameInterval = TimeSpan.FromMilliseconds(33);
    private static readonly TimeSpan ResumeDelay = TimeSpan.FromSeconds(1.5);

    private readonly SKCanvasView _canvas = new() { EnableTouchEvents = true };
    private SKPoint _lastTouch;
    private bool _dragging;
    private IDispatcherTimer? _timer;
    private readonly Stopwatch _clock = Stopwatch.StartNew();
    private TimeSpan _lastFrame;
    private TimeSpan _lastInteraction = TimeSpan.FromDays(-1);

    public Surface3DView()
    {
        _canvas.PaintSurface += OnPaintSurface;
        _canvas.Touch += OnTouch;
        Content = _canvas;
        Loaded += (_, _) => UpdateTimer();
        Unloaded += (_, _) => StopTimer();
    }

    public HeatMapData? Surface { get => (HeatMapData?)GetValue(SurfaceProperty); set => SetValue(SurfaceProperty, value); }
    public string ZLabel { get => (string)GetValue(ZLabelProperty); set => SetValue(ZLabelProperty, value); }
    public string XLabel { get => (string)GetValue(XLabelProperty); set => SetValue(XLabelProperty, value); }
    public string YLabel { get => (string)GetValue(YLabelProperty); set => SetValue(YLabelProperty, value); }
    public double Azimuth { get => (double)GetValue(AzimuthProperty); set => SetValue(AzimuthProperty, value); }
    public double Elevation { get => (double)GetValue(ElevationProperty); set => SetValue(ElevationProperty, value); }

    /// <summary>Slowly turns the surface (about 20 degrees a second) until the user drags it, resuming a moment after release.</summary>
    public bool AutoRotate { get => (bool)GetValue(AutoRotateProperty); set => SetValue(AutoRotateProperty, value); }

    private static BindableProperty Property<T>(string name, T defaultValue) => BindableProperty.Create(
        name, typeof(T), typeof(Surface3DView), defaultValue,
        propertyChanged: (bindable, _, _) => ((Surface3DView)bindable)._canvas.InvalidateSurface());

    private void OnTouch(object? sender, SKTouchEventArgs e)
    {
        switch (e.ActionType)
        {
            case SKTouchAction.Pressed:
                _dragging = true;
                _lastInteraction = _clock.Elapsed;
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
                _lastInteraction = _clock.Elapsed;
                break;
        }
        e.Handled = true;
    }

    private void UpdateTimer()
    {
        if (!AutoRotate || !IsLoaded)
        {
            StopTimer();
            return;
        }

        if (_timer is { IsRunning: true })
            return;

        _timer ??= Dispatcher.CreateTimer();
        _timer.Interval = FrameInterval;
        _timer.Tick -= OnTick;
        _timer.Tick += OnTick;
        _lastFrame = _clock.Elapsed;
        _timer.Start();
    }

    private void StopTimer()
    {
        if (_timer is null)
            return;

        _timer.Stop();
        _timer.Tick -= OnTick;
    }

    private void OnTick(object? sender, EventArgs e)
    {
        var now = _clock.Elapsed;
        var elapsed = (now - _lastFrame).TotalSeconds;
        _lastFrame = now;

        // Nothing to turn until a surface is plotted; hold still while dragging and for a moment after.
        if (Surface is null || _dragging || now - _lastInteraction < ResumeDelay)
            return;

        Azimuth = AutoRotation.Step(Azimuth, elapsed);
    }

    private void OnPaintSurface(object? sender, SKPaintSurfaceEventArgs e)
    {
        float density = Width > 0 ? e.Info.Width / (float)Width : 1;
        var textColor = Application.Current?.RequestedTheme == AppTheme.Dark ? SKColors.White : SKColors.Black;

        SurfaceRenderer.Draw(e.Surface.Canvas, e.Info.Width, e.Info.Height, density,
            new SurfaceRenderer.Options(Surface, Azimuth, Elevation, XLabel, YLabel, ZLabel, textColor));
    }
}
