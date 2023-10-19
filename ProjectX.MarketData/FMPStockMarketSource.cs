using MatthiWare.FinancialModelingPrep.Model.StockTimeSeries;
using MatthiWare.FinancialModelingPrep;
using ProjectX.Core;

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
        
        return response.Historical.Select(h => h.ToMarketPrice());
    }
}