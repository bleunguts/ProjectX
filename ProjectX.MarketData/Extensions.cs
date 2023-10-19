using MatthiWare.FinancialModelingPrep.Model.StockTimeSeries;
using ProjectX.Core;

namespace ProjectX.MarketData;

public static class Extensions
{
    public static MarketPrice ToMarketPrice(this HistoricalPriceItem p) => new()
    {
        Close = p.Close
    };
}