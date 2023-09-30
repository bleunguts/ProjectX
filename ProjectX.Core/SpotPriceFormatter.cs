using System;
using System.ComponentModel.Composition;

namespace ProjectX.Core;

public interface ISpotPriceFormatter
{
    SpotPrice ToSpotPrice(string spotPrice);
    string PrettifySpotPrice(SpotPrice price);
}

[Export(typeof(ISpotPriceFormatter)), PartCreationPolicy(CreationPolicy.NonShared)]
public class SpotPriceFormatter : ISpotPriceFormatter
{
    public string PrettifySpotPrice(SpotPrice price) => $"{price.BidPrice.ToString("#.00000")}/{price.AskPrice.ToString("#.00000")}";
    public SpotPrice ToSpotPrice(string spotPrice)
    {
        var parts = spotPrice.Split('/');
        var bidPrice = Convert.ToDecimal(parts[0].Trim());
        var askPrice = Convert.ToDecimal(parts[1].Trim());

        return new SpotPrice("N/A", bidPrice, askPrice);
    }
}
