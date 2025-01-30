using ProjectX.Core;
using ProjectX.MachineLearning;
using ProjectX.MachineLearning.Accord;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shell;

public interface IMachineLearningApiClient
{
    Task<StockPriceMovementResult> Knn(int k, DateTime trainingFromDate, DateTime trainingToDate);
    Task<IEnumerable<MarketPrice>> LoadPrices(string ticker, DateTime from, DateTime to);
}

public class MachineLearningApiClient : IMachineLearningApiClient
{
    private readonly IStockMarketSource _stockMarketSource;

    public MachineLearningApiClient(IStockMarketSource stockMarketSource)
    {
        _stockMarketSource = stockMarketSource;
    }

    public async Task<StockPriceMovementResult> Knn(int k, DateTime trainingFromDate, DateTime trainingToDate)
    {
        var knn = new StockPriceMovementPredictorKNNClassification(3);
        var prices = await knn.PredictStockPriceMovements(new[] { new ExpectedStockPriceTrendDirection() });
        return prices;
    }

    public async Task<IEnumerable<MarketPrice>> LoadPrices(string ticker, DateTime from, DateTime to)
    {
        var prices = await _stockMarketSource.GetPrices(ticker, from, to);
        return prices;
    }
}

