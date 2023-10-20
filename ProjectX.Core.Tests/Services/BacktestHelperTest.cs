using ProjectX.Core.Strategy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectX.Core.Tests.Services
{
    public class BacktestHelperTest
    {
        const double notional = 10_000.0;
        const double signalIn = 2;
        const double signalOut = 0.0;
        const string ticker = "IBM";
        readonly DateTime startDate = DateTime.Now;

        // prevSignal > 2 enters short
        // prevSignal < -2 enters long
        // -2 < prevSignal < 2 is allowed out of this range it will go into short or long position
        // -2 < -1.0 is still profitable
        //  -2 < -0.1 almost profitable
        // -2 < 0 signalOut barrier breached exit position
        [Test]
        public void WhenSignalIsLessThanNegSignalItShouldEnterAndExitLongTrade()
        {
            var builder = new SignalBuilder(ticker);
            var signals = new List<PriceSignalEntity>
            {
                builder.NewSignal(-1.5),
                builder.NewSignal(-2.1),
                builder.NewSignal(-3.1), // enters long trade (prevSignal < -2) 
                builder.NewSignal(1.1),
                builder.NewSignal(-4.1)  // exit long trade (prevSignal > 0)
            };

            var pnlEntities = BacktestHelper.ComputeLongShortPnl(signals, notional, signalIn, signalOut, StrategyTypeEnum.MeanReversion, false).ToList();
            Print(pnlEntities);
            (int enterTradeIndex, int exitTradeIndex) = ToTrades(pnlEntities, PnlTradeType.POSITION_LONG);
            var enterTrade = pnlEntities[enterTradeIndex];
            Assert.That(enterTrade.Date, Is.EqualTo(signals[enterTradeIndex].Date));
            Assert.That(enterTrade.TradeType, Is.EqualTo(PnlTradeType.POSITION_LONG));
            Assert.That(enterTrade.PriceIn, Is.EqualTo(signals[enterTradeIndex].Price));
            Assert.That(enterTrade.NumTrades, Is.EqualTo(1));
            Assert.That(enterTrade.PnlPerTrade, Is.EqualTo(0));

            var exitTrade = pnlEntities[exitTradeIndex];
            Assert.That(exitTrade.Date, Is.EqualTo(signals[exitTradeIndex].Date));
            Assert.That(exitTrade.TradeType, Is.EqualTo(PnlTradeType.POSITION_LONG));
            Assert.That(exitTrade.PriceIn, Is.EqualTo(signals[enterTradeIndex].Price));
            Assert.That(exitTrade.NumTrades, Is.EqualTo(2));
            Assert.That(exitTrade.PnlPerTrade, Is.GreaterThan(0).Or.LessThan(0));

            PrintStrategy(pnlEntities, enterTradeIndex, exitTradeIndex);
        }

        // prevSignal > 2 enters short
        // prevSignal < -2 enters long
        // -2 < prevSignal < 2 is allowed out of this range it will go into short or long position
        // prevSignal > 2 
        // 3 > 2  still cool
        // 4 > 2 still cool
        // 1 > 2 not gt 2 but still cool
        // 0 > 2 breached the point exit short position
        [Test]
        public void WhenSignalIsGreaterThanSignalItShouldEnterAndExitShortTrade()
        {
            var builder = new SignalBuilder(ticker, startDate);
            var signals = new List<PriceSignalEntity>
            {
                builder.NewSignal(-1.5),
                builder.NewSignal(3.1),
                builder.NewSignal(0.1), // enters short trade (prevSignal > 2)
                builder.NewSignal(-10.1),
                builder.NewSignal(0.1)   // exit short trade (prevSignal < 0) 
            };

            var pnlEntities = BacktestHelper.ComputeLongShortPnl(signals, notional, signalIn, signalOut, StrategyTypeEnum.MeanReversion, false).ToList();
            Print(pnlEntities);

            (int enterTradeIndex, int exitTradeIndex) = ToTrades(pnlEntities, PnlTradeType.POSITION_SHORT);
            var enterTrade = pnlEntities[enterTradeIndex];
            Assert.That(enterTrade.Date, Is.EqualTo(signals[enterTradeIndex].Date));
            Assert.That(enterTrade.TradeType, Is.EqualTo(PnlTradeType.POSITION_SHORT));
            Assert.That(enterTrade.PriceIn, Is.EqualTo(signals[enterTradeIndex].Price));
            Assert.That(enterTrade.NumTrades, Is.EqualTo(1));
            Assert.That(enterTrade.PnlPerTrade, Is.EqualTo(0));

            var exitTrade = pnlEntities[exitTradeIndex];
            Assert.That(exitTrade.Date, Is.EqualTo(signals[exitTradeIndex].Date));
            Assert.That(exitTrade.TradeType, Is.EqualTo(PnlTradeType.POSITION_SHORT));
            Assert.That(exitTrade.PriceIn, Is.EqualTo(signals[enterTradeIndex].Price));
            Assert.That(exitTrade.NumTrades, Is.EqualTo(2));
            Assert.That(exitTrade.PnlPerTrade, Is.GreaterThan(0).Or.LessThan(0));

            PrintStrategy(pnlEntities, enterTradeIndex, exitTradeIndex);
        }

        [Test]
        public void WhenSignalSwingsViciouslyShouldNotEnterPositionIfAlreadyTrading()
        {
            var signalIn = 2.0;
            var signalOut = -3.0;
            var random = new Random();
            const double basePrice = 1_000_000.0;
            int counter = 0;
            var builder = new SignalBuilder(ticker, startDate);

            var signals = new List<PriceSignalEntity>
            {
                builder.NewSignal(15.5),
                builder.NewSignal(-1.1),  // enters short trade (prevSignal > 2)
                builder.NewSignal(-0.1),  // [shouldnot] enter long trade (prevSignal < -2) 
                builder.NewSignal(0.1),
                builder.NewSignal(0.4),
                builder.NewSignal(0.8),
                builder.NewSignal(0.7),
                builder.NewSignal(0.1),
                builder.NewSignal(0.4),
                builder.NewSignal(0.3),
                builder.NewSignal(0.9),
                builder.NewSignal(-10.1),
                builder.NewSignal(0.1)    // exit short trade (prevSignal < -3) 
            };

            var pnlEntities = BacktestHelper.ComputeLongShortPnl(signals, notional, signalIn, signalOut, StrategyTypeEnum.MeanReversion, false).ToList();
            Print(pnlEntities);
            (int enterTradeIndex, int exitTradeIndex) = ToTrades(pnlEntities, PnlTradeType.POSITION_SHORT);
            PrintStrategy(pnlEntities, enterTradeIndex, exitTradeIndex);
        }       

        private static (int enterTradeIndex, int exitTradeIndex) ToTrades(List<PnlEntity> pnlEntities, PnlTradeType pnlTradeType)
        {
            var first = pnlEntities.FindIndex(p => p.TradeType == pnlTradeType);
            var last = pnlEntities.FindLastIndex(p => p.TradeType == pnlTradeType);
            return (first, last);
        }

        private static void Print(List<PnlEntity> pnlEntities) => pnlEntities.ForEach(p => Console.WriteLine(p));

        private static void PrintStrategy(List<PnlEntity> pnlEntities, int start, int end)
        {
            Console.WriteLine("Strategy PnL:");
            for (int i = start; i <= end; i++)
            {
                PnlEntity p = pnlEntities[i];
                //{0:0.##}
                Console.WriteLine($"{p.Date.ToShortDateString()},Price={p.Price:0.##},Signal={p.Signal:0.##},PnlPerTrade={p.PnlPerTrade:0.##},PnlDaily={p.PnLDaily:0.##},PnlCum={p.PnLCum:0.##},PnlDailyHold={p.PnLDailyHold:0.##},PnlCumHold={p.PnLCumHold:0.##}");
            }
        }
    }

    public enum StrategyTypeEnum { MeanReversion, Converging }

    public class BacktestHelper
    {
        public static List<PnlEntity> ComputeLongShortPnl(List<PriceSignalEntity> signals, double notional, double signalIn, double signalOut, object meanReversion, bool v)
        {
            throw new NotImplementedException();
        }
    }
}
