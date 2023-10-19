using MatthiWare.FinancialModelingPrep.Model.StockTimeSeries;
using ProjectX.Core;

namespace ProjectX.MarketData;

public static class Extensions
{
    public static MarketPrice ToMarketPrice(this HistoricalPriceItem p) => new()
    {
        Open = Convert.ToDecimal(p.Open),
        Close = Convert.ToDecimal(p.Close),
        High = Convert.ToDecimal(p.High),   
        Low = Convert.ToDecimal(p.Low),
        Volume = Convert.ToDecimal(p.Volume),
        Date = DateTime.Parse(p.Date),
    };
}