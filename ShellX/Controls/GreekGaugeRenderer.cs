using ShellX.OptionsPricing;
using SkiaSharp;

namespace ShellX.Controls;

/// <summary>
/// A radial dial: a 270 degree track, a glowing arc swept to <see cref="Options.Fraction"/>, a bead at its end
/// and the value and name in the middle. No MAUI dependency, so it can render headlessly.
/// </summary>
public static class GreekGaugeRenderer
{
    private const float StartAngle = 135;
    // Blue through teal to amber; interpolating blue straight to amber would pass through grey.
    private static readonly SKColor Low = SKColor.Parse("#3B82F6");
    private static readonly SKColor Mid = SKColor.Parse("#2DD4BF");
    private static readonly SKColor High = SKColor.Parse("#FBBF24");

    public record Options(double Fraction, string ValueText, string Label, SKColor TextColor);

    /// <param name="density">Pixels per device-independent unit.</param>
    public static void Draw(SKCanvas canvas, int width, int height, float density, Options options)
    {
        canvas.Clear(SKColors.Transparent);

        var stroke = 7 * density;
        var radius = Math.Min(width, height) / 2f - stroke * 1.6f;
        if (radius <= 0)
            return;

        float cx = width / 2f, cy = height / 2f;
        var oval = new SKRect(cx - radius, cy - radius, cx + radius, cy + radius);
        var sweep = (float)GaugeScale.SweepDegrees(options.Fraction);
        var color = Ramp(options.Fraction);

        using var track = new SKPaint { Style = SKPaintStyle.Stroke, StrokeWidth = stroke, StrokeCap = SKStrokeCap.Round, IsAntialias = true, Color = options.TextColor.WithAlpha(45) };
        canvas.DrawArc(oval, StartAngle, 270, false, track);

        if (sweep > 0.5f)
        {
            // The glow: the same arc, wider and blurred, drawn first.
            using var glow = new SKPaint
            {
                Style = SKPaintStyle.Stroke, StrokeWidth = stroke * 1.9f, StrokeCap = SKStrokeCap.Round, IsAntialias = true,
                Color = color.WithAlpha(150), MaskFilter = SKMaskFilter.CreateBlur(SKBlurStyle.Normal, 5 * density),
            };
            canvas.DrawArc(oval, StartAngle, sweep, false, glow);

            using var arc = new SKPaint { Style = SKPaintStyle.Stroke, StrokeWidth = stroke, StrokeCap = SKStrokeCap.Round, IsAntialias = true, Color = color };
            canvas.DrawArc(oval, StartAngle, sweep, false, arc);
        }

        // A bead at the end of the arc (or at the start when the dial is empty).
        var endAngle = (StartAngle + sweep) * MathF.PI / 180f;
        using var bead = new SKPaint { Style = SKPaintStyle.Fill, IsAntialias = true, Color = SKColors.White };
        canvas.DrawCircle(cx + radius * MathF.Cos(endAngle), cy + radius * MathF.Sin(endAngle), stroke * 0.7f, bead);

        using var valueFont = new SKFont { Size = Math.Max(radius * 0.46f, 9 * density), Embolden = true };
        using var labelFont = new SKFont { Size = Math.Max(radius * 0.26f, 8 * density) };
        using var valuePaint = new SKPaint { Color = options.TextColor, IsAntialias = true };
        using var labelPaint = new SKPaint { Color = options.TextColor.WithAlpha(170), IsAntialias = true };
        canvas.DrawText(options.ValueText, cx, cy + valueFont.Size * 0.1f, SKTextAlign.Center, valueFont, valuePaint);
        canvas.DrawText(options.Label.ToUpperInvariant(), cx, cy + valueFont.Size * 0.1f + labelFont.Size * 1.5f, SKTextAlign.Center, labelFont, labelPaint);
    }

    private static SKColor Ramp(double fraction) =>
        fraction < 0.5 ? Lerp(Low, Mid, fraction * 2) : Lerp(Mid, High, (fraction - 0.5) * 2);

    private static SKColor Lerp(SKColor a, SKColor b, double t)
    {
        t = Math.Clamp(t, 0, 1);
        byte Channel(byte x, byte y) => (byte)Math.Round(x + (y - x) * t);
        return new SKColor(Channel(a.Red, b.Red), Channel(a.Green, b.Green), Channel(a.Blue, b.Blue));
    }
}
