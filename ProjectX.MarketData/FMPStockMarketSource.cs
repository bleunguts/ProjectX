using MatthiWare.FinancialModelingPrep.Model.StockTimeSeries;
using MatthiWare.FinancialModelingPrep;
using ProjectX.Core;
using Skender.Stock.Indicators;
using MatthiWare.FinancialModelingPrep.Model;
using MatthiWare.FinancialModelingPrep.Model.StockMarket;

namespace ProjectX.MarketData;

public class FMPStockMarketSource : IStockMarketSource, IRealStockMarketSource
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
        ThrowIfBadResponse(ticker, from, to, result);
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
        ThrowIfBadResponse(ticker, from, to, result);
        return response.Historical.Select(h => h.ToQuote());
    }

    public async Task<IEnumerable<StockMarketSymbol>> GetHighestGainerStocks()
    {
        var result = await _api.StockMarket.GetBiggestGainerStocksAsync();
        var response = result.Data;
        ThrowIfBadResponse(result);
        return response.Select(r => r.ToStockMarketSymbol());
    }

    public async Task<IEnumerable<StockMarketSymbol>> GetMostActiveStocks()
    {
        var result = await _api.StockMarket.GetMostActiveStocksAsync();
        var response = result.Data;
        ThrowIfBadResponse(result);
        return response.Select(r => r.ToStockMarketSymbol());
    }

    private void ThrowIfBadResponse(ApiResponse<List<StockMarketSymbolResponse>> result)
    {
        ThrowIfError(result);

        var response = result.Data;
        if (response == null)
        {
            throw new Exception($"Cannot fetch any data for GetBiggestGainerStocksAsync api call sourceProvider: {this}");
        }
    }

    private void ThrowIfBadResponse(string ticker, DateTime from, DateTime to, ApiResponse<HistoricalPriceResponse> result)
    {
        ThrowIfError(result);

        var response = result.Data;
        if (response.Historical == null)
        {
            throw new Exception($"Cannot fetch any data for ticker {ticker} for periods {from.ToShortDateString()} to {to.ToShortDateString()} sourceProvider: {this}");
        }
    }

    private static void ThrowIfError<T>(ApiResponse<T> result) where T: class
    {
        if (result.HasError)
        {
            throw new Exception($"Error occured: {result.Error}");
        }
    }

    public override string ToString() => $"FinancialModelingPrep market source {_api.ToString()}";
}