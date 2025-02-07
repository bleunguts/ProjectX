using Accord.MachineLearning;
using Accord.MachineLearning.VectorMachines;
using Accord.MachineLearning.VectorMachines.Learning;
using Accord.Math.Distances;
using Accord.Statistics.Analysis;
using Accord.Statistics.Kernels;

namespace ProjectX.MachineLearning.Accord;

public class StockPriceMovementPredictorSVMClassification 
{    
    public Task<StockPriceMovementResult> PredictStockPriceMovements(double[][] inputs, int[] outputs, int numberOfInputs, int numberOfClasses)
    {
        // Create some sample learning data. In this data,
        // the first two instances belong to a class, the
        // four next belong to another class and the last
        // three to yet another.        
        var machine = new MulticlassSupportVectorMachine<Gaussian>(numberOfInputs, new Gaussian(), numberOfClasses);
        var teacher = new MulticlassSupportVectorLearning<Gaussian>(machine);

        // We learn the algorithm
        var svm = teacher.Learn(inputs, outputs);
        
        int actualNumberOfClasses = svm.NumberOfClasses; // should be 2 (positive or negative)
        int actualNumberOfInputs = svm.NumberOfInputs;  // should be 2 (1)
        
        // After the algorithm has been created, we can use it:        
        int[] answers = machine.Decide(inputs); 

        //TODO: add confusion matrix/contingency table/error matrix to examine the perf of the ML algo
        // Let's say we would like to compute the error matrix for the classifier:
        var cm = GeneralConfusionMatrix.Estimate(machine, inputs, outputs);

        // We can use it to estimate measures such as 
        double error = cm.Error;  // should be 0
        double acc = cm.Accuracy; // should be 1
        double kappa = cm.Kappa;  // should be 1

        Console.WriteLine($"There are {answers.Length} answers: ");
        foreach(var answer in answers)
        {
            Console.WriteLine(answer);
        }
        Console.WriteLine($"error is {error}");
        Console.WriteLine($"acc is {acc}");
        Console.WriteLine($"kappa is {kappa}");

        return Task.FromResult(new StockPriceMovementResult
        {
        });
    }
}
