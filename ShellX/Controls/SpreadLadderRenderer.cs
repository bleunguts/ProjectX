using ShellX.Fx;
using SkiaSharp;

namespace ShellX.Controls;

/// <summary>
/// Draws a vertical price ladder centred on the mid: round-step rungs, the bid and ask lines, the spread band
/// between them and a fading trail of recent ticks. No MAUI dependency, so it can render headlessly.
/// </summary>
public static class SpreadLadderRenderer
{
    private const float LabelFontDips = 10;
    private const float LabelColumnDips = 46;
    private const int TrailSpacingDips = 6;

    private static readonly SKColor BidColor = SKColor.Parse("#2563EB");
    private static readonly SKColor AskColor = SKColor.Parse("#F59E0B");

    public record Options(double Bid, double Ask, IReadOnlyList<LadderTick> Trail, bool HasPrice, SKColor TextColor, SKColor Background);

    /// <param name="density">Pixels per device-independent unit.</param>
    public static void Draw(SKCanvas canvas, int width, int height, float density, Options options)
    {
        canvas.Clear(SKColors.Transparent);

        using var font = new SKFont { Size = LabelFontDips * density };
        using var text = new SKPaint { Color = options.TextColor, IsAntialias = true };

        if (!options.HasPrice)
        {
            canvas.DrawText("waiting for ticks", width / 2f, height / 2f, SKTextAlign.Center, font, text);
            return;
        }

        var mid = (options.Bid + options.Ask) / 2;
        var spread = options.Ask - options.Bid;
        var span = Math.Max(0.0006, spread * 6);
        var step = span <= 0.0012 ? 0.0001 : 0.0005;
        var scale = new SpreadLadderScale(mid, span);

        float left = LabelColumnDips * density;
        float Y(double price) => (float)(scale.YFraction(price) * height);

        using var grid = new SKPaint { Color = options.TextColor.WithAlpha(40), StrokeWidth = density, IsAntialias = true };
        foreach (var rung in scale.Rungs(step))
        {
            var y = Y(rung);
            canvas.DrawLine(left, y, width, y, grid);
            canvas.DrawText(rung.ToString("0.0000"), left - 4 * density, y + font.Size / 3, SKTextAlign.Right, font, text);
        }

        using var band = new SKPaint { Color = BidColor.WithAlpha(40), Style = SKPaintStyle.Fill };
        canvas.DrawRect(left, Y(options.Ask), width - left, Math.Max(Y(options.Bid) - Y(options.Ask), density), band);

        using var ask = new SKPaint { Color = AskColor, StrokeWidth = 2 * density, IsAntialias = true };
        using var bid = new SKPaint { Color = BidColor, StrokeWidth = 2 * density, IsAntialias = true };
        canvas.DrawLine(left, Y(options.Ask), width, Y(options.Ask), ask);
        canvas.DrawLine(left, Y(options.Bid), width, Y(options.Bid), bid);

        // Newest tick at the right edge, older ones fading away to the left.
        using var dot = new SKPaint { IsAntialias = true, Style = SKPaintStyle.Fill };
        var trail = options.Trail;
        for (var k = 0; k < trail.Count; k++)
        {
            var tick = trail[trail.Count - 1 - k];
            var x = width - 10 * density - k * TrailSpacingDips * density;
            if (x < left)
                break;

            var alpha = (byte)Math.Max(30, 255 - k * 18);
            dot.Color = AskColor.WithAlpha(alpha);
            canvas.DrawCircle(x, Y(tick.Ask), 2.5f * density, dot);
            dot.Color = BidColor.WithAlpha(alpha);
            canvas.DrawCircle(x, Y(tick.Bid), 2.5f * density, dot);
        }

        // Labels last, on a chip, so the trail never runs through the numbers.
        DrawChip(canvas, font, $"ASK {options.Ask:0.00000}", width - 4 * density, Y(options.Ask) - 4 * density, AskColor, options.Background, density);
        DrawChip(canvas, font, $"BID {options.Bid:0.00000}", width - 4 * density, Y(options.Bid) + font.Size + 2 * density, BidColor, options.Background, density);
    }

    private static void DrawChip(SKCanvas canvas, SKFont font, string label, float right, float baseline, SKColor color, SKColor background, float density)
    {
        var textWidth = font.MeasureText(label);
        var pad = 3 * density;
        var chip = new SKRect(right - textWidth - pad, baseline - font.Size, right + pad, baseline + 3 * density);
        using var fill = new SKPaint { Color = background.WithAlpha(225), Style = SKPaintStyle.Fill, IsAntialias = true };
        canvas.DrawRoundRect(chip, 3 * density, 3 * density, fill);
        using var text = new SKPaint { Color = color, IsAntialias = true };
        canvas.DrawText(label, right, baseline, SKTextAlign.Right, font, text);
    }
}
