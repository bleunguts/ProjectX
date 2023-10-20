using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectX.Core.Strategy;

public abstract class ActivePosition
{
    public static ActivePosition INACTIVE = new InactivePosition();
    public abstract bool IsActive { get;}
}
public class InactivePosition : ActivePosition
{
    public override bool IsActive => false;
}
public class LiveActivePosition: ActivePosition
{
    private readonly PositionStatus positionState;
    private readonly DateTime dateIn;
    private readonly double priceIn;
    private readonly double shares;    
    public LiveActivePosition(PositionStatus positionState, DateTime dateIn, double priceIn, double shares)
    {
        this.positionState = positionState;
        this.dateIn = dateIn;
        this.priceIn = priceIn;
        this.shares = shares;
    }
    public override bool IsActive => true;
    public double Shares => shares;
    public PositionStatus PositionStatus => positionState;
    public double PriceIn => priceIn;
    public DateTime DateIn => dateIn;
    public bool IsLongPosition() => PositionStatus == PositionStatus.POSITION_LONG;
    public bool IsShortPosition() => PositionStatus == PositionStatus.POSITION_SHORT;
}
