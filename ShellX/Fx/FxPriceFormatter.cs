using ProjectX.Core;

namespace ShellX.Fx;

/// <summary>A price cut for display: a small prefix, the big figure traders read, and a small last digit (1.08 | 43 | 2).</summary>
public record FxPriceParts(string Prefix, string Big, string Fraction);

public static class FxPriceFormatter
{
    public static string LatestPrice(SpotPrice price) => price.ToPrettifiedBidAskPrice();

    public static string SpreadText(SpotPrice price) => $"({price.Spread()} pips spread)";

    /// <summary>The Gateway sends every tick to every client; only the selected pair is shown.</summary>
    public static bool IsForPair(SpotPrice price, string selectedPair) => price.CurrencyPair == selectedPair;

    public static string SellLabel(SpotPrice? price) => price is { } p ? $"Sell {p.BidPrice:0.00000}" : "Sell";

    public static string BuyLabel(SpotPrice? price) => price is { } p ? $"Buy {p.AskPrice:0.00000}" : "Buy";

    public static FxPriceParts Split(decimal? price)
    {
        if (price is not { } value)
            return new FxPriceParts("--", "--", string.Empty);

        var text = value.ToString("0.00000");
        return new FxPriceParts(text[..^3], text[^3..^1], text[^1..]);
    }
}
