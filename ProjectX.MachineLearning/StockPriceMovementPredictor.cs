using ProjectX.Core;

namespace ProjectX.MachineLearning;

public record PredictStockPriceMovementsResult();

public abstract class StockPriceMovementPredictor
{
    public List<ExpectedStockPriceMovement> EvaluateExpectedStockPriceMovement(List<MarketPrice> marketPrices)
    {
        List<ExpectedStockPriceMovement> expectedMovements = new();
        for (int i = 0; i < marketPrices.Count; i++)
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
                    Expected = StockPriceTrend.Flat
                });
            }
            else
            {
                var previousPrice = marketPrices[i - 1].Close;
                var currentPrice = current.Close;
                var expected = currentPrice switch
                {
                    _ when currentPrice > previousPrice => StockPriceTrend.Upward,
                    _ when currentPrice < previousPrice => StockPriceTrend.Downward,
                    _ => StockPriceTrend.Flat
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

    public abstract PredictStockPriceMovementsResult PredictStockPriceMovements(IEnumerable<ExpectedStockPriceMovement> expectedMovements);    
}
