using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.ML;
using Microsoft.ML.AutoML;
using Microsoft.ML.Data;
using System.Collections.Immutable;
using System.Net.Mime;
using System.Text;

namespace ProjectX.MachineLearning;

public class StockPriceMovementPredictorAutoML : StockPriceMovementPredictor
{
    private readonly string _dataPath = Path.GetFullPath(@".\Kaggle\AAPL.csv");

    public override async Task<StockPriceMovementResult> PredictStockPriceMovements(IEnumerable<ExpectedStockPriceMovement> expectedMovements)
    {
        Console.WriteLine($"Predicting Stock Price Movements using best Algorithm, Tick Count {expectedMovements.Count()}");
        var context = new MLContext();

        var data = context.Data.LoadFromTextFile(_dataPath, new TextLoader.Options()
        {
            HasHeader = true,
            Separators = [',']
        });

        var split = context.Data.TrainTestSplit(data, testFraction: 0.2);
        var trainData = split.TrainSet;
        var testData = split.TestSet;

        ColumnInferenceResults columnInference =
            context.Auto().InferColumns(_dataPath, labelColumnName: "adjclose", groupColumns: false);

        SweepablePipeline pipeline =
            context.Auto().Featurizer(data, columnInference.ColumnInformation)
            .Append(context.Auto().Regression());

        var experiment = context.Auto().CreateExperiment();
        experiment
            .SetPipeline(pipeline)
            .SetRegressionMetric(RegressionMetric.RSquared)
            .SetTrainingTimeInSeconds(60)
            .SetDataset(trainData);

        //context.Log += (_, e) =>
        //{
        //    if (e.Source.Equals("AutoMLExperiment"))
        //    {                
        //        Console.WriteLine(e.RawMessage);
        //    }
        //};

        // Find Optimal Algo
        var experimentResults = await experiment.RunAsync();
        TransformerChain<ITransformer> model = (TransformerChain<ITransformer>)experimentResults.Model;
        var metric = experimentResults.Metric;        
        Console.WriteLine($"Metric RSquared: {metric}");
        Console.WriteLine($"Last Algo: {model.LastTransformer.ToString()}");

        // Use Algo to make predictions
        var predictions = model.Transform(testData);
        var result = predictions.Preview(5);
        Dump(result.RowView);        

        return new StockPriceMovementResult();
    }

    private static void Dump(ImmutableArray<DataDebuggerPreview.RowInfo> rowView)
    {
        var text = new StringBuilder();
        foreach (var row in rowView)
        {
            foreach (var col in row.Values)
            {
                text.AppendLine($"\t {col}");
            }
        }
        Console.WriteLine(text.ToString());
    }
}
