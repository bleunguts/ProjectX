using System.Diagnostics;
using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;

namespace ShellX.Controls;

/// <summary>The animated shader backdrop (see <see cref="ShaderBackdropRenderer"/>); it only runs while the view is on screen.</summary>
public class ShaderBackdropView : ContentView
{
    private static readonly TimeSpan FrameInterval = TimeSpan.FromMilliseconds(33);

    private readonly SKCanvasView _canvas = new();
    private readonly Stopwatch _clock = Stopwatch.StartNew();
    private ShaderBackdropRenderer? _renderer;
    private IDispatcherTimer? _timer;

    public ShaderBackdropView()
    {
        _canvas.PaintSurface += OnPaintSurface;
        Content = _canvas;
        InputTransparent = true;
        Loaded += (_, _) => Start();
        Unloaded += (_, _) => Stop();
    }

    private void Start()
    {
        _timer ??= Dispatcher.CreateTimer();
        _timer.Interval = FrameInterval;
        _timer.Tick -= OnTick;
        _timer.Tick += OnTick;
        if (!_timer.IsRunning)
            _timer.Start();
    }

    private void Stop()
    {
        _timer?.Stop();
        if (_timer is not null)
            _timer.Tick -= OnTick;
    }

    private void OnTick(object? sender, EventArgs e) => _canvas.InvalidateSurface();

    private void OnPaintSurface(object? sender, SKPaintSurfaceEventArgs e)
    {
        _renderer ??= new ShaderBackdropRenderer();
        _renderer.Draw(e.Surface.Canvas, e.Info.Width, e.Info.Height, _clock.Elapsed.TotalSeconds);
    }
}
