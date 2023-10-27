using MatthiWare.FinancialModelingPrep.Model.StockTimeSeries;
using MatthiWare.FinancialModelingPrep;
using ProjectX.Core;
using Skender.Stock.Indicators;

namespace ProjectX.MarketData;

public class FMPStockMarketSource : IStockMarketSource
{
    private readonly IFinancialModelingPrepApiClient _api;

    public FMPStockMarketSource()
    {
        _api = FinancialModelingPrepApiClientFactory.CreateClient(new FinancialModelingPrepOptions()
        {
            ApiKey = "35fdfe7c1a0d49e6ca2283bb073fea3a"
        });
    }
   
    public async Task<IEnumerable<MarketPrice>> GetPrices(string ticker, DateTime from, DateTime to)
    {
        var result = await _api.StockTimeSeries.GetHistoricalDailyPricesAsync(ticker, from.ToString("yyyy-MM-dd"), to.ToString("yyyy-MM-dd"));
        var response = result.Data;
        ThrowIfBadResponse(ticker, from, to, response);
        return response.Historical.Select(h => h.ToMarketPrice(ticker));
    }
    public async Task<IEnumerable<double?>> GetHurst(string ticker, DateTime from, DateTime to) 
    {       
        var quotes = await GetQuote(ticker, from, to);
        return quotes.GetHurst().Select(x => x.HurstExponent);
    }

    public async Task<IEnumerable<Quote>> GetQuote(string ticker, DateTime from, DateTime to)
    {
        var result = await _api.StockTimeSeries.GetHistoricalDailyPricesAsync(ticker, from.ToString("yyyy-MM-dd"), to.ToString("yyyy-MM-dd"));
        var response = result.Data;
        ThrowIfBadResponse(ticker, from, to, response);
        return response.Historical.Select(h => h.ToQuote());
    }

    private void ThrowIfBadResponse(string ticker, DateTime from, DateTime to, HistoricalPriceResponse response)
    {
        if (response.Historical == null)
        {
            throw new Exception($"Cannot fetch any data for ticker {ticker} for periods {from.ToShortDateString()} to {to.ToShortDateString()} sourceProvider: {this}");
        }
    }


    public override string ToString() => $"FinancialModelingPrep market source {_api.ToString()}";
}