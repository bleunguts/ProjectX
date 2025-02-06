using Accord.MachineLearning;
using Accord.Math.Distances;
using Accord.Statistics.Analysis;

namespace ProjectX.MachineLearning.Accord;

public class StockPriceMovementPredictorKNNClassification 
{
    private readonly int _kNumber;

    public StockPriceMovementPredictorKNNClassification(int kNumber)
    {
        _kNumber = kNumber;
    }

    public Task<StockPriceMovementResult> PredictStockPriceMovements(string[] inputs, int[] outputs)
    {
        // Create some sample learning data. In this data,
        // the first two instances belong to a class, the
        // four next belong to another class and the last
        // three to yet another.

        var knn = new KNearestNeighbors<string>(k: _kNumber, distance: new Levenshtein());

        // We learn the algorithm:
        knn.Learn(inputs, outputs);
        int numberOfClasses = knn.NumberOfClasses; // should be 2 (positive or negative)
        int numberOfInputs = knn.NumberOfInputs;  // should be 2 (1)
        
        // After the algorithm has been created, we can use it:        
        int answer = knn.Decide("predict"); // answer should be 1.        

        //TODO: add confusion matrix/contingency table/error matrix to examine the perf of the ML algo
        // Let's say we would like to compute the error matrix for the classsifier:
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
