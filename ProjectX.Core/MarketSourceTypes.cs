using Skender.Stock.Indicators;

namespace ProjectX.Core
{
    public interface IStockMarketSource
    {
        Task<IEnumerable<MarketPrice>> GetPrices(string ticker, DateTime from, DateTime to);
        Task<IEnumerable<double?>> GetHurst(string ticker, DateTime from, DateTime to);
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
}