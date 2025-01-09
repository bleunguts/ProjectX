using CsvHelper;
using ProjectX.Core;
using ProjectX.MachineLearning.Tests;
using System.Globalization;

namespace ProjectX.MachineLearning;
public class KaggleFileBasedStockMarketDataSource : IStockMarketSource
{
    public Task<IEnumerable<MarketPrice>> GetPrices(string ticker, DateTime from, DateTime to)
    {
        if (ticker == "AAPL")
        {
            var configuration = new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture)
            {
                PrepareHeaderForMatch = args => args.Header.ToLower(),
            };
            using (var reader = new StreamReader("AAPL.csv"))
            using (var csv = new CsvReader(reader, configuration))
            {
                var kagglePrices = csv.GetRecords<KaggleStockData>().ToList();
                var marketPrices = kagglePrices
                                        .Select(record => record.ToMarketPrice())
                                        .Where(record => record.Date >= from.Date && record.Date <= to.Date)
                                        .ToList();
                return Task.FromResult((IEnumerable<MarketPrice>)(marketPrices));
            }
        }
        throw new Exception($"There are no data for ticker: {ticker}");
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

    public Task<IEnumerable<global::Skender.Stock.Indicators.Quote>> GetQuote(string ticker, DateTime from, DateTime to)
    {
        throw new NotImplementedException();
    }
    #endregion
}
