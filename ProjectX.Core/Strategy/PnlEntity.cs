using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectX.Core.Strategy
{
    public enum PositionStatus { POSITION_NONE = 0, POSITION_LONG, POSITION_SHORT }
    public class PnlEntity
    {              
        public PnlEntity(DateTime date, string ticker, double price, double signal, double pnLCum, double pnLDaily, double pnlPerTrade, double pnlDailyHold, double pnlCumHold, PositionStatus tradeType, DateTime? dateIn, double? priceIn, int numTrades)
        {
            Date = date;
            Ticker = ticker;
            Price = price;
            Signal = signal;
            TradeType = tradeType;
            NumTrades = numTrades;
            PnLCum = pnLCum;
            PnLDaily = pnLDaily;
            PnlPerTrade = pnlPerTrade;
            PnLDailyHold = pnlDailyHold;
            PnLCumHold = pnlCumHold;
            DateIn = dateIn;
            PriceIn = priceIn;
        }
     
        public string Ticker { get; set; }
        public DateTime Date { get; set; }
        public double Price { get; set; }
        public double Signal { get; set; }
        public PositionStatus TradeType { get; set; }
        public DateTime? DateIn { get; set; }
        public double? PriceIn { get; set; }

        public int NumTrades { get; set; }
        public double PnlPerTrade { get; set; }
        public double PnLDaily { get; set; }
        public double PnLCum { get; set; }
        public double PnLDailyHold { get; set; }
        public double PnLCumHold { get; set; }

        public override string ToString()
        {
            return $"Ticker={Ticker},Date={Date.ToShortDateString()},Price={Price},Signal={Signal},Type={TradeType},NumTrades={NumTrades},DateIn={DateIn},PriceIn{PriceIn},PnlPerTrade={PnlPerTrade},PnlDaily={PnLDaily},PnlCum={PnLCum},PnlDailyHold={PnLDailyHold},PnlCumHold={PnLCumHold}";
        }
    }
}
