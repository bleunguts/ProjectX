using ProjectX.Core;

namespace ProjectX.MachineLearning;

public static class KaggleStockDataExtensions
{
    public static MarketPrice ToMarketPrice(this KaggleStockData stockData)
    {
        return new MarketPrice
        {
            Ticker = stockData.Symbol,
            High = stockData.AdjHigh,
            Low = stockData.AdjLow,
            Open = stockData.Open,
            Close = stockData.Close,
            Date = DateTimeOffset.Parse(stockData.Date).Date,
            Volume = stockData.Volume
        };
    }
}