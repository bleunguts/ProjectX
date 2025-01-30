using ProjectX.Core;
using ProjectX.Core.Strategy;
using ProjectX.MachineLearning;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shell.Screens.MachineLearning;

public class StockTableBuilder : TableBuilderBase
{
    protected override DataColumn[] Headers => [
        new DataColumn("Date", typeof(string)),
        new DataColumn("Open", typeof(double)),
        new DataColumn("High", typeof(double)),
        new DataColumn("Low", typeof(double)),
        new DataColumn("Close", typeof(double)),
        new DataColumn("Expected", typeof(string)),
        new DataColumn("Predicted", typeof(string)),
    ];

    public override DataTable Build()
    {
        _dt.DefaultView.Sort = "Date ASC";        
        return _dt.DefaultView.ToTable();
    }

    internal void SetRows(IEnumerable<MarketPrice> prices)
    {
        FillRows(_dt, prices);
    }

    static void FillRows(DataTable table, IEnumerable<MarketPrice> rows)
    {
        foreach (var row in rows)
        {            
            table.Rows.Add(row.Date.ToString("ddMMyy"), row.Open, row.High, row.Low, row.Close, StockPriceTrendDirection.Unset, StockPriceTrendDirection.Unset);
        }
    }
}
