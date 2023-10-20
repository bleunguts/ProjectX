using ProjectX.Core.Services;
using ProjectX.Core.Strategy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectX.Core.Tests.Services
{
    public class BacktestServiceTest
    {
        const double notional = 10_000.0;
        const double signalIn = 2;
        const double signalOut = 0.0;
        const string ticker = "IBM";
        readonly DateTime startDate = DateTime.Now;

        private readonly BacktestService _backtestService = new();

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
            var strategy = new TradingStrategy(TradingStrategyType.MeanReversion, false);
            var signals = new List<PriceSignal>
            {
                builder.NewSignal(-1.5),
                builder.NewSignal(-2.1),
                builder.NewSignal(-3.1), // enters long trade (prevSignal < -2) 
                builder.NewSignal(1.1),
                builder.NewSignal(-4.1)  // exit long trade (prevSignal > 0)
            };

            var pnlEntities = _backtestService.ComputeLongShortPnl(signals, notional, signalIn, signalOut, strategy).ToList();
            pnlEntities.Print();
            (int enterTradeIndex, int exitTradeIndex) = pnlEntities.DeconstructTradeTimeline(PositionStatus.POSITION_LONG);
            var enterTrade = pnlEntities[enterTradeIndex];
            Assert.That(enterTrade.Date, Is.EqualTo(signals[enterTradeIndex].Date));
            Assert.That(enterTrade.TradeType, Is.EqualTo(PositionStatus.POSITION_LONG));
            Assert.That(enterTrade.PriceIn, Is.EqualTo(signals[enterTradeIndex].Price));
            Assert.That(enterTrade.NumTrades, Is.EqualTo(1));
            Assert.That(enterTrade.PnlPerTrade, Is.EqualTo(0));

            var exitTrade = pnlEntities[exitTradeIndex];
            Assert.That(exitTrade.Date, Is.EqualTo(signals[exitTradeIndex].Date));
            Assert.That(exitTrade.TradeType, Is.EqualTo(PositionStatus.POSITION_LONG));
            Assert.That(exitTrade.PriceIn, Is.EqualTo(signals[enterTradeIndex].Price));
            Assert.That(exitTrade.NumTrades, Is.EqualTo(2));
            Assert.That(exitTrade.PnlPerTrade, Is.GreaterThan(0).Or.LessThan(0));

            pnlEntities.PrintStrategyFor(enterTradeIndex, exitTradeIndex);
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
            var signals = new List<PriceSignal>
            {
                builder.NewSignal(-1.5),
                builder.NewSignal(3.1),
                builder.NewSignal(0.1), // enters short trade (prevSignal > 2)
                builder.NewSignal(-10.1),
                builder.NewSignal(0.1)   // exit short trade (prevSignal < 0) 
            };

            var pnlEntities = _backtestService.ComputeLongShortPnl(signals, notional, signalIn, signalOut, new TradingStrategy(TradingStrategyType.MeanReversion, false)).ToList();
            pnlEntities.Print();

            (int enterTradeIndex, int exitTradeIndex) = pnlEntities.DeconstructTradeTimeline(PositionStatus.POSITION_SHORT);
            var enterTrade = pnlEntities[enterTradeIndex];
            Assert.That(enterTrade.Date, Is.EqualTo(signals[enterTradeIndex].Date));
            Assert.That(enterTrade.TradeType, Is.EqualTo(PositionStatus.POSITION_SHORT));
            Assert.That(enterTrade.PriceIn, Is.EqualTo(signals[enterTradeIndex].Price));
            Assert.That(enterTrade.NumTrades, Is.EqualTo(1));
            Assert.That(enterTrade.PnlPerTrade, Is.EqualTo(0));

            var exitTrade = pnlEntities[exitTradeIndex];
            Assert.That(exitTrade.Date, Is.EqualTo(signals[exitTradeIndex].Date));
            Assert.That(exitTrade.TradeType, Is.EqualTo(PositionStatus.POSITION_SHORT));
            Assert.That(exitTrade.PriceIn, Is.EqualTo(signals[enterTradeIndex].Price));
            Assert.That(exitTrade.NumTrades, Is.EqualTo(2));
            Assert.That(exitTrade.PnlPerTrade, Is.GreaterThan(0).Or.LessThan(0));

            pnlEntities.PrintStrategyFor(enterTradeIndex, exitTradeIndex);
        }

        [Test]
        public void WhenSignalSwingsViciouslyShouldNotEnterPositionIfAlreadyTrading()
        {
            var signalIn = 2.0;
            var signalOut = -3.0;
            var random = new Random();                        
            var builder = new SignalBuilder(ticker, startDate);

            var signals = new List<PriceSignal>
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

            var pnlEntities = _backtestService.ComputeLongShortPnl(signals, notional, signalIn, signalOut, new TradingStrategy(TradingStrategyType.MeanReversion, false)).ToList();                     
            pnlEntities.PrintStrategyFor(PositionStatus.POSITION_SHORT);            
        }                 
    }    
}
