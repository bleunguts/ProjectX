using Microsoft.ML;
using Microsoft.ML.AutoML;
using Microsoft.ML.Data;
using System.Collections.Immutable;
using System.Net.Mime;

namespace ProjectX.MachineLearning;

public class StockPriceMovementPredictorAutoML : StockPriceMovementPredictor
{
    private readonly string _dataPath = Path.GetFullPath(@".\Kaggle\AAPL.csv");

    public override async Task<PredictStockPriceMovementsResult> PredictStockPriceMovements(IEnumerable<ExpectedStockPriceMovement> expectedMovements)
    {
        Console.WriteLine($"Predicting Stock Price Movements using best Algorithm, Tick Count {expectedMovements.Count()}");
        var context = new MLContext();
               
        var data = context.Data.LoadFromTextFile(_dataPath, new TextLoader.Options()
        {
            HasHeader = true,
            Separators = new char[] { ',' }
        });

        var split = context.Data.TrainTestSplit(data, testFraction: 0.2);

        ColumnInferenceResults columnInference =
            context.Auto().InferColumns(_dataPath, labelColumnName: "close", groupColumns: false);

        SweepablePipeline pipeline =
            context.Auto().Featurizer(data, columnInference.ColumnInformation)
            .Append(context.Auto().Regression());

        var experiment = context.Auto().CreateExperiment();
        experiment
            .SetPipeline(pipeline)
            .SetRegressionMetric(RegressionMetric.RSquared)
            .SetTrainingTimeInSeconds(60)
            .SetDataset(data);

        context.Log += (_, e) =>
        {
            if(e.Source.Equals("AutoMLExperiment"))
                Console.WriteLine(e.RawMessage);
        };

        var experimentResults = await experiment.RunAsync();
        var model = experimentResults.Model;
        var metric = experimentResults.Metric;
        
        // TODO: WIP return results
        var results = model.Preview(data, 5);
        

        return new PredictStockPriceMovementsResult();
    }    
}
