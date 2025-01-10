namespace ProjectX.MachineLearning;

public class StockPriceMovementPredictorKNNClassification : StockPriceMovementPredictor
{
    private readonly int _kNumber;

    public StockPriceMovementPredictorKNNClassification(int kNumber)
    {
        _kNumber = kNumber;
    }

    public PredictStockPriceMovementsResult PredictStockPriceMovements(IEnumerable<ExpectedStockPriceMovement> expectedMovements)
    {
        //train model
        //train model with test data
        //Knn.Compute(inputTrain[i]) to predict classification point

        //TODO: support confusion matrix

        throw new NotImplementedException();
    }
}
