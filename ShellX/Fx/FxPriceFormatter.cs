using ProjectX.Core;

namespace ShellX.Fx;

public static class FxPriceFormatter
{
    public static string LatestPrice(SpotPrice price) => price.ToPrettifiedBidAskPrice();

    public static string SpreadText(SpotPrice price) => $"({price.Spread()} pips spread)";

    /// <summary>The Gateway sends every tick to every client; only the selected pair is shown.</summary>
    public static bool IsForPair(SpotPrice price, string selectedPair) => price.CurrencyPair == selectedPair;
}
