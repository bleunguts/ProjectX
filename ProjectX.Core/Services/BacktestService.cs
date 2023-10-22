using ProjectX.Core.Strategy;
using System.ComponentModel.Composition;
using System.Xml;

namespace ProjectX.Core.Services
{
    public interface IBacktestService
    {
        IEnumerable<StrategyPnl> ComputeLongShortPnl(IEnumerable<PriceSignal> inputSignals, double notional, double signalIn, double signalOut, TradingStrategy strategy);
        IEnumerable<YearlyStrategyPnl> GetYearlyPnl(List<StrategyPnl> pnls);
    }

    [Export(typeof(IBacktestService)), PartCreationPolicy(CreationPolicy.Shared)]
    public class BacktestService : IBacktestService
    {
        private static List<StrategyPnl> CreatePnlsWith(PriceSignal candidateSignal) => new() { StrategyPnlFactory.NewPnl(candidateSignal.Date, candidateSignal.Ticker, candidateSignal.Price, candidateSignal.Signal) };

        public IEnumerable<StrategyPnl> ComputeLongShortPnl(
            IEnumerable<PriceSignal> inputSignals,
            double notional, // initial invest capital
            double signalIn,
            double signalOut,
            TradingStrategy strategy)
        {
            Position currentPosition = new();
            var signals = new List<PriceSignal>(inputSignals);
            var candidateSignal = signals.First();
            List<StrategyPnl> pnls = CreatePnlsWith(candidateSignal);

            double pnlCum = 0.0;
            int totalNumTrades = 0;
            // walkthrough timeline of signals from beginning to end 
            for (int i = 1; i < signals.Count; i++)
            {
                bool exitingPosition = false;

                var prev = signals[i - 1];
                var current = signals[i];

                double pnlDaily = 0.0;
                double pnlPerTrade = 0.0;
                double prevSignal = strategy.IsMomentum() ? (double)-prev.Signal : (double)prev.Signal;
                double prevPnlCum = pnls[i - 1].PnLCum;

                switch (currentPosition.PositionState)
                {
                    case InactivePositionState _:
                        bool testSignal(double prevSignal_, double signalIn_, out PositionStatus positionStatus)
                        {
                            // Enter Long Position:
                            if (prevSignal_ < -signalIn_)
                            {
                                positionStatus = PositionStatus.POSITION_LONG;
                                return true;
                            }

                            // enter short position
                            if (prevSignal_ > signalIn_)
                            {
                                positionStatus = PositionStatus.POSITION_SHORT;
                                return true;
                            }

                            // no signals breached 
                            positionStatus = PositionStatus.POSITION_NONE;
                            return false;
                        }

                        PositionStatus positionStatus = PositionStatus.POSITION_NONE;
                        if (testSignal(prevSignal, signalIn, out positionStatus))
                        {
                            var shares = strategy.IsReinvest ? (notional + prevPnlCum) / (double)current.Price : notional / (double)current.Price;
                            currentPosition.EnterPosition(positionStatus, current.Date, (double)current.Price, shares);
                            totalNumTrades++;
                        }
                        break;
                    case LiveActivePositionState _:
                        var livePosition = currentPosition;
                        if (livePosition.IsLongPosition())
                        {
                            // long position, compute daily PnL:
                            pnlDaily = livePosition.Shares * (double)(current.Price - prev.Price);
                            pnlCum += pnlDaily;

                            // Exit Long Position:
                            if (prevSignal > -signalOut)
                            {
                                pnlPerTrade = livePosition.Shares * ((double)current.Price - livePosition.PriceIn);
                                exitingPosition = true;
                                totalNumTrades++;
                            }
                        }
                        else if (livePosition.IsShortPosition())
                        {
                            // in short position, compute daily PnL
                            pnlDaily = -livePosition.Shares * (double)(current.Price - prev.Price);
                            pnlCum += pnlDaily;

                            // exit short position
                            if (prevSignal < signalOut)
                            {
                                pnlPerTrade = -livePosition.Shares * ((double)current.Price - livePosition.PriceIn);
                                exitingPosition = true;
                                totalNumTrades++;
                            }
                        }
                        break;
                    default:
                        throw new NotImplementedException($"{currentPosition.GetType()} not recognised.");
                }

                // compute pnl for holding position
                var initialPrice = signals.First().Price;
                double pnlDailyHold = notional * (double)((current.Price - prev.Price) / initialPrice);
                double pnlCumHold = notional * (double)((current.Price - initialPrice) / initialPrice);

                pnls.Add(StrategyPnlFactory.NewPnl(current.Date, current.Ticker, (double)current.Price, (double)current.Signal, pnlCum, pnlDaily, pnlPerTrade, pnlDailyHold, pnlCumHold, totalNumTrades, currentPosition));
                // exiting position has to be called at the end after adding pnlresults
                if (exitingPosition) { currentPosition.ExitPosition(); }
            }

            return pnls;
        }

