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
                table.Rows.Add(row.ticker, row.year, row.numTrades, FormatPnl(row.pnl), FormatSharpe(row.sharpe), FormatPnl(row.pnlHold), FormatSharpe(row.sharpeHold)); ;
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
            _dt.DefaultView.Sort = "Date ASC";
            return _dt.DefaultView.ToTable();            
        }

        public void SetRows(IEnumerable<StrategyPnl> pnls)
        {
            FillRows(_dt, pnls);
        }

        static void FillRows(DataTable table, IEnumerable<StrategyPnl> rows)
        {
            foreach (var row in rows)
            {
                var positionState = Position(row.TradeType);
                var dateIn = row.DateIn.HasValue ? row.DateIn.Value.ToString("ddMMyy") : string.Empty;
                table.Rows.Add(row.Date.ToString("ddMMyy"), row.Ticker, row.Price, FormatSharpe(row.Signal), FormatPnl(row.PnLCum), FormatPnl(row.PnLDaily), FormatPnl(row.PnlPerTrade), FormatPnl(row.PnLDaily), FormatPnl(row.PnLCum), positionState, dateIn, row.PriceIn, row.NumTrades);
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
             new DataColumn("Window", typeof(string)),
             new DataColumn("SignalIn", typeof(string)),
             new DataColumn("SignalOut", typeof(string)),
             new DataColumn("Num", typeof(string)),
             new DataColumn("PnL", typeof(string)),
             new DataColumn("Sharpe", typeof(string)),
        };

        public void SetRows(IEnumerable<MatrixStrategyPnl> data)
        {
            FillRows(_dt, data);
        }

        public override DataTable Build()
        {
            var sorted = _dt.AsEnumerable()
                            .OrderByDescending(row => Convert.ToDouble(row.Field<string>("PnL").Replace("$", "")))
                            .AsDataView();

            return sorted.ToTable();
        }

        static void FillRows(DataTable table, IEnumerable<MatrixStrategyPnl> rows)
        {
            foreach (var row in rows)
            {
                table.Rows.Add(row.ticker, row.movingWindow, row.zin, row.zout, row.numTrades, FormatPnl(row.pnlCum), FormatSharpe(row.sharpe));
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

        protected static string FormatSharpe(double sharpe) => sharpe.ToString("N3");
        protected static string FormatPnl(double pnl) => "$" + pnl.ToString("N0");
    }
}
