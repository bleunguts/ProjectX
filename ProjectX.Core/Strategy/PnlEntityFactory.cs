namespace ProjectX.Core.Strategy
{
    public class PnlEntityFactory
    {
        public static PnlEntity NewPnlEntity(DateTime date, string ticker, decimal price, decimal signal) => NewPnlEntity(date, ticker, (double) price, (double) signal, 0.0, 0.0, 0.0, 0.0, 0.0, 0, ActivePosition.INACTIVE);

        public static PnlEntity NewPnlEntity(DateTime date, string ticker, double price, double signal, double pnLCum, double pnLDaily, double pnlPerTrade, double pnlDailyHold, double pnlCumHold, int numTrades, ActivePosition activePosition)
        {
            var positionStatus = PositionStatus.POSITION_NONE;
            DateTime? dateIn = null;
            double? priceIn = null;

            LiveActivePosition? position = activePosition as LiveActivePosition;
            if (activePosition.IsActive && position != null)
            {
                positionStatus = position.PositionStatus;
                priceIn = position.PriceIn;
                dateIn = position.DateIn;
            }

            return new PnlEntity(date, ticker, price, signal, pnLCum, pnLDaily, pnlPerTrade, pnlDailyHold, pnlCumHold, positionStatus, dateIn, priceIn, numTrades);
        }
    }
}
