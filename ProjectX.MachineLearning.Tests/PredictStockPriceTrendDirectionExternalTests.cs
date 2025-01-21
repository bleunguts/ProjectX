using ProjectX.Core;
using ProjectX.MachineLearning.Accord;
using ProjectX.MarketData;

namespace ProjectX.MachineLearning.Tests;

public class PredictStockPriceTrendDirectionExternalTests
{
    private KaggleFileBasedStockMarketSource _marketDataSource = new KaggleFileBasedStockMarketSource();
    private List<MarketPrice> _marketPrices;

    [SetUp]
    public async Task BeforeTest()
    {
        _marketPrices = (await _marketDataSource.GetPrices("AAPL", new DateTime(2015, 5, 27), new DateTime(2016, 5, 27))).ToList();
    }

    [Test]
    public void PredictAppleStockPriceTrendDirectionUsingKnn()
    {
        var model = new StockPriceMovementPredictorKNNClassification(kNumber: 4);

        var expectedMovements = model.EvaluateExpectedStockPriceMovement(_marketPrices);

        var predictionResults = model.PredictStockPriceMovements(expectedMovements);

        Assert.That(predictionResults, Is.Not.Null);
    }
}
