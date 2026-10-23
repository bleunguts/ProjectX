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
            _rows.Add([maturity.ToString(), greeks.price.ToString(), greeks.delta.ToString(), greeks.gamma.ToString(), greeks.theta.ToString(), greeks.rho.ToString(), greeks.vega.ToString()]);
        }
    }
}
