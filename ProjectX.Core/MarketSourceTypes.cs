using Skender.Stock.Indicators;

namespace ProjectX.Core;

public interface IRealStockMarketSource: IStockMarketSource
{
}

public interface IStockMarketSource
{
    Task<IEnumerable<MarketPrice>> GetPrices(string ticker, DateTime from, DateTime to);
    Task<IEnumerable<double?>> GetHurst(string ticker, DateTime from, DateTime to);
    Task<IEnumerable<Quote>> GetQuote(string ticker, DateTime from, DateTime to);
    Task<IEnumerable<StockMarketSymbol>> GetHighestGainerStocks();
    Task<IEnumerable<StockMarketSymbol>> GetMostActiveStocks();
}
public class MarketPrice : IQuote
{       
    public decimal Open { get; set; }

    public decimal High { get; set; }

    public decimal Low { get; set;  }

    public decimal Volume { get; set; }

    public DateTime Date { get; set;  }

    public decimal Close { get; set; }
    public string Ticker { get; set; }
}

public class StockMarketSymbol
{
    public string CompanyName { get; set; }
    public string Ticker { get; set; }
    public string Price { get; set; }
    public double Changes { get; set; }
    public string ChangesPercentage { get; set; }
}
public class KaggleStockData
{
    public string Symbol { get; set; }
    public string Date { get; set; }
    public decimal Close { get; set; }
    public decimal High { get; set; }
    public decimal Low { get; set; }
    public decimal Open { get; set; }
    public decimal Volume { get; set; }
    public decimal AdjClose { get; set; }
    public decimal AdjHigh { get; set; }
    public decimal AdjLow { get; set; }
    public decimal AdjOpen { get; set; }
    public decimal AdjVolume { get; set; }
    public decimal DivCash { get; set; }
    public decimal SplitFactor { get; set; }
}

public class KaggleStockDataRaw
{
    public string symbol { get; set; }
    public string date { get; set; }
    public decimal close { get; set; }
    public decimal high { get; set; }
    public decimal low { get; set; }
    public decimal open { get; set; }
    public decimal volume { get; set; }
    public decimal adjClose { get; set; }
    public decimal adjHigh { get; set; }
    public decimal adjLow { get; set; }
    public decimal adjOpen { get; set; }
    public decimal adjVolume { get; set; }
    public decimal divCash { get; set; }
    public decimal splitFactor { get; set; }
}