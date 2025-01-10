
using Microsoft.ML;
using Microsoft.ML.Trainers;
using System.Text;

namespace ProjectX.MachineLearning;

public class StockPriceMovementPredictorMLNetClassification : StockPriceMovementPredictor
{ 
    public override Task<PredictStockPriceMovementsResult> PredictStockPriceMovements(IEnumerable<ExpectedStockPriceMovement> expectedMovements)
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
            Timestamp = ((DateTimeOffset) m.Date.Value).ToUnixTimeMilliseconds()
        });

        var data = context.Data.LoadFromEnumerable(sanitisedData);

        var split = context.Data.TrainTestSplit(data, testFraction: 0.2);

        var features = split.TrainSet.Schema
            .Where(col => col.Name != "Expected" && col.Name != "Timestamp")
            .Select(col => col.Name)            
            .ToArray();

        Console.WriteLine($"Features: {string.Join(",",features)}");

        // ML.NET enforces that the Label Column is the column to predict
        // Predict 'Expected' values
        var pipeline = context.Transforms.CopyColumns("Label", "Expected")
            .Append(context.Transforms.Concatenate("Features", features))
            .Append(context.Transforms.Conversion.ConvertType("Features", outputKind: Microsoft.ML.Data.DataKind.Single))
            .Append(context.Regression.Trainers.LbfgsPoissonRegression());

        var model = pipeline.Fit(split.TrainSet);

        var predictions = model.Transform(split.TestSet);        

        var metrics = context.Regression.Evaluate(predictions);

        var result = predictions.Preview(5);
        Console.WriteLine($"PriceTick Count: {sanitisedData.Count()}");
        Console.WriteLine($"R^2 - {metrics.RSquared}");
        Console.WriteLine($"Predictions: ");
        Console.WriteLine($"{result}");
        
        var text = new StringBuilder();
        foreach (var row in result.RowView)
        {            
            var timestamp = (long) row.Values.First(col => col.Key == "Timestamp").Value;
            DateTime date = DateTimeOffset.FromUnixTimeMilliseconds((long)timestamp).UtcDateTime;
            var labelValue = (float)row.Values.First(col => col.Key == "Label").Value;
            var expectedValue  = (float)row.Values.First(col => col.Key == "Expected").Value;
            var label = Enum.Parse<StockPriceTrend>(labelValue.ToString());
            var expected = Enum.Parse<StockPriceTrend>(expectedValue.ToString());
            var pass = label == expected ? "OK" : "FAIL";
            text.AppendLine($"({date.ToShortDateString()}) predictions {pass} [Label, {label}] vs [Expected, {expected}]:");

            foreach (var col in row.Values.Where(r => r.Key != "Timestamp" && r.Key != "Label" && r.Key != "Expected"))
            {                                               
                text.AppendLine($"\t {col}");                
            }            
        }
        Console.WriteLine(text.ToString());

        return Task.FromResult(new PredictStockPriceMovementsResult());
    }
}