namespace ProjectX.Core.Strategy
{
    public static class PnlEntityExtensions
    {
        public static void PrintStrategyFor(this List<PnlEntity> pnlEntities, PositionStatus tradeType)
        {
            (int enterTradeIndex, int exitTradeIndex) = pnlEntities.DeconstructTradeTimeline(PositionStatus.POSITION_SHORT);
            pnlEntities.Print();
            pnlEntities.PrintStrategyFor(enterTradeIndex, exitTradeIndex);
        }
        public static void PrintStrategyFor(this List<PnlEntity> pnlEntities, int start, int end)
        {
            Console.WriteLine("Strategy PnL:");
            for (int i = start; i <= end; i++)
            {
                PnlEntity p = pnlEntities[i];
                //{0:0.##}
                Console.WriteLine($"{p.Date.ToShortDateString()},Price={p.Price:0.##},Signal={p.Signal:0.##},PnlPerTrade={p.PnlPerTrade:0.##},PnlDaily={p.PnLDaily:0.##},PnlCum={p.PnLCum:0.##},PnlDailyHold={p.PnLDailyHold:0.##},PnlCumHold={p.PnLCumHold:0.##}");
            }
        }

        public static void Print(this List<PnlEntity> pnlEntities) => pnlEntities.ForEach(p => Console.WriteLine(p));

        public static (int enterTradeIndex, int exitTradeIndex) DeconstructTradeTimeline(this List<PnlEntity> pnlEntities, PositionStatus pnlTradeType)
        {
            var first = pnlEntities.FindIndex(p => p.TradeType == pnlTradeType);
            var last = pnlEntities.FindLastIndex(p => p.TradeType == pnlTradeType);
            return (first, last);
        }
    }
}
