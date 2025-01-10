namespace ProjectX.MachineLearning;

public class StockPriceMovementPredictorKNNClassification : StockPriceMovementPredictor
{
    private readonly int _kNumber;

    public StockPriceMovementPredictorKNNClassification(int kNumber)
    {
        _kNumber = kNumber;
    }

    public override PredictStockPriceMovementsResult PredictStockPriceMovements(IEnumerable<ExpectedStockPriceMovement> expectedMovements)
    {
        //train model
        //train model with test data
        //Knn.Compute(inputTrain[i]) to predict classification point

        //TODO: add confusion matrix/contingency table/error matrix to examine the perf of the ML algo

        throw new NotImplementedException();
    }
}
