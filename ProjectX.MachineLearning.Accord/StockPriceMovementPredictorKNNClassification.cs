using Accord.MachineLearning;
using Accord.Math.Distances;
using Accord.Statistics.Analysis;

namespace ProjectX.MachineLearning.Accord;

public class StockPriceMovementPredictorKNNClassification : StockPriceMovementPredictor
{
    private readonly int _kNumber;

    public StockPriceMovementPredictorKNNClassification(int kNumber)
    {
        _kNumber = kNumber;
    }

    public override Task<StockPriceMovementResult> PredictStockPriceMovements(IEnumerable<ExpectedStockPriceTrendDirection> expectedMovements)
    {
        //train model
        //train model with test data
        //Knn.Compute(inputTrain[i]) to predict classification point

        // Create some sample learning data. In this data,
        // the first two instances belong to a class, the
        // four next belong to another class and the last
        // three to yet another.

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

        var knn = new KNearestNeighbors<string>(k: _kNumber, distance: new Levenshtein());

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

        return Task.FromResult(new StockPriceMovementResult
        {
        });
    }
}
