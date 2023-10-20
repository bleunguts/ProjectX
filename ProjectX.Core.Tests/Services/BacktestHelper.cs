using ProjectX.Core.Strategy;

namespace ProjectX.Core.Tests.Services
{
    public enum StrategyTypeEnum { MeanReversion, Converging }
    public class BacktestHelper
    {
        public static List<PnlEntity> ComputeLongShortPnl(List<PriceSignalEntity> signals, double notional, double signalIn, double signalOut, object meanReversion, bool v)
        {
            throw new NotImplementedException();
        }
    }
}
