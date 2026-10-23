using ShellX.OptionsPricing;
using ShellX.Surface3D;
using SkiaSharp;

namespace ShellX.Controls;

/// <summary>Everything <see cref="Surface3DView"/> draws, with no MAUI dependency so it can render headlessly.</summary>
public static class SurfaceRenderer
{
    private const int GridDivisions = 5;
    private const float LabelFontDips = 11;
    private const float MarginDips = 46;

    public record Options(HeatMapData? Surface, double Azimuth, double Elevation, string XLabel, string YLabel, string ZLabel, SKColor TextColor);

    /// <param name="density">Pixels per device-independent unit, so sizes look the same on any display scaling.</param>
    public static void Draw(SKCanvas canvas, int width, int height, float density, Options options)
    {
        canvas.Clear(SKColors.Transparent);

        using var font = new SKFont { Size = LabelFontDips * density };
        using var textPaint = new SKPaint { Color = options.TextColor, IsAntialias = true };

        if (options.Surface is not { } data || data.XCount < 2 || data.YCount < 2)
        {
            canvas.DrawText("Plot price or a Greek to see the surface", width / 2f, height / 2f, SKTextAlign.Center, font, textPaint);
            return;
        }

        var projection = new SurfaceProjection(options.Azimuth, options.Elevation);
        var view = ViewTransform.Fit(projection, width, height, MarginDips * density);
        SKPoint Screen(double x, double y, double z) => view.ToScreen(projection.Project(x, y, z));

        DrawFloorGrid(canvas, Screen, density);

        using var fill = new SKPaint { IsAntialias = true, Style = SKPaintStyle.Fill };
        using var wire = new SKPaint { IsAntialias = true, Style = SKPaintStyle.Stroke, Color = new SKColor(0, 0, 0, 90), StrokeWidth = 0.5f * density };
        using var path = new SKPath();
        foreach (var quad in SurfaceMesh.Build(data, projection))
        {
            path.Rewind();
            for (int i = 0; i < quad.Corners.Length; i++)
            {
                var p = view.ToScreen(quad.Corners[i]);
                if (i == 0) path.MoveTo(p); else path.LineTo(p);
            }
            path.Close();
            fill.Color = new SKColor(quad.Color.R, quad.Color.G, quad.Color.B);
            canvas.DrawPath(path, fill);
            canvas.DrawPath(path, wire);
        }

        DrawAxes(canvas, data, projection, Screen, options, font, textPaint, density);
    }

    private static void DrawFloorGrid(SKCanvas canvas, Func<double, double, double, SKPoint> screen, float density)
    {
        const double floor = -SurfaceMesh.HalfHeight;
        using var paint = new SKPaint
        {
            IsAntialias = true,
            Style = SKPaintStyle.Stroke,
            Color = new SKColor(60, 160, 60, 150),
            StrokeWidth = density,
            PathEffect = SKPathEffect.CreateDash([2 * density, 3 * density], 0),
        };

        for (int i = 0; i <= GridDivisions; i++)
        {
            double v = -1 + 2.0 * i / GridDivisions;
            canvas.DrawLine(screen(v, -1, floor), screen(v, 1, floor), paint);
            canvas.DrawLine(screen(-1, v, floor), screen(1, v, floor), paint);
        }
    }

    private static void DrawAxes(SKCanvas canvas, HeatMapData data, SurfaceProjection projection, Func<double, double, double, SKPoint> screen,
        Options options, SKFont font, SKPaint textPaint, float density)
    {
        const double floor = -SurfaceMesh.HalfHeight, top = SurfaceMesh.HalfHeight;
        // The floor corner nearest the viewer: the t and Spot axes run along the two floor edges meeting there.
        var corners = new[] { (-1d, -1d), (-1d, 1d), (1d, -1d), (1d, 1d) };
        var (nearX, nearY) = corners.MinBy(c => projection.Project(c.Item1, c.Item2, floor).Depth);

        void Text(string text, SKPoint at, SKTextAlign align, float dxDips = 0, float dyDips = 0) =>
            canvas.DrawText(text, at.X + dxDips * density, at.Y + (dyDips * density) + font.Size / 3, align, font, textPaint);

        // Each axis is labelled just outside its floor edge, pushed away along the other axis so the two
        // sets of labels do not collide at the shared near corner.
        const double tick = 1.14, title = 1.4;
        Text(Format(data.XValue(0)), screen(-1, nearY * tick, floor), SKTextAlign.Center);
        Text(Format(data.XValue(data.XCount - 1)), screen(1, nearY * tick, floor), SKTextAlign.Center);
        Text(options.XLabel, screen(0, nearY * title, floor), SKTextAlign.Center);

        Text(Format(data.YValue(0)), screen(nearX * tick, -1, floor), SKTextAlign.Center);
        Text(Format(data.YValue(data.YCount - 1)), screen(nearX * tick, 1, floor), SKTextAlign.Center);
        Text(options.YLabel, screen(nearX * title, 0, floor), SKTextAlign.Center);

        // Value axis up the left-most floor corner, labels to its left.
        var (zx, zy) = corners.MinBy(c => projection.Project(c.Item1, c.Item2, floor).X);
        using var axis = new SKPaint { IsAntialias = true, Style = SKPaintStyle.Stroke, Color = textPaint.Color, StrokeWidth = density };
        canvas.DrawLine(screen(zx, zy, floor), screen(zx, zy, top), axis);
        Text(Format(data.ZMin), screen(zx, zy, floor), SKTextAlign.Right, -22);
        Text(Format(data.ZMax), screen(zx, zy, top), SKTextAlign.Right, -22);
        Text(options.ZLabel, screen(zx, zy, top), SKTextAlign.Center, 0, -18);
    }

    private static string Format(double value) => value.ToString("0.##");

    /// <summary>Scales and centres the projected unit box into the canvas, leaving a margin for axis labels.</summary>
    private readonly record struct ViewTransform(double MidX, double MidY, double Scale, double CentreX, double CentreY)
    {
        public static ViewTransform Fit(SurfaceProjection projection, int width, int height, float margin)
        {
            var box = new[] { -1d, 1d }
                .SelectMany(x => new[] { -1d, 1d }, (x, y) => (x, y))
                .SelectMany(c => new[] { -SurfaceMesh.HalfHeight, SurfaceMesh.HalfHeight }, (c, z) => projection.Project(c.x, c.y, z))
                .ToList();
            double minX = box.Min(p => p.X), maxX = box.Max(p => p.X), minY = box.Min(p => p.Y), maxY = box.Max(p => p.Y);
            double scale = Math.Min((width - 2 * margin) / Math.Max(maxX - minX, 1e-6), (height - 2 * margin) / Math.Max(maxY - minY, 1e-6));
            return new ViewTransform((minX + maxX) / 2, (minY + maxY) / 2, Math.Max(scale, 1), width / 2.0, height / 2.0);
        }

        public SKPoint ToScreen(ProjectedPoint p) => ToScreen(p.X, p.Y);
        public SKPoint ToScreen(Point2 p) => ToScreen(p.X, p.Y);
        private SKPoint ToScreen(double x, double y) => new((float)(CentreX + (x - MidX) * Scale), (float)(CentreY - (y - MidY) * Scale));
    }
}
