
using Microsoft.CodeAnalysis.Operations;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers;
using System.Collections.Immutable;
using System.Text;

namespace ProjectX.MachineLearning;

public class StockPriceMovementPredictorMLNetClassification : StockPriceMovementPredictor
{ 
    public override Task<StockPriceMovementResult> PredictStockPriceMovements(IEnumerable<ExpectedStockPriceTrendDirection> expectedMovements)
    {
        Console.WriteLine($"Predicting Stock Price Movements, Tick Count {expectedMovements.Count()}");
        var context = new MLContext();

        // Convert DateTime to numeric features
        var sanitisedData = expectedMovements.Select(m => new
        {
            Expected = Convert.ToSingle(m.Expected),
            DayOfYear = Convert.ToSingle(m.Date.Value.DayOfYear),
            High = Convert.ToSingle(m.High.Value),
            Low = Convert.ToSingle(m.Low.Value),
            Close = Convert.ToSingle(m.Close.Value),
            Open = Convert.ToSingle(m.Open.Value),
            Predicted = Convert.ToSingle(m.Predicted),
            Timestamp = ((DateTimeOffset)m.Date.Value).ToUnixTimeMilliseconds()
        });

        var data = context.Data.LoadFromEnumerable(sanitisedData);

        var split = context.Data.TrainTestSplit(data, testFraction: 0.2);

        var features = split.TrainSet.Schema
            .Where(col => col.Name != "Expected" && col.Name != "Timestamp")
            .Select(col => col.Name)
            .ToArray();

        Console.WriteLine($"Features: {string.Join(",", features)}");

        // ML.NET enforces that the Label Column is the column to predict
        // Predict 'Expected' values
        var pipeline = context.Transforms.CopyColumns("Label", "Close")
            .Append(context.Transforms.Concatenate("Features", features))
            .Append(context.Transforms.Conversion.ConvertType("Features", outputKind: Microsoft.ML.Data.DataKind.Single))
            .Append(context.Regression.Trainers.LbfgsPoissonRegression());

        var model = pipeline.Fit(split.TrainSet);
        var predictions = model.Transform(split.TestSet);
        var metrics = context.Regression.Evaluate(predictions);
       
        Dump(predictions.Preview(5).RowView);

        var predictedPrices = predictions.GetColumn<float>("Score").ToArray();
        string[] aux = [
            $"RSquared: {metrics.RSquared}"
        ];
        StockPriceMovementResult stockPriceMovements = 
            new(sanitisedData.Count(), 
                predictedPrices, 
                predictedPrices.Length, 
                aux);
        return Task.FromResult(stockPriceMovements);
    }

    private void Dump(ImmutableArray<DataDebuggerPreview.RowInfo> rowView)                
    {
        var text = new StringBuilder();
        foreach (var row in rowView)
        {
            var timestamp = (long)row.Values.First(col => col.Key == "Timestamp").Value;
            DateTime date = DateTimeOffset.FromUnixTimeMilliseconds((long)timestamp).UtcDateTime;
            var labelValue = (float)row.Values.First(col => col.Key == "Label").Value;
            var predictedValue = (float)row.Values.First(col => col.Key == "Score").Value;            
            text.AppendLine($"({date.ToShortDateString()}) predictions [Label, {labelValue}] vs [Score, {predictedValue}]:");
            foreach (var col in row.Values.Where(r => r.Key != "Timestamp" && r.Key != "Label"))
            {
                text.AppendLine($"\t {col}");                
            }
        }
        Console.WriteLine(text.ToString());
    }
}