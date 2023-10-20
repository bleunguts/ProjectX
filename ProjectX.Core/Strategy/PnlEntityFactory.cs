namespace ProjectX.Core.Strategy
{
    public class PnlEntityFactory
    {
        public static PnlEntity Build(DateTime date, string ticker, decimal price, decimal signal) => Build(date, ticker, (double) price, (double) signal, 0.0, 0.0, 0.0, 0.0, 0.0, 0, ActivePosition.INACTIVE);

        public static PnlEntity Build(DateTime date, string ticker, double price, double signal, double pnLCum, double pnLDaily, double pnlPerTrade, double pnlDailyHold, double pnlCumHold, int numTrades, ActivePosition activePosition)
        {
            var tradeType = PnlTradeType.POSITION_NONE;
            DateTime? dateIn = null;
            double? priceIn = null;

            if (activePosition.IsActive)
            {
                tradeType = activePosition.TradeType;
                priceIn = activePosition.PriceIn;
                dateIn = activePosition.DateIn;
            }

            return new PnlEntity(date, ticker, price, signal, pnLCum, pnLDaily, pnlPerTrade, pnlDailyHold, pnlCumHold, tradeType, dateIn, priceIn, numTrades);
        }
    }
}
