using Caliburn.Micro;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.Painting.Effects;
using ProjectX.Core.Strategy;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Data;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
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
        
        Series1 = new ISeries[]
        {
            new LineSeries<double>
            {
                Values = Signals,
                Name = Title1,
            },
        };
        Series2 = new ISeries[]
        {
            new LineSeries<double>
            {
                Values = Signals,
                Name = Title2,
            },
        };        
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
    private ObservableCollection<double> Signals = new();

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

    public ISeries[] Series1 { get; set; }
    public ISeries[] Series2 { get; set; }    

    #endregion

    #region Chart Properties 
    public string Title1 => $"{Ticker}: Stock Price (Price Type = {priceType}, Signal Type = {signalType})";
    public string Title2 => $"{Ticker}: Signal (Price Type = {priceType}, Signal Type = {signalType})";
    public Axis[] XAxes => new Axis[] { XAxis("Date") };
    public Axis[] YAxes1 => new Axis[] { YAxis("Stock Price") };    
    public Axis[] YAxes2 => new Axis[] { YAxis("Signal") };
    private static Axis XAxis(string axisLabel) =>
        new()
        {
            Name = axisLabel,
            //NamePaint = new SolidColorPaint(SKColors.Black),
            //LabelsPaint = new SolidColorPaint(SKColors.Blue),
            //TextSize = 10,
            //SeparatorsPaint = new SolidColorPaint(SKColors.LightSlateGray) { StrokeThickness = 2 }
        };

    private static Axis YAxis(string axisLabel) =>
         new()
         {
             Name = axisLabel,
             //NamePaint = new SolidColorPaint(SKColors.Red),
             //LabelsPaint = new SolidColorPaint(SKColors.Green),
             //TextSize = 20,
             //SeparatorsPaint = new SolidColorPaint(SKColors.LightSlateGray)
             //{
             //    StrokeThickness = 2,
             //    PathEffect = new DashEffect(new float[] { 3, 3 })
             //}
         };

    #endregion
    public async Task Compute()
    {
        await Task.Run(() => 
        {
            PnLTable.Clear();
            YearlyPnLTable = new DataTable();            

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
            Signals.Clear();
            foreach(var i in new double[] { 2, 1, 3, 5, 3, 4, 6 })
            {
                Signals.Add(i);
            }

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
            PnlEntity.Build(new DateTime(2023, 1, 1), "IBM", 130.8, 1.5, 120, 100, 120, 50, 500, 3, ActivePosition.INACTIVE)
        };

        public static IEnumerable<(string ticker, string period, string numTrades, string pnl, string sharpe, string pnlHold, string sharpehold)> YearlyPnL => new List<(string ticker, string period, string numTrades, string pnl, string sharpe, string pnlHold, string sharpehold)>
        {
            ("IBM", "2018", "5", "1200.5", "0.487", "1000.0", "0.9"),
            ("IBM", "2019", "5", "1230.5", "0.687", "1800.0", "0.4"),
            ("IBM", "2020", "5", "1500.5", "0.48", "1500.0", "0.5"),
            ("IBM", "Total", "15", "3931.5", "1.65", "3300.0", "1.8"),
        };
    }
}
