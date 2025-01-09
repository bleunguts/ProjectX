using CsvHelper;
using ProjectX.Core;
using ProjectX.MachineLearning;
using ProjectX.MarketData;
using System.Globalization;

namespace ProjectX.MarketData;
public class KaggleFileBasedStockMarketSource : IStockMarketSource
{
    private const string KaggleFolder = "Kaggle";
    // DataSets: "suyashlakhani/apple-stock-prices-20152020"
    private string AppleKaggleFilePath = Path.Combine(KaggleFolder, "AAPL.csv");

    public Task<IEnumerable<MarketPrice>> GetPrices(string ticker, DateTime from, DateTime to)
    {
        switch (ticker)
        {
            case "AAPL":
            {
                var configuration = new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    PrepareHeaderForMatch = args => args.Header.ToLower(),
                };
                using (var reader = new StreamReader(AppleKaggleFilePath))
                using (var csv = new CsvReader(reader, configuration))
                {
                    var marketPrices = csv.GetRecords<KaggleStockData>()
                                            .Select(record => record.ToMarketPrice())
                                            .Where(record => record.Date >= from.Date && record.Date <= to.Date)
                                            .ToList();
                    return Task.FromResult((IEnumerable<MarketPrice>)marketPrices);
                }
            }
            default:
                throw new Exception($"Ticker data not found: {ticker}.");
        }
    }

    #region Not Implemented
    public Task<IEnumerable<StockMarketSymbol>> GetHighestGainerStocks()
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<double?>> GetHurst(string ticker, DateTime from, DateTime to)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<StockMarketSymbol>> GetMostActiveStocks()
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Skender.Stock.Indicators.Quote>> GetQuote(string ticker, DateTime from, DateTime to)
    {
        throw new NotImplementedException();
    }
    #endregion
}
