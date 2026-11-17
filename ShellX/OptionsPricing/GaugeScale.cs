using ProjectX.Core;
using ProjectX.Core.Requests;

namespace ShellX.OptionsPricing;

public static class GaugeScale
{
    private const double PadFraction = 0.1;

    /// <summary>Where a value sits within a range, 0 to 1; a flat range sits in the middle.</summary>
    public static double Fraction(double value, double min, double max) =>
        max <= min ? 0.5 : Math.Clamp((value - min) / (max - min), 0, 1);

    /// <summary>The value range with a tenth of its height added each side, so the extremes are not pegged at the dial ends.</summary>
    public static (double Min, double Max) Range(IEnumerable<double> values)
    {
        var list = values.ToList();
        if (list.Count == 0)
            return (0, 1);

        double min = list.Min(), max = list.Max();
        if (max <= min)
            return (min - 1, max + 1);

        var pad = (max - min) * PadFraction;
        return (min - pad, max + pad);
    }

    /// <summary>The dial's arc length in degrees for a 0 to 1 fraction (a 270 degree sweep).</summary>
    public static double SweepDegrees(double fraction) => Math.Clamp(fraction, 0, 1) * 270;
}

/// <summary>One Greek on a dial: its value for the selected maturity and where that sits across all maturities.</summary>
public record GaugeReading(string Name, double Value, double Fraction, string Format)
{
    public string Text => Value.ToString(Format);
}

public static class GreekGaugeSet
{
    /// <summary>The five Greeks for the selected maturity, each placed within its own range across all the results.</summary>
    public static IReadOnlyList<GaugeReading> From(IReadOnlyList<MaturityAndOptionGreeksResultPair> results, int selectedIndex)
    {
        var selected = SelectedMaturity.Pick(results, selectedIndex);

        GaugeReading Reading(string name, string format, Func<OptionGreeksResult, double> pick)
        {
            if (selected is null)
                return new GaugeReading(name, 0, 0.5, format);

            var (min, max) = GaugeScale.Range(results.Select(r => pick(r.OptionGreeks)));
            var value = pick(selected.OptionGreeks);
            return new GaugeReading(name, value, GaugeScale.Fraction(value, min, max), format);
        }

        return
        [
            Reading("Delta", "0.000", g => g.delta),
            Reading("Gamma", "0.0000", g => g.gamma),
            Reading("Theta", "0.00", g => g.theta),
            Reading("Rho", "0.00", g => g.rho),
            Reading("Vega", "0.00", g => g.vega),
        ];
    }
}