        /// <summary>
        /// Sharpe ratio is a risk measure used to determine return of the strategy above risk free rate
        /// It average of daily returns divided by standard deviation of the daily returns        
        /// </summary>     
        public (double strategyResult, double holdResult) GetSharpe(IEnumerable<StrategyPnl> pnl)
        {

            // computes annualized sharpe ratio by taking int account daily P&L from the strategy and from the buying-and-holding of position
            double avg = pnl.Average(x => x.PnLDaily);
            double std = (double)pnl.StdDev(x => (decimal)x.PnLDaily);

            // buying and holding of position
            double avgHolding = pnl.Average(x => x.PnLDailyHold);
            double stdHolding = (double)pnl.StdDev(x => (decimal)x.PnLDailyHold);

            double sp = Math.Round(Math.Sqrt(252.0) * avg / std, 4);
            double spHolding = Math.Round(Math.Sqrt(252.0) * avgHolding / stdHolding, 4);

            // shows how your strategy performs each year 
            return (sp, spHolding);
        }

        public IEnumerable<YearlyStrategyPnl> GetYearlyPnl(List<StrategyPnl> pnls)
        {
            DateTime firstDate = pnls.First().Date;
            DateTime lastDate = pnls.Last().Date;

            DateTime currentDate = new DateTime(firstDate.Year, 1, 1);
            var result = new List<YearlyStrategyPnl>();
            while (currentDate <= lastDate)
            {
                DateTime FIRST_DAY_OF_THAT_YEAR = new DateTime(currentDate.Year, 1, 1);
                DateTime LAST_DAY_OF_THAT_YEAR = new DateTime(currentDate.Year, 12, 31);
                var currentPnls = pnls.Where(pnl => pnl.Date >= FIRST_DAY_OF_THAT_YEAR && pnl.Date <= LAST_DAY_OF_THAT_YEAR).OrderBy(pnl => pnl.Date).ToList();
                if (currentPnls.Count > 0)
                {
                    var first = currentPnls.First();
                    var last = currentPnls.Last();
                    var entitiesWithPnlDaily = currentPnls.Where(pnl => pnl.PnLDaily != 0).OrderBy(pnl => pnl.Date).ToList();
                    if (entitiesWithPnlDaily.Count > 0)
                    {
                        var sharpe = GetSharpe(entitiesWithPnlDaily);
                        int numTrades = last.NumTrades - first.NumTrades;
                        double pnl1 = last.PnLCum - first.PnLCum + first.PnLDaily;
                        double pnl2 = last.PnLCumHold - first.PnLCumHold + first.PnLDailyHold;
                        result.Add(new YearlyStrategyPnl(first.Ticker, currentDate.Year.ToString(), numTrades, Math.Round(pnl1, 0), sharpe.strategyResult, Math.Round(pnl2, 0), sharpe.holdResult));
                    }
                }
                currentDate = currentDate.AddYears(1);
            }
            var sharpeResult = GetSharpe(pnls);
            double sum = Math.Round(pnls.Last().PnLCum, 0);
            double sum1 = Math.Round(pnls.Last().PnLCumHold, 0);
            result.Add(new YearlyStrategyPnl(pnls.First().Ticker, "Total", pnls.Last().NumTrades, sum, sharpeResult.strategyResult, sum1, sharpeResult.holdResult));
            return result;
        }
    }
    public record YearlyStrategyPnl(string ticker, string year, int numTrades, double pnl, double sharpe, double pnlHold, double sharpeHold);
}
