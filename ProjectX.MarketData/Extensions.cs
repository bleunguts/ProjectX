using MatthiWare.FinancialModelingPrep.Model.StockMarket;
using MatthiWare.FinancialModelingPrep.Model.StockTimeSeries;
using ProjectX.Core;
using Skender.Stock.Indicators;

namespace ProjectX.MarketData;

public static class Extensions
{
    public static MarketPrice ToMarketPrice(this HistoricalPriceItem p, string ticker) => new()
    {
        Open = Convert.ToDecimal(p.Open),
        Close = Convert.ToDecimal(p.Close),
        High = Convert.ToDecimal(p.High),
        Low = Convert.ToDecimal(p.Low),
        Volume = Convert.ToDecimal(p.Volume),
        Date = DateTime.Parse(p.Date),
        Ticker = ticker,
    };

    public static Quote ToQuote(this HistoricalPriceItem p) => new()
    {
        Open = Convert.ToDecimal(p.Open),
        Close = Convert.ToDecimal(p.Close),
        High = Convert.ToDecimal(p.High),
        Low = Convert.ToDecimal(p.Low),
        Volume = Convert.ToDecimal(p.Volume),
        Date = DateTime.Parse(p.Date)
    };


    public static StockMarketSymbol ToStockMarketSymbol(this StockMarketSymbolResponse s) => new() 
    {
        CompanyName = s.CompanyName,
        Ticker = s.Ticker,
        Price = s.Price,
        Changes = s.Changes,
        ChangesPercentage = s.ChangesPercentage,
    };
}
