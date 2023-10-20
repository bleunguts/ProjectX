using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectX.Core.Strategy;

public class ActivePosition
{
    private readonly PnlTradeType tradeType;
    private readonly DateTime dateIn;
    private readonly double priceIn;
    private readonly double shares;

    public ActivePosition(PnlTradeType tradeType, DateTime dateIn, double priceIn, double shares)
    {
        this.tradeType = tradeType;
        this.dateIn = dateIn;
        this.priceIn = priceIn;
        this.shares = shares;
    }

    private ActivePosition() { }

    public bool IsActive => !Equals(INACTIVE);

    public double Shares => shares;
    public PnlTradeType TradeType => tradeType;
    public double PriceIn => priceIn;
    public DateTime DateIn => dateIn;
    public bool IsLongPosition() => TradeType == PnlTradeType.POSITION_LONG;
    public bool IsShortPosition() => TradeType == PnlTradeType.POSITION_SHORT;

    public static ActivePosition INACTIVE = new ActivePosition();
    public override bool Equals(object rhs)
    {
        var other = (ActivePosition)rhs;
        if (other == null) return false;
        return tradeType == other.tradeType &&
                dateIn.Equals(other.dateIn) &&
                priceIn == other.priceIn &&
                shares == other.shares;
    }
    public override int GetHashCode()
    {
        return new { tradeType, dateIn, priceIn, shares }.GetHashCode();
    }    
}
