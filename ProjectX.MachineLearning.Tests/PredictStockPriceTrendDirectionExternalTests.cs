using Accord.MachineLearning;
using Accord.Math;
using Accord.Math.Distances;
using Accord.Statistics.Analysis;
using ProjectX.Core;
using ProjectX.MachineLearning.Accord;
using ProjectX.MarketData;
using System.Data;

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
    public void PredictAppleStockPriceTrendDirectionUsingClassification()
    {
        const string dir = @".\data\GE_010523_250923.csv";
        var dt = ConvertCSVtoDataTable(dir);
        // Sample input data
        //double[][] inputs =
        //{
        //    new double[] { 0 },
        //    new double[] { 3 },
        //    new double[] { 1 },
        //    new double[] { 2 },
        //};
        List<double[]> inputList = new();
        List<int> outputsList = new();
        foreach(DataRow row in dt.Rows)
        {
            var expected = (StockPriceTrendDirection) Enum.Parse(typeof(StockPriceTrendDirection), (string)row["Expected"]);
            var expectedNumber = (int)expected;
            
            double[] newRow = [
                double.Parse((string)row["Open"]),
                double.Parse((string) row["Close"]),
                double.Parse((string) row["High"]),
                double.Parse((string) row["Low"])                
            ];

            inputList.Add(newRow);
            outputsList.Add((int)expectedNumber);
        }
        double[][] inputs = inputList.ToArray();

        // Outputs for each of the inputs
        //int[] outputs =
        //{
        //    0,
        //    3,
        //    1,
        //    2,
        //};
        int[] outputs = outputsList.ToArray();

        Console.WriteLine($"Inputs lines {inputs.Length}");
        Console.WriteLine($"Outputs length {outputs.Length}");

        var model = new StockPriceMovementPredictorSVMClassification();
        var predictionResults = model.PredictStockPriceMovements(inputs, outputs, 1, 2);

        Assert.That(predictionResults, Is.Not.Null);
    }

    public static DataTable ConvertCSVtoDataTable(string strFilePath)
    {
        DataTable dt = new DataTable();
        using (StreamReader sr = new StreamReader(strFilePath))
        {
            string[] headers = sr.ReadLine()
                                .Split(',')
                                .Select(header => header.Trim())
                                .ToArray();
            foreach (string header in headers)
            {
                dt.Columns.Add(header);
            }
            while (!sr.EndOfStream)
            {
                string[] rows = sr.ReadLine().Split(',');
                DataRow dr = dt.NewRow();
                for (int i = 0; i < headers.Length; i++)
                {
                    dr[i] = rows[i];
                }
                dt.Rows.Add(dr);
            }
        }
        return dt;
    }
}
