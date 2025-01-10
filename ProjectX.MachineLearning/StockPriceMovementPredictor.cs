using Microsoft.ML;
using Microsoft.ML.Data;
using ProjectX.Core;
using System.Text;

namespace ProjectX.MachineLearning;

public record StockPriceMovementResult(long priceTicksCount = 0, float[] predictedPrices = null, int predictionsCount = 0, string[] aux = null);

public abstract class StockPriceMovementPredictor
{ 
    public List<ExpectedStockPriceTrendDirection> EvaluateExpectedStockPriceMovement(List<MarketPrice> marketPrices)
    {
        List<ExpectedStockPriceTrendDirection> expectedMovements = new();
        for (int i = 0; i < marketPrices.Count; i++)
        {
            var current = marketPrices[i];
            if (i == 0)
            {
                expectedMovements.Add(new ExpectedStockPriceTrendDirection
                {
                    Date = current.Date,
                    Low = current.Low,
                    High = current.High,
                    Close = current.Close,
                    Open = current.Open,
                    Expected = StockPriceTrendDirection.Flat
                });
            }
            else
            {
                var previousPrice = marketPrices[i - 1].Close;
                var currentPrice = current.Close;
                var expected = currentPrice switch
                {
                    _ when currentPrice > previousPrice => StockPriceTrendDirection.Upward,
                    _ when currentPrice < previousPrice => StockPriceTrendDirection.Downward,
                    _ => StockPriceTrendDirection.Flat
                };

                expectedMovements.Add(new ExpectedStockPriceTrendDirection
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

    public abstract Task<StockPriceMovementResult> PredictStockPriceMovements(IEnumerable<ExpectedStockPriceTrendDirection> expectedMovements);    
}
