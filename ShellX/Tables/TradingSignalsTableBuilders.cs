using System.Globalization;
using ProjectX.Core.Strategy;

namespace ShellX.Tables;

public abstract class TradingSignalsTableBuilderBase : TableBuilderBase
{
    protected static string FormatSharpe(double sharpe) => sharpe.ToString("N3");
    protected static string FormatPnl(double pnl) => "$" + pnl.ToString("N0");
}

public class YearlyPnLTableBuilder : TradingSignalsTableBuilderBase
{
    protected override string[] Headers => ["Ticker", "Period", "Num", "PnL", "Sharpe", "PnLHold", "SharpeHold"];

    public override TableData Build() => ToTable(_rows);

    public void SetRows(IEnumerable<YearlyStrategyPnl> yearlyPnls)
    {
        foreach (var row in yearlyPnls)
        {
            _rows.Add([row.ticker, row.year, row.numTrades.ToString(), FormatPnl(row.pnl), FormatSharpe(row.sharpe), FormatPnl(row.pnlHold), FormatSharpe(row.sharpeHold)]);
        }
    }

    public void SetRows(IEnumerable<(string ticker, string period, string numTrades, string pnl, string sharpe, string pnlHold, string sharpehold)> yearlyPnL)
    {
        foreach (var row in yearlyPnL)
        {
            _rows.Add([row.ticker, row.period, row.numTrades, row.pnl, row.sharpe, row.pnlHold, row.sharpehold]);
        }
    }
}

public class PnLTableBuilder : TradingSignalsTableBuilderBase
{
    protected override string[] Headers =>
        ["Date", "Ticker", "Price", "Signal", "PnLCum", "PnLDaily", "PnLPerTrade", "PnLDailyHold", "PnLCumHold", "Position", "DateIn", "PriceIn", "Num"];

    public override TableData Build() => ToTable(_rows.OrderBy(r => r[0], StringComparer.CurrentCulture));

    public void SetRows(IEnumerable<StrategyPnl> pnls)
    {
        foreach (var row in pnls)
        {
            var dateIn = row.DateIn.HasValue ? row.DateIn.Value.ToString("ddMMyy") : string.Empty;
            var priceIn = row.PriceIn.HasValue ? row.PriceIn.Value.ToString() : string.Empty;
            _rows.Add([row.Date.ToString("ddMMyy"), row.Ticker, row.Price.ToString(), FormatSharpe(row.Signal), FormatPnl(row.PnLCum), FormatPnl(row.PnLDaily), FormatPnl(row.PnlPerTrade),
                FormatPnl(row.PnLDaily), FormatPnl(row.PnLCum), Position(row.TradeType), dateIn, priceIn, row.NumTrades.ToString()]);
        }

        static string Position(PositionStatus positionStatus) => positionStatus switch
        {
            PositionStatus.POSITION_LONG => "LONG",
            PositionStatus.POSITION_SHORT => "SHORT",
            _ => string.Empty,
        };
    }
}

public class PnlRankingTableBuilder : TradingSignalsTableBuilderBase
{
    protected override string[] Headers => ["Ticker", "Window", "SignalIn", "SignalOut", "Num", "PnL", "Sharpe"];

    public void SetRows(IEnumerable<MatrixStrategyPnl> data)
    {
        foreach (var row in data)
        {
            _rows.Add([row.ticker, row.movingWindow.ToString(), row.zin.ToString(), row.zout.ToString(), row.numTrades.ToString(), FormatPnl(row.pnlCum), FormatSharpe(row.sharpe)]);
        }
    }

    public override TableData Build() => ToTable(_rows.OrderByDescending(r => double.Parse(r[5].Replace("$", ""), NumberStyles.Number, CultureInfo.CurrentCulture)));
}
