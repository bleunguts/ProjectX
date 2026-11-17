using ProjectX.Core;

namespace ShellX.Fx;

public enum FxTickDirection { Flat, Up, Down }

public static class FxTickDirections
{
    /// <summary>Which way the mid price moved between two ticks; the first tick has no direction.</summary>
    public static FxTickDirection Between(SpotPrice? previous, SpotPrice current)
    {
        if (previous is not { } before)
            return FxTickDirection.Flat;

        var then = Mid(before);
        var now = Mid(current);
        return now > then ? FxTickDirection.Up : now < then ? FxTickDirection.Down : FxTickDirection.Flat;
    }

    public static string Arrow(FxTickDirection direction) => direction switch
    {
        FxTickDirection.Up => "▲",
        FxTickDirection.Down => "▼",
        _ => string.Empty,
    };

    private static decimal Mid(SpotPrice price) => (price.BidPrice + price.AskPrice) / 2;
}
