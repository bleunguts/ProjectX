
using ProjectX.Core;

namespace ProjectX.MachineLearning;

public record PredictStockPriceMovementsResult();

public class StockPriceMovementPredictorKNNClassificationModel
{    
    private readonly int _kNumber;

    public StockPriceMovementPredictorKNNClassificationModel(int kNumber)
    {
        _kNumber = kNumber;
    }

    public List<ExpectedStockPriceMovement> EvaluateExpectedStockPriceMovement(List<MarketPrice> marketPrices)
    {
        List<ExpectedStockPriceMovement> expectedMovements = new();
        for(int i = 0; i < marketPrices.Count; i++)
        {
            var current = marketPrices[i];
            if (i == 0)
            {
                expectedMovements.Add(new ExpectedStockPriceMovement
                {                    
                    Date = current.Date,
                    Low = current.Low,
                    High = current.High,
                    Close = current.Close,
                    Open = current.Open,
                    Expected = -1
                });
            }
            else
            {
                var previousPrice = marketPrices[i - 1].Close;
                var currentPrice = current.Close;
                var expected = currentPrice switch
                {
                    _ when currentPrice > previousPrice => 1,
                    _ when currentPrice < previousPrice => 2,
                    _ => 0
                };

                expectedMovements.Add(new ExpectedStockPriceMovement
                {
                    Date = current.Date,
                    Low = current.Low,
                    High = current.High,
                    Close = current.Close,
                    Open = current.Open,
                    Expected = expected
                });
            }
        }
        return expectedMovements;
    }

    public PredictStockPriceMovementsResult PredictStockPriceMovements(IEnumerable<ExpectedStockPriceMovement> expectedMovements)
    {        
        Console.WriteLine($"Predicting Stock Price Movements using KNN, Tick Count {expectedMovements.Count()}");
        // train model 
        // train model with test data
        // Knn.Compute(inputTrain[i]) to predict classification point

        // TODO: support confusion matrix

        return new PredictStockPriceMovementsResult();
    }
}