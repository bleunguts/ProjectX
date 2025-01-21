using ProjectX.Core;
using ProjectX.MarketData;
using System.Security.Cryptography;

namespace ProjectX.MachineLearning.Tests;

public class PredictStockPriceValueExternalTests
{
    private KaggleFileBasedStockMarketSource _marketDataSource = new KaggleFileBasedStockMarketSource();
    private List<MarketPrice> _marketPrices;

    [SetUp]
    public async Task BeforeTest()
    {
        _marketPrices = (await _marketDataSource.GetPrices("AAPL", new DateTime(2015, 5, 27), new DateTime(2016, 5, 27))).ToList();
    }

    [Ignore("As this will try out all algorithms it takes a long time to run")]
    [Test]
    public async Task PredictAppleStockPricesWithAutoML()
    {
        var model = new StockPriceMovementPredictorAutoML();

        var expectedMovements = model.EvaluateExpectedStockPriceMovement(_marketPrices);

        var predictionResults = await model.PredictStockPriceMovements(expectedMovements);
        Dump(predictionResults);

        Assert.That(predictionResults.predictedPrices.Count, Is.GreaterThan(0));
    }   

    [Test]
    public async Task PredictAppleStockPricesWithMLNet()
    {
        // ...start doing predictions using KNN
        var model = new StockPriceMovementPredictorMLNetClassification();

        // ...calculate expected price by subtracting previous price
        var expectedMovements = model.EvaluateExpectedStockPriceMovement(_marketPrices);

        // ...use ML.NET classification models Poisson to predict 
        var predictionResults = await model.PredictStockPriceMovements(expectedMovements);
        Dump(predictionResults);

        Assert.That(predictionResults.predictedPrices.Count, Is.GreaterThan(0));
    }

    private static void Dump(StockPriceMovementResult predictionResults)
    {
        Console.WriteLine($"PriceTick Count: {predictionResults.priceTicksCount}");
        Console.WriteLine(string.Join(Environment.NewLine, predictionResults.aux));
        Console.WriteLine($"{predictionResults.predictionsCount} predictions.");
        foreach (var item in predictionResults.predictedPrices)
        {
            Console.WriteLine(item.ToString());
        }
    }
}