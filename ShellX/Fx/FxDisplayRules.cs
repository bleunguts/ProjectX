using System.Globalization;

namespace ShellX.Fx;

public record FxQuantityChip(string Label, int Value);

public static class FxQuantity
{
    public static IReadOnlyList<FxQuantityChip> Chips { get; } =
    [
        new("1k", 1_000),
        new("10k", 10_000),
        new("100k", 100_000),
        new("1m", 1_000_000),
    ];
}

public static class FxLiveness
{
    private static readonly TimeSpan DefaultWindow = TimeSpan.FromSeconds(3);

    /// <summary>The stream counts as live while ticks keep arriving within the window.</summary>
    public static bool IsLive(DateTimeOffset? lastTick, DateTimeOffset now, TimeSpan? window = null) =>
        lastTick is { } last && now - last <= (window ?? DefaultWindow);
}

public static class PnlSign
{
    /// <summary>-1, 0 or 1 for a formatted PnL cell such as "-2,001"; anything that is not a number is 0.</summary>
    public static int Of(string text) =>
        decimal.TryParse(text, NumberStyles.Number, CultureInfo.CurrentCulture, out var value) ? Math.Sign(value) : 0;
}

public static class FxChartRange
{
    private const double PadFraction = 0.1;
    private const double MinimumBand = 0.00005;

    /// <summary>The value range with a tenth of its height added above and below, so the line never touches the edges.</summary>
    public static (double Min, double Max)? Padded(IEnumerable<double> values)
    {
        var list = values.ToList();
        if (list.Count == 0)
            return null;

        double min = list.Min(), max = list.Max();
        double pad = max > min ? (max - min) * PadFraction : MinimumBand;
        return (min - pad, max + pad);
    }
}

public static class CountUp
{
    /// <summary>The number shown <paramref name="t"/> (0 to 1) of the way through a count from one value to another, easing out.</summary>
    public static double Value(double from, double to, double t)
    {
        t = Math.Clamp(t, 0, 1);
        var eased = 1 - Math.Pow(1 - t, 3);
        return from + (to - from) * eased;
    }
}
