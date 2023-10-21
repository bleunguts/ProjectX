using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ProjectX.Core.Strategy;

public enum PositionStatus { POSITION_NONE = 0, POSITION_LONG, POSITION_SHORT }

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

public abstract class ActivePositionState
{    
    public PositionStatus PositionStatus { get; protected set; }        
    public bool IsActive { get; protected set; }    
}

public class InactivePositionState : ActivePositionState
{
    public InactivePositionState()
    {
        PositionStatus = PositionStatus.POSITION_NONE;
        IsActive = false;   
    }    
}


public class LiveActivePositionState : ActivePositionState
{   
    public LiveActivePositionState(PositionStatus status)
    {        
        PositionStatus = status;
        IsActive = true;
    }    
}