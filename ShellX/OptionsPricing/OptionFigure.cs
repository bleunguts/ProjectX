using ProjectX.Core.Requests;

namespace ShellX.OptionsPricing;

/// <summary>A price cut for display: the figure people read (whole part and two decimals) and the small trailing digits.</summary>
public record OptionFigureParts(string Big, string Small);

public static class OptionFigure
{
    public static OptionFigureParts Split(double value)
    {
        if (!double.IsFinite(value))
            return new OptionFigureParts("--", string.Empty);

        var text = value.ToString("0.0000");
        return new OptionFigureParts(text[..^2], text[^2..]);
    }
}

public static class SelectedMaturity
{
    /// <summary>The result row for the chosen index, clamped to the rows that exist; null when there are no results.</summary>
    public static MaturityAndOptionGreeksResultPair? Pick(IReadOnlyList<MaturityAndOptionGreeksResultPair> results, int index) =>
        results.Count == 0 ? null : results[Math.Clamp(index, 0, results.Count - 1)];
}
