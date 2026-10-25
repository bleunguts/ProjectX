namespace ShellX.Fx;

/// <summary>Maps prices to positions on a vertical price ladder centred on <paramref name="Center"/> and <paramref name="Span"/> tall.</summary>
public record SpreadLadderScale(double Center, double Span)
{
    /// <summary>0 at the top of the ladder (highest price) to 1 at the bottom.</summary>
    public double YFraction(double price) => 0.5 - (price - Center) / Span;

    /// <summary>The round-step prices that fall on the ladder, highest first, so the rungs stay still while the centre drifts.</summary>
    public IReadOnlyList<double> Rungs(double step)
    {
        const double tolerance = 1e-9;
        var top = (long)Math.Floor((Center + Span / 2) / step + tolerance);
        var bottom = (long)Math.Ceiling((Center - Span / 2) / step - tolerance);

        var rungs = new List<double>();
        for (var index = top; index >= bottom; index--)
            rungs.Add(index * step);
        return rungs;
    }
}
