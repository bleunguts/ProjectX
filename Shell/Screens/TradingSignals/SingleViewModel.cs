using Caliburn.Micro;
using ProjectX.Core.Strategy;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Xml;

namespace Shell.Screens.TradingSignals;

[Export(typeof(IScreen)), PartCreationPolicy(CreationPolicy.NonShared)]
public partial class SingleViewModel : Screen
{
    private readonly IEventAggregator eventAggregator;

    [ImportingConstructor]
    public SingleViewModel(IEventAggregator eventAggregator)
    {
        this.eventAggregator = eventAggregator;
        DisplayName = "Mean Reversion strategy (Backtesting)";
    }

    private string ticker = "IBM";
    private DateTime fromDate = new(2023, 2, 25);
    private DateTime toDate = new(2023, 2, 25);
    private string signalType = "MovingAverage";
    private string priceType = "Close";
    private int notional = 10_000;
    private DataTable pnlRankingTable = new();
    private DataTable yearlyPnLTable = new();
    private BindableCollection<PnlEntity> pnlTable = new();

    #region Bindable Properties    
    public string Ticker
    {
        get { return ticker; }
        set { ticker = value; NotifyOfPropertyChange(() => Ticker); }
    }    
    public DateTime FromDate
    {
        get { return fromDate; }
        set { fromDate = value; NotifyOfPropertyChange(() => FromDate); }
    }
    public DateTime ToDate
    {
        get { return toDate; }
        set { toDate = value; NotifyOfPropertyChange(() => ToDate); }
    }
    public string SignalType
    {
        get { return signalType; }
        set { signalType = value; NotifyOfPropertyChange(() => SignalType); }
    }
    public string PriceType
    {
        get { return priceType; }
        set { priceType = value; NotifyOfPropertyChange(() => PriceType); }
    }    
    public DataTable YearlyPnLTable
    {
        get { return yearlyPnLTable; }
        set { yearlyPnLTable = value; NotifyOfPropertyChange(() => YearlyPnLTable); }
    }

    public DataTable PnLRankingTable
    {
        get { return pnlRankingTable; }
        set { pnlRankingTable = value; NotifyOfPropertyChange(() => PnLRankingTable); }
    }

    public BindableCollection<PnlEntity> PnLTable
    {
        get { return pnlTable; }
        set { pnlTable = value; NotifyOfPropertyChange(() => PnLTable); }
    }    

    public int Notional
    {
        get { return notional; }
        set { notional = value; NotifyOfPropertyChange(() => Notional); }
    }

    #endregion
    public async Task Compute()
    {
        await Task.Run(() => 
        {
            PnLTable.Clear();
            YearlyPnLTable = new DataTable();
            PnLRankingTable = new DataTable();

            var builder = new PnlRankingTableBuilder();
            builder.SetRows(DummyData.DummyPnLRankingTable);
            PnLRankingTable = builder.Build();

            return Task.CompletedTask;
        });       
    }   

    public async void SelectedCellChanged(object sender, SelectedCellsChangedEventArgs e)
    {
        await Task.Run(() =>
        {
            var selectedCells = e.AddedCells;
            DataRowView row = (DataRowView)selectedCells[selectedCells.Count - 1].Item;
            int movingWindow = Convert.ToInt32(row[1]);
            double signalIn = Convert.ToDouble(row[2]);
            double signalOut = Convert.ToDouble(row[3]);        

            // Fix below
            // var pnls = BacktestHelper.ComputeLongShortPnl(signal, 10_000.0, signalIn, signalOut, SelectedStrategyType, IsReinvest).ToList();            
            PnLTable.Clear();
            PnLTable.AddRange(DummyData.DummyPnlTable);

            // GetYearlyPnl and upate YearlyPnlTable            
            // public static List<(string ticker, string year, int numTrades, double pnl, double sp0, double pnl2, double sp1)> GetYearlyPnl(List<PnlEntity> p)
            // or can write own yearly aggregator based on pnls above 
            var builder = new YearlyPnLTableBuilder();
            builder.SetRows(DummyData.YearlyPnL);            
            YearlyPnLTable = builder.Build();

            // Update IEnumerable<SignalData> SignalDataForSignalCharts
            // Compute Signals for user selected movingWindow 
            // var signal = SignalHelper.GetSignal(data, movingWindow, SelectedSignalType);

            // GetDrawdown
            // Update IEnumerable<Drawdow> DrawdownDataForDrawdownCharts
            //DataTable dt2 = new DataTable();
        });
    }

    static class DummyData
    {
        public static IEnumerable<(string ticker, int bar, double zin, double zout, int numTrades, double pnlCum, double sharpe)> DummyPnLRankingTable => new List<(string ticker, int bar, double zin, double zout, int numTrades, double pnlCum, double sharpe)>()
        {
            ("IBM", 10, 10, 20, 5, 120, 0.9),
            ("IBM", 9, 9, 20, 5, 180, 1.9)
        };

        public static PnlEntity[] DummyPnlTable => new[]
        {
            new PnlEntity("IBM", new DateTime(2023, 1, 1), 2.0, 5, 1000, 120, 1200, 1000, 1000)
        };

        public static IEnumerable<(string ticker, string period, string numTrades, string pnl, string sharpe, string pnlHold, string sharpehold)> YearlyPnL => new List<(string ticker, string period, string numTrades, string pnl, string sharpe, string pnlHold, string sharpehold)>
        {
            ("IBM", "2018", "5", "1200.5", "0.487", "1000.0", "0.9"),
            ("IBM", "2019", "5", "1230.5", "0.687", "1800.0", "0.4"),
            ("IBM", "2020", "5", "1500.5", "0.48", "1500.0", "0.5"),
        };
    }
}
