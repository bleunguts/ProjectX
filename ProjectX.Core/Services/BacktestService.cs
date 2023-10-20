using ProjectX.Core.Strategy;

namespace ProjectX.Core.Services
{
    public class BacktestService
    {
        public IEnumerable<PnlEntity> ComputeLongShortPnl(
            IEnumerable<PriceSignal> inputSignals,
            double notional, // initial invest capital
            double signalIn,
            double signalOut,
            TradingStrategy strategy)
        {
            ActivePosition currentPosition = ActivePosition.INACTIVE;
            var signals = new List<PriceSignal>(inputSignals);
            var candidateSignal = signals.First();
            var pnlEntities = new List<PnlEntity>() { PnlEntityFactory.NewPnlEntity(candidateSignal.Date, candidateSignal.Ticker, candidateSignal.Price, candidateSignal.Signal) };

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
                double prevPnlCum = pnlEntities[i - 1].PnLCum;

                switch (currentPosition)
                {
                    case InactivePosition position:                        
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
                            EnterPosition(ref currentPosition, positionStatus, current.Date, (double)current.Price, shares);
                            totalNumTrades++;
                        }                        
                        break;
                    case LiveActivePosition livePosition:
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

                pnlEntities.Add(PnlEntityFactory.NewPnlEntity(current.Date, current.Ticker, (double)current.Price, (double)current.Signal, pnlCum, pnlDaily, pnlPerTrade, pnlDailyHold, pnlCumHold, totalNumTrades, currentPosition));

                if (exitingPosition) { ExitPosition(ref currentPosition); }
            }

            return pnlEntities;

            void ExitPosition(ref ActivePosition position) => position = ActivePosition.INACTIVE;
            void EnterPosition(ref ActivePosition position, PositionStatus tradeType, DateTime dateIn, double priceIn, double shares) => position = new LiveActivePosition(tradeType, dateIn, priceIn, shares);
        }
    }
}
