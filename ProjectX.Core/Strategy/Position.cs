namespace ProjectX.Core.Strategy;

public class Position
{
    private DateTime dateIn;
    private double priceIn;
    private double shares;
    public ActivePositionState PositionState { get; protected set; }

    public Position()
    {
        PositionState = new InactivePositionState();
    }
    public double Shares => shares;
    public double PriceIn => priceIn;
    public DateTime DateIn => dateIn;
    public bool IsLongPosition() => PositionState.PositionStatus == PositionStatus.POSITION_LONG;
    public bool IsShortPosition() => PositionState.PositionStatus == PositionStatus.POSITION_SHORT;
    public bool IsActive => PositionState.IsActive;
    public void EnterPosition(PositionStatus status, DateTime dateIn, double priceIn, double shares)
    {
        PositionState = new LiveActivePositionState(status);
        this.dateIn = dateIn;
        this.priceIn = priceIn;
        this.shares = shares;
    }
    public void ExitPosition()
    {
        PositionState = new InactivePositionState();
    }
}
