﻿using Moq;
using ProjectX.Core.Services;
using ProjectX.Core.Strategy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ProjectX.Core.Tests.Services;

public class BacktestServiceTest
{
    const double notional = 10_000.0;
    const double signalIn = 2;
    const double signalOut = 0.0;
    const string ticker = "IBM";
    readonly DateTime startDate = DateTime.Now;
    Dictionary<int, int> yearCounter = new Dictionary<int, int>();

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

        var strategyPnls = _backtestService.ComputeLongShortPnl(signals, notional, signalIn, signalOut, strategy).ToList();
        strategyPnls.Print();
        (int enterTradeIndex, int exitTradeIndex) = strategyPnls.DeconstructTradeTimeline(PositionStatus.POSITION_LONG);
        var enterTrade = strategyPnls[enterTradeIndex];
        Assert.That(enterTrade.Date, Is.EqualTo(signals[enterTradeIndex].Date));
        Assert.That(enterTrade.TradeType, Is.EqualTo(PositionStatus.POSITION_LONG));
        Assert.That(enterTrade.PriceIn, Is.EqualTo(signals[enterTradeIndex].Price));
        Assert.That(enterTrade.NumTrades, Is.EqualTo(1));
        Assert.That(enterTrade.PnlPerTrade, Is.EqualTo(0));

        var exitTrade = strategyPnls[exitTradeIndex];
        Assert.That(exitTrade.Date, Is.EqualTo(signals[exitTradeIndex].Date));
        Assert.That(exitTrade.TradeType, Is.EqualTo(PositionStatus.POSITION_LONG));
        Assert.That(exitTrade.PriceIn, Is.EqualTo(signals[enterTradeIndex].Price));
        Assert.That(exitTrade.NumTrades, Is.EqualTo(2));
        Assert.That(exitTrade.PnlPerTrade, Is.GreaterThan(0).Or.LessThan(0));

        strategyPnls.PrintStrategyFor(enterTradeIndex, exitTradeIndex);
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

        var strategyPnls = _backtestService.ComputeLongShortPnl(signals, notional, signalIn, signalOut, new TradingStrategy(TradingStrategyType.MeanReversion, false)).ToList();
        strategyPnls.Print();

        (int enterTradeIndex, int exitTradeIndex) = strategyPnls.DeconstructTradeTimeline(PositionStatus.POSITION_SHORT);
        var enterTrade = strategyPnls[enterTradeIndex];
        Assert.That(enterTrade.Date, Is.EqualTo(signals[enterTradeIndex].Date));
        Assert.That(enterTrade.TradeType, Is.EqualTo(PositionStatus.POSITION_SHORT));
        Assert.That(enterTrade.PriceIn, Is.EqualTo(signals[enterTradeIndex].Price));
        Assert.That(enterTrade.NumTrades, Is.EqualTo(1));
        Assert.That(enterTrade.PnlPerTrade, Is.EqualTo(0));

        var exitTrade = strategyPnls[exitTradeIndex];
        Assert.That(exitTrade.Date, Is.EqualTo(signals[exitTradeIndex].Date));
        Assert.That(exitTrade.TradeType, Is.EqualTo(PositionStatus.POSITION_SHORT));
        Assert.That(exitTrade.PriceIn, Is.EqualTo(signals[enterTradeIndex].Price));
        Assert.That(exitTrade.NumTrades, Is.EqualTo(2));
        Assert.That(exitTrade.PnlPerTrade, Is.GreaterThan(0).Or.LessThan(0));

        strategyPnls.PrintStrategyFor(enterTradeIndex, exitTradeIndex);
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

        var strategyPnls = _backtestService.ComputeLongShortPnl(signals, notional, signalIn, signalOut, new TradingStrategy(TradingStrategyType.MeanReversion, false)).ToList();                     
        strategyPnls.PrintStrategyFor(PositionStatus.POSITION_SHORT);            
    }

    [Test]
    public void WhenGettingYearlyPnl()
    {
        var today = DateTime.Now;
        var pnls = new List<StrategyPnl>
        {
            // Sharpe takes stdev PnlDaily, PnlDailyHold and avgs of them as well
            GiveMeAPnlEntitiy(NextDateFor(2020), 1, 100.0, 0, 0, 0),
            GiveMeAPnlEntitiy(NextDateFor(2020), 1, 110.0, 10, 10, 50),
            GiveMeAPnlEntitiy(NextDateFor(2020), 2, 120.0, 10, 20, 30),
            GiveMeAPnlEntitiy(NextDateFor(2021), 0, 40.0, 10, 90, 80),
            GiveMeAPnlEntitiy(NextDateFor(2021), 1, 130.0, 30, 80, 210),
            GiveMeAPnlEntitiy(NextDateFor(2021), 1, 140.0, 40, 30, 310),
            GiveMeAPnlEntitiy(NextDateFor(2021), 2, 150.0, 50, 190, 50),
        };

        foreach (var row in _backtestService.GetYearlyPnl(pnls))
        {
            Console.WriteLine($"numTrades={row.numTrades},ticker={row.ticker},year={row.year},pnl={row.pnl},pnl2={row.pnl2},sp0={row.sp0},sp1={row.sp1}");
        }   
    }

    static StrategyPnl GiveMeAPnlEntitiy(DateTime date, int numTrades, double pnlDaily, double pnlCum, double pnlDailyHold, double pnlCumHold) 
        => StrategyPnlFactory.NewPnl(date, "TICKER", -1.0, -1.0, pnlCum, pnlDaily, -1.0, pnlDailyHold, pnlCumHold, numTrades, new Position());

    DateTime NextDateFor(int year)
    {
        var date = new DateTime(year, 2, 3);
        int counter;

        if (!yearCounter.TryGetValue(year, out counter))
        {
            yearCounter.Add(year, 0);
        }

        return date.AddDays(counter);
    }
}

