using MatthiWare.FinancialModelingPrep.Model;
using ProjectX.Core.Services;
using ProjectX.Core.Strategy;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Documents;
using System.Windows.Shapes;

namespace Shell.Screens.TradingSignals;
public partial class SingleViewModel
{   
    class YearlyPnLTableBuilder : TableBuilder
    {
        protected override DataColumn[] Headers => new[]
        {
            new DataColumn("Ticker",typeof(string)),
            new DataColumn("Period",typeof(string)),
            new DataColumn("Num",typeof(string)),
            new DataColumn("PnL",typeof(string)),
            new DataColumn("Sharpe",typeof(string)),
            new DataColumn("PnLHold",typeof(string)),
            new DataColumn("SharpeHold",typeof(string)),
        };

        public override DataTable Build()
        {
            return _dt;         
        }
        public void SetRows(IEnumerable<YearlyStrategyPnl> yearlyPnls)
        {
            FillRows(_dt, yearlyPnls);
        }

        public void SetRows(IEnumerable<(string ticker, string period, string numTrades, string pnl, string sharpe, string pnlHold, string sharpehold)> yearlyPnL)
        {
            FillRows(_dt, yearlyPnL);
        }        

        static void FillRows(DataTable table, IEnumerable<YearlyStrategyPnl> rows)
        {
            foreach (var row in rows)
            {
                table.Rows.Add(row.ticker, row.year, row.numTrades, row.pnl, row.sharpe, row.pnlHold, row.sharpeHold);
            }
        }

        static void FillRows(DataTable table, IEnumerable<(string ticker, string period, string numTrades, string pnl, string sharpe, string pnlHold, string sharpehold)> rows)
        {
            foreach (var row in rows)
            {
                table.Rows.Add(row.ticker, row.period, row.numTrades, row.pnl, row.sharpe, row.pnlHold, row.sharpehold);
            }
        }   
    }

    class PnlRankingTableBuilder : TableBuilder
    {
        public PnlRankingTableBuilder()
        {           
        }
        public PnlRankingTableBuilder(List<(string ticker, int bar, double zin, double zout, int numTrades, double pnlCum, double sharpe)> list)
        {
            FillRows(_dt, list);
        }

        protected override DataColumn[] Headers => new[]
        {
             new DataColumn("Ticker", typeof(string)),
             new DataColumn("Window", typeof(int)),
             new DataColumn("SignalIn", typeof(double)),
             new DataColumn("SignalOut", typeof(double)),
             new DataColumn("Num", typeof(int)),
             new DataColumn("PnL", typeof(double)),
             new DataColumn("Sharpe", typeof(double)),
        };

        public void SetRows(IEnumerable<(string ticker, int bar, double zin, double zout, int numTrades, double pnlCum, double sharpe)> data)
        {
            FillRows(_dt, data);
        }

        public override DataTable Build()
        {
            _dt.DefaultView.Sort = "PnL DESC";
            return _dt.DefaultView.ToTable();
        }

        static void FillRows(DataTable table, IEnumerable<(string ticker, int bar, double zin, double zout, int numTrades, double pnlCum, double sharpe)> rows)
        {
            foreach (var row in rows)
            {
                table.Rows.Add(row.ticker, row.bar, row.zin, row.zout, row.numTrades, row.pnlCum, row.sharpe);
            }
        }
    }

    abstract class TableBuilder
    {
        protected readonly DataTable _dt = new DataTable();

        protected abstract DataColumn[] Headers { get; }

        public TableBuilder()
        {
            _dt.Columns.AddRange(Headers);
        }
        public abstract DataTable Build();
    }
}
