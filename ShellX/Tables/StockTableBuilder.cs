using ProjectX.Core;
using ProjectX.MachineLearning;

namespace ShellX.Tables;

public class StockTableBuilder : TableBuilderBase
{
    protected override string[] Headers => ["Date", "Open", "High", "Low", "Close", "Expected", "Predicted"];

    public override TableData Build() => ToTable(_rows.OrderBy(r => r[0], StringComparer.CurrentCulture));

    public void SetRows(IEnumerable<MarketPrice> prices)
    {
        foreach (var row in prices)
        {
            _rows.Add([row.Date.ToString("ddMMyy"), row.Open.ToString(), row.High.ToString(), row.Low.ToString(), row.Close.ToString(),
                StockPriceTrendDirection.Unset.ToString(), StockPriceTrendDirection.Unset.ToString()]);
        }
    }
}
