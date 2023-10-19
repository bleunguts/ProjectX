namespace ProjectX.Core
{
    public interface IStockMarketSource
    {
        Task<IEnumerable<MarketPrice>> GetPrices(string ticker, DateTime from, DateTime to);
    }

    public class MarketPrice
    {
        public double Close { get; set; }
    }
}