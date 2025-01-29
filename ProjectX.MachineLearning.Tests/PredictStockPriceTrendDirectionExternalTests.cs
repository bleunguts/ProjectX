using Accord.MachineLearning;
using Accord.Math.Distances;
using Accord.Statistics.Analysis;
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
    public void KnnSimpleExample()
    {
        string[] inputs =
        {
            "Car",     // class 0
            "Bar",     // class 0
            "Jar",     // class 0

            "Charm",   // class 1
            "Chair"    // class 1
        };

        int[] outputs =
        {
            0, 0, 0,  // First three are from class 0
            1, 1,     // And next two are from class 1
        };

        var knn = new KNearestNeighbors<string>(k: 3, distance: new Levenshtein());

        // We learn the algorithm:
        knn.Learn(inputs, outputs);

        // After the algorithm has been created, we can use it:
        int answer = knn.Decide("Chars"); // answer should be 1.

        //TODO: add confusion matrix/contingency table/error matrix to examine the perf of the ML algo
        // Let's say we would like to compute the error matrix for the classifier:
        var cm = ConfusionMatrix.Estimate(knn, inputs, outputs);

        // We can use it to estimate measures such as 
        double error = cm.Error;  // should be 0
        double acc = cm.Accuracy; // should be 1
        double kappa = cm.Kappa;  // should be 1

        Console.WriteLine($"Answer of the KNN algorithm is {answer}");
        Console.WriteLine($"error is {error}");
        Console.WriteLine($"acc is {acc}");
        Console.WriteLine($"kappa is {kappa}");
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
