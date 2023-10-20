using ProjectX.Core.Strategy;

namespace ProjectX.Core.Tests.Services
{
    public enum StrategyTypeEnum { MeanReversion, Momentum }
    public class BacktestHelper
    {
        public static IEnumerable<PnlEntity> ComputeLongShortPnl(
            IEnumerable<PriceSignalEntity> inputSignals,
            double notional, // initial invest capital
            double signalIn,
            double signalOut,
            StrategyTypeEnum strategyType,
            bool isReinvest)
        {
            ActivePosition activePosition = ActivePosition.INACTIVE;
            var signals = new List<PriceSignalEntity>(inputSignals);
            var candidateSignal = signals.First();
            var pnlEntities = new List<PnlEntity>() { PnlEntityFactory.Build(candidateSignal.Date, candidateSignal.Ticker, candidateSignal.Price, candidateSignal.Signal) };

            double pnlCum = 0.0;
            int totalNumTrades = 0;
            for (int i = 1; i < signals.Count; i++)
            {
                bool exitingPosition = false;

                var prev = signals[i - 1];
                var current = signals[i];

                double pnlDaily = 0.0;
                double pnlPerTrade = 0.0;
                double prevSignal = strategyType == StrategyTypeEnum.Momentum ? (double)-prev.Signal : (double)prev.Signal;
                double prevPnlCum = pnlEntities[i - 1].PnLCum;

                if (activePosition.IsActive)
                {
                    if (activePosition.IsLongPosition())
                    {
                        // long position, compute daily PnL:
                        pnlDaily = activePosition.Shares * (double)(current.Price - prev.Price);
                        pnlCum += pnlDaily;

                        // Exit Long Position:
                        if (prevSignal > -signalOut)
                        {
                            pnlPerTrade = activePosition.Shares * ((double)current.Price - activePosition.PriceIn);
                            exitingPosition = true;
                            totalNumTrades++;
                        }
                    }
                    else if (activePosition.IsShortPosition())
                    {
                        // in short position, compute daily PnL
                        pnlDaily = -activePosition.Shares * (double)(current.Price - prev.Price);
                        pnlCum += pnlDaily;

                        // exit short position
                        if (prevSignal < signalOut)
                        {
                            pnlPerTrade = -activePosition.Shares * ((double)current.Price - activePosition.PriceIn);
                            exitingPosition = true;
                            totalNumTrades++;
                        }
                    }
                }
                else
                {
                    bool testSignal(double prevSignal_, double signalIn_, out PnlTradeType tradeType)
                    {
                        // Enter Long Position:
                        if (prevSignal_ < -signalIn_)
                        {
                            tradeType = PnlTradeType.POSITION_LONG;
                            return true;
                        }

                        // enter short position
                        if (prevSignal_ > signalIn_)
                        {
                            tradeType = PnlTradeType.POSITION_SHORT;
                            return true;
                        }

                        // no signals breached 
                        tradeType = PnlTradeType.POSITION_NONE;
                        return false;
                    }

                    PnlTradeType positionType = PnlTradeType.POSITION_NONE;
                    if (testSignal(prevSignal, signalIn, out positionType))
                    {
                        var shares = isReinvest ? (notional + prevPnlCum) / (double)current.Price : notional / (double)current.Price;
                        activePosition = EnterPosition(positionType, current.Date, (double)current.Price, shares);
                        totalNumTrades++;
                    }
                }

                // compute pnl for holding position
                var initialPrice = signals.First().Price;
                double pnlDailyHold = notional * (double)((current.Price - prev.Price) / initialPrice);
                double pnlCumHold = notional * (double)((current.Price - initialPrice) / initialPrice);

                pnlEntities.Add(PnlEntityFactory.Build(current.Date, current.Ticker, (double)current.Price, (double)current.Signal, pnlCum, pnlDaily, pnlPerTrade, pnlDailyHold, pnlCumHold, totalNumTrades, activePosition));

                if (exitingPosition) { ExitPosition(ref activePosition); }
            }

            return pnlEntities;

            void ExitPosition(ref ActivePosition position) => position = ActivePosition.INACTIVE;
            ActivePosition EnterPosition(PnlTradeType tradeType, DateTime dateIn, double priceIn, double shares) => new ActivePosition(tradeType, dateIn, priceIn, shares);
        }
    }
}
