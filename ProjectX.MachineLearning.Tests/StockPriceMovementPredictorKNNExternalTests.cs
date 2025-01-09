using System.Security.Cryptography;

namespace ProjectX.MachineLearning.Tests;
public class StockPriceMovementPredictorKNNExternalTests
{
    [Test]
    public async Task GetPricesShouldOnlyReturnPricesWithinDateRange()
    {
        var marketDataSource = new KaggleFileBasedStockMarketDataSource();
        var startDate = new DateTime(2015, 5, 27);
        var nextDay = new DateTime(2015, 5, 28);
        var marketPrices = (await marketDataSource.GetPrices("AAPL",startDate, nextDay )).ToList();

        CollectionAssert.AreEquivalent(new [] { startDate,nextDay }, marketPrices.Select(p => p.Date));
    }
    [Test]
    public async Task TrainKnnWithAAplHistoricalPricesAsync()
    {
        var marketDataSource = new KaggleFileBasedStockMarketDataSource();
        var marketPrices = (await marketDataSource.GetPrices("AAPL", new DateTime(2015, 5, 27), new DateTime(2016, 5, 27))).ToList();

        // ...start doing predictions using KNN
        var model = new StockPriceMovementPredictorKNNClassificationModel(kNumber: 4);

        // ...calculate expected price by subtracting previous price
        var expectedMovements = model.EvaluateExpectedStockPriceMovement(marketPrices);

        // ...use KNN to predict 
        var predictionResults = model.PredictStockPriceMovements(expectedMovements);
    }
}