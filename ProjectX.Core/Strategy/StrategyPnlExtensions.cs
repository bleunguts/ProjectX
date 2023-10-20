namespace ProjectX.Core.Strategy
{
    public static class StrategyPnlExtensions
    {
        public static void PrintStrategyFor(this List<StrategyPnl> pnls, PositionStatus tradeType)
        {
            (int enterTradeIndex, int exitTradeIndex) = pnls.DeconstructTradeTimeline(PositionStatus.POSITION_SHORT);
            pnls.Print();
            pnls.PrintStrategyFor(enterTradeIndex, exitTradeIndex);
        }
        public static void PrintStrategyFor(this List<StrategyPnl> pnls, int start, int end)
        {
            Console.WriteLine("Strategy PnL:");
            for (int i = start; i <= end; i++)
            {
                StrategyPnl p = pnls[i];
                //{0:0.##}
                Console.WriteLine($"{p.Date.ToShortDateString()},Price={p.Price:0.##},Signal={p.Signal:0.##},PnlPerTrade={p.PnlPerTrade:0.##},PnlDaily={p.PnLDaily:0.##},PnlCum={p.PnLCum:0.##},PnlDailyHold={p.PnLDailyHold:0.##},PnlCumHold={p.PnLCumHold:0.##}");
            }
        }

        public static void Print(this List<StrategyPnl> pnls) => pnls.ForEach(p => Console.WriteLine(p));

        public static (int enterTradeIndex, int exitTradeIndex) DeconstructTradeTimeline(this List<StrategyPnl> pnls, PositionStatus pnlTradeType)
        {
            var first = pnls.FindIndex(p => p.TradeType == pnlTradeType);
            var last = pnls.FindLastIndex(p => p.TradeType == pnlTradeType);
            return (first, last);
        }
    }
}
