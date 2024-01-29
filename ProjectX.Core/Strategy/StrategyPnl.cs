using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ProjectX.Core.Strategy
{
    public record MatrixStrategyPnl(string ticker, int movingWindow, double zin, double zout, int numTrades, double pnlCum, double sharpe)
    {
        public override string ToString() =>
            $"ticker={ticker} movingWindow={movingWindow} zin={zin} zout={zout} numTrades={numTrades} pnlCum={pnlCum} sharpe={sharpe}";
    }
    public record YearlyStrategyPnl(string ticker, string year, int numTrades, double pnl, double sharpe, double pnlHold, double sharpeHold);

    public record StrategyPnl(DateTime Date, string Ticker, double Price, double Signal, double PnLCum, double PnLDaily, double PnlPerTrade, double PnLDailyHold, double PnLCumHold, PositionStatus TradeType, DateTime? DateIn, double? PriceIn, int NumTrades)
    {
        public override string ToString()
        {
            return $"Ticker={Ticker},Date={Date.ToShortDateString()},Price={Price},Signal={Signal},Type={TradeType},NumTrades={NumTrades},DateIn={DateIn},PriceIn={PriceIn},PnlPerTrade={PnlPerTrade},PnlDaily={PnLDaily},PnlCum={PnLCum},PnlDailyHold={PnLDailyHold},PnlCumHold={PnLCumHold}";
        }
    }
    public record StrategyDrawdown(DateTime date, double pnl, double drawdown, double drawup, double pnlHold, double drawdownHold, double drawupHold);
    public record StrategyResults(IEnumerable<StrategyPnl> StrategyPnls, IEnumerable<YearlyStrategyPnl> YearlyStrategyPnls, IEnumerable<StrategyDrawdown> StrategyDrawdowns);
}
