using MatthiWare.FinancialModelingPrep.Model;
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
                table.Rows.Add(row.ticker, row.year, row.numTrades, Math.Round(row.pnl,2), Math.Round(row.sharpe,3), Math.Round(row.pnlHold,2), Math.Round(row.sharpeHold,3));
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
    class PnLTableBuilder : TableBuilder
    {
        protected override DataColumn[] Headers => new[]
        {
            new DataColumn("Date",typeof(string)),
            new DataColumn("Ticker",typeof(string)),
            new DataColumn("Price",typeof(string)),
            new DataColumn("Signal",typeof(string)),
            new DataColumn("PnLCum",typeof(string)),
            new DataColumn("PnLDaily",typeof(string)),
            new DataColumn("PnLPerTrade",typeof(string)),
            new DataColumn("PnLDailyHold",typeof(string)),
            new DataColumn("PnLCumHold",typeof(string)),
            new DataColumn("Position",typeof(string)),
            new DataColumn("DateIn",typeof(string)),
            new DataColumn("PriceIn",typeof(string)),
            new DataColumn("Num",typeof(string)),
        };

        public override DataTable Build()
        {
            return _dt;
        }

        public void SetRows(List<StrategyPnl> pnls)
        {
            FillRows(_dt, pnls);
        }

        static void FillRows(DataTable table, IEnumerable<StrategyPnl> rows)
        {
            foreach (var row in rows)
            {
                var positionState = Position(row.TradeType);
                var dateIn = row.DateIn.HasValue ? row.DateIn.Value.ToString("ddMMyy") : string.Empty;
                table.Rows.Add(row.Date.ToString("ddMMyy"), row.Ticker, row.Price, Math.Round(row.Signal,3), Math.Round(row.PnLCum,2), Math.Round(row.PnLDaily, 2), Math.Round(row.PnlPerTrade, 2), Math.Round(row.PnLDaily, 2), Math.Round(row.PnLCum, 2), positionState, dateIn, row.PriceIn, row.NumTrades);
            }

            static string Position(PositionStatus positionStatus)
            {
                switch(positionStatus)
                {
                    case PositionStatus.POSITION_LONG:
                        return "LONG";
                    case PositionStatus.POSITION_SHORT:
                        return "SHORT";
                    default:
                        return string.Empty;
                }
            }
        }        
    }
    class PnlRankingTableBuilder : TableBuilder
    {
        public PnlRankingTableBuilder()
        {           
        }
        public PnlRankingTableBuilder(List<MatrixStrategyPnl> list)
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

        public void SetRows(IEnumerable<MatrixStrategyPnl> data)
        {
            FillRows(_dt, data);
        }

        public override DataTable Build()
        {
            _dt.DefaultView.Sort = "PnL DESC";
            return _dt.DefaultView.ToTable();
        }

        static void FillRows(DataTable table, IEnumerable<MatrixStrategyPnl> rows)
        {
            foreach (var row in rows)
            {
                table.Rows.Add(row.ticker, row.movingWindow, row.zin, row.zout, row.numTrades, Math.Round(row.pnlCum, 2), Math.Round(row.sharpe, 3));
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
