using ProjectX.Core;
using ProjectX.MarketData;
using System.Security.Cryptography;

namespace ProjectX.MachineLearning.Tests;
public class StockPriceMovementPredictorExternalTests
{
    private KaggleFileBasedStockMarketSource _marketDataSource = new KaggleFileBasedStockMarketSource();
    private List<MarketPrice> _marketPrices;

    [SetUp]
    public async Task BeforeTest()
    {
        _marketPrices = (await _marketDataSource.GetPrices("AAPL", new DateTime(2015, 5, 27), new DateTime(2016, 5, 27))).ToList();
    }

    [Test]
    public async Task TrainAutoMLWithAAplHistoricalPrices()
    {
        var model = new StockPriceMovementPredictorAutoML();

        var expectedMovements = model.EvaluateExpectedStockPriceMovement(_marketPrices);

        var predictionResults = await model.PredictStockPriceMovements(expectedMovements);

        Assert.That(predictionResults, Is.Not.Null);
    }

    [Ignore("TODO: Implement KNN Machine Learning with Accord")]
    [Test]
    public void TrainKnnWithAAplHistoricalPrices()
    {
        var model = new StockPriceMovementPredictorKNNClassification(kNumber: 4);
        
        var expectedMovements = model.EvaluateExpectedStockPriceMovement(_marketPrices);

        var predictionResults = model.PredictStockPriceMovements(expectedMovements);
    }

    [Test]
    public async Task TrainClassifierMLNetWithAAplHistoricalPrices()
    {                
        // ...start doing predictions using KNN
        var model = new StockPriceMovementPredictorMLNetClassification();

        // ...calculate expected price by subtracting previous price
        var expectedMovements = model.EvaluateExpectedStockPriceMovement(_marketPrices);

        // ...use ML.NET classification models Poission to predict 
        var predictionResults = model.PredictStockPriceMovements(expectedMovements);
    }
}