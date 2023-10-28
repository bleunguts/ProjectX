using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json;
using ProjectX.Core.Services;
using ProjectX.Core.Strategy;
using ProjectX.MarketData;
using ProjectX.MarketData.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
        strategyPnls.PrintStrategyFor(enterTradeIndex, exitTradeIndex);

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
        // ONLY when the signal triggers an exit position will you get Pnl
        Assert.That(exitTrade.PnlPerTrade, Is.GreaterThan(0).Or.LessThan(0));          
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
    public void WhenComputingSharpeRatioForStrategyAndHolding()
    {
        StrategyPnl[] pnls = new[]
        {
            GiveMeAPnlEntitiy(NextDateFor(2020), 10, 100.0, 0, 100.0, 0),
            GiveMeAPnlEntitiy(NextDateFor(2020), 10, 110.0, 0, 110.0, 0),
            GiveMeAPnlEntitiy(NextDateFor(2020), 10, 120.0, 0, 120.0, 0)
        };
        (double strategyResult, double holdResult) = _backtestService.GetSharpe(pnls);

        Assert.That(strategyResult, Is.GreaterThan(0).Or.LessThan(0));
        Assert.That(holdResult, Is.GreaterThan(0).Or.LessThan(0));
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

        var result = _backtestService.GetYearlyPnl(pnls).ToList();
        foreach (var row in result)
        {
            Console.WriteLine($"numTrades={row.numTrades},ticker={row.ticker},year={row.year},pnl={row.pnl},pnlHold={row.pnlHold},sharpe={row.sharpe},sharpeHold={row.sharpeHold}");
        }   
        Assert.That(result, Has.Count.EqualTo(3));
        var years = new List<YearlyStrategyPnl>();
        years.Add(result.First(r => r.year == "2020"));
        years.Add(result.First(r => r.year == "2021"));
        var total = result.First(r => r.year == "Total");
        years.Add(total);

        foreach (var year in years)
        {
            Assert.That(year, Is.Not.Null);
            Assert.That(year.pnl, Is.GreaterThan(0).Or.LessThan(0));
            Assert.That(year.sharpe, Is.GreaterThan(0).Or.LessThan(0));
            Assert.That(year.pnlHold, Is.GreaterThan(0).Or.LessThan(0));
            Assert.That(year.sharpeHold, Is.GreaterThan(0).Or.LessThan(0));
        }   
    }

    [Test]
    public void WhenGettingMatrixStrategyPnl()
    {
        var builder = new SignalBuilder(ticker);
        var strategy = new TradingStrategy(TradingStrategyType.MeanReversion, false);
        var signals = GetRandomPrices(1000);
        var pnls = _backtestService.ComputeLongShortPnlFull(signals, 10_000, strategy).ToList();
        
        Assert.That(pnls, Has.Count.GreaterThan(100));        
        foreach(var pnl in pnls)
        {
            Console.WriteLine(pnl); 
            Assert.That(pnl.movingWindow, Is.GreaterThan(0));
            Assert.That(pnl.zin, Is.GreaterThan(0));
            Assert.That(pnl.zout, Is.GreaterThanOrEqualTo(0));
            Assert.That(pnl.sharpe, Is.GreaterThan(0).Or.LessThan(0), pnl.ToString());
            Assert.That(pnl.numTrades, Is.GreaterThan(0));
            Assert.That(pnl.pnlCum, Is.GreaterThan(0).Or.LessThan(0));
            Assert.That(pnl.ticker, Is.EqualTo("IBM"));
        }
    }

    [Test]     
    public void WhenGettingDrawdownsFromPnls()
    {
        var pnls = new List<StrategyPnl>()
        {
            GiveMeAPnlEntitiy(NextDateFor(2020), 0, 0, 100, 0, 100),
            GiveMeAPnlEntitiy(NextDateFor(2020), 0, 0, 500, 0, 500),
            GiveMeAPnlEntitiy(NextDateFor(2020), 0, 0, 1000, 0, 1000),
        };

        var drawdownResult = _backtestService.CalculateDrawdown(pnls, 10_000);
        Assert.That(drawdownResult, Is.Not.Null);
    }

    static MarketPrice[] GetRandomPrices(int howmany = 100)
    {
        var current = DateTime.Now.AddDays(-howmany * 2);
        List<MarketPrice> marketPrices = new List<MarketPrice>();   
        for (int i = 0; i < howmany; i++)
        {
            marketPrices.Add(new MarketPrice
            {
                Open = RandomPrice(15, 17),
                Close = RandomPrice(15, 17),
                High = RandomPrice(17, 18),
                Low = RandomPrice(14, 15),  
                Ticker = "IBM",
                Volume = 1000,
                Date = current
            });
            current = current.AddDays(1);
        }

        return marketPrices.ToArray();
    }

    static decimal RandomPrice(int from = 1, int end = 99)
    {
        Random random = new Random(Guid.NewGuid().GetHashCode());
        return (decimal)( random.Next(from, end) + random.NextDouble() );
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

