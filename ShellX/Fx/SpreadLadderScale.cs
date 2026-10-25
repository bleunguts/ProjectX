namespace ShellX.Fx;

/// <summary>Maps prices to positions on a vertical price ladder centred on <paramref name="Center"/> and <paramref name="Span"/> tall.</summary>
public record SpreadLadderScale(double Center, double Span)
{
    private static readonly double[] NiceSteps = [0.00001, 0.00002, 0.00005, 0.0001, 0.0002, 0.0005, 0.001, 0.002, 0.005, 0.01, 0.02, 0.05, 0.1];

    /// <summary>The finest round step that keeps the ladder to about <paramref name="maxRungs"/> rungs, so the price labels never pile up.</summary>
    public static double NiceStep(double span, int maxRungs = 8) =>
        NiceSteps.FirstOrDefault(step => span / step <= maxRungs + 1e-9, NiceSteps[^1]);

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

/// <summary>One recent bid/ask pair for the ladder trail.</summary>
public record LadderTick(double Bid, double Ask);
