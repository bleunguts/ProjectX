using ProjectX.Core.Requests;
using ShellX.Tables;

namespace ShellX.OptionsPricing;

public class OptionResultsTableBuilder : TableBuilderBase
{
    protected override string[] Headers => ["Maturity", "Price", "Delta", "Gamma", "Theta", "Rho", "Vega"];

    public override TableData Build() => ToTable(_rows);

    public void SetRows(IEnumerable<MaturityAndOptionGreeksResultPair> results)
    {
        foreach (var (maturity, greeks) in results)
        {
            _rows.Add([Format(maturity), Format(greeks.price), Format(greeks.delta), Format(greeks.gamma), Format(greeks.theta), Format(greeks.rho), Format(greeks.vega)]);
        }
    }

    // Six significant digits keeps the columns narrow; the full doubles are noise on screen.
    private static string Format(double value) => value.ToString("G6");
}
