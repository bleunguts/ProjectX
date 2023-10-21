namespace ProjectX.Core.Strategy
{
    public class StrategyPnlFactory
    {
        public static StrategyPnl NewPnl(DateTime date, string ticker, decimal price, decimal signal) => NewPnl(date, ticker, (double) price, (double) signal, 0.0, 0.0, 0.0, 0.0, 0.0, 0, new Position());

        public static StrategyPnl NewPnl(DateTime date, string ticker, double price, double signal, double pnLCum, double pnLDaily, double pnlPerTrade, double pnlDailyHold, double pnlCumHold, int numTrades, Position position)
        {
            var positionStatus = PositionStatus.POSITION_NONE;
            DateTime? dateIn = null;
            double? priceIn = null;
        
            if (position.IsActive)
            {
                positionStatus = position.PositionState.PositionStatus;
                priceIn = position.PriceIn;
                dateIn = position.DateIn;
            }

            return new StrategyPnl(date, ticker, price, signal, pnLCum, pnLDaily, pnlPerTrade, pnlDailyHold, pnlCumHold, positionStatus, dateIn, priceIn, numTrades);
        }
    }
}
