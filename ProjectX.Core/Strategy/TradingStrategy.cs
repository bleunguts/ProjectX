using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectX.Core.Strategy
{
    public enum TradingStrategyType { MeanReversion, Momentum }
    public class TradingStrategy
    {
        public TradingStrategyType StrategyType { get; init; }
        public bool IsReinvest { get; init; }
        public bool IsMomentum() => StrategyType == TradingStrategyType.Momentum;
        public TradingStrategy(TradingStrategyType strategyType, bool shouldReinvest)
        {
            StrategyType = strategyType;
            IsReinvest = shouldReinvest;
        }
    }
}
