using ShellX.Tables;

namespace ShellX.Fx;

public class FxPositionsTableBuilder : TableBuilderBase
{
    private decimal _total;

    protected override string[] Headers => ["CcyPair", "Notional", "PnL", "Breakdown"];

    public void SetRows(Dictionary<string, (int netQuantity, int totalTrades, decimal PnL, string debug)> positions)
    {
        foreach (var (ccyPair, position) in positions)
        {
            // The total adds the rounded figures shown on screen, as the WPF grid did.
            var shown = Math.Round(position.PnL, 0, MidpointRounding.AwayFromZero);
            _total += shown;
            _rows.Add([ccyPair, position.netQuantity.ToString(), Format(shown), position.debug]);
        }
    }

    public override TableData Build() => ToTable(_rows.Append(["total", string.Empty, Format(_total), string.Empty]));

    private static string Format(decimal pnl) => pnl.ToString("#,#0");
}
