using Caliburn.Micro;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.Painting.Effects;
using ProjectX.Core.Services;
using ProjectX.Core.Strategy;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Xml;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Shell.Screens.TradingSignals;

[Export(typeof(IScreen)), PartCreationPolicy(CreationPolicy.NonShared)]
public partial class SingleViewModel : Screen
{
    private readonly IEventAggregator _eventAggregator;
    private readonly IStockSignalService _stockSignalService;

    [ImportingConstructor]
    public SingleViewModel(IEventAggregator eventAggregator, IStockSignalService stockSignalService)
    {
        _eventAggregator = eventAggregator;
        _stockSignalService = stockSignalService;
        DisplayName = "Mean Reversion strategy (Backtesting)";    
    }

    private string ticker = "ACAQ";
    private DateTime fromDate = new(2023, 9, 1);
    private DateTime toDate = new(2023, 9, 25);
    private string signalType = "MovingAverage";
    private string priceType = "Close";
    private int notional = 10_000;
    private DataTable pnlRankingTable = new() ;
    private DataTable yearlyPnLTable = new();
    private BindableCollection<PnlEntity> pnlTable = new();
    private ISeries[] _series1 = Array.Empty<ISeries>();
    private ISeries[] _series2 = Array.Empty<ISeries>();
    private Axis[] _yAxes1 = Array.Empty<Axis>();
    private Axis[] _yAxes2 = Array.Empty<Axis>();
    private string _title1 = string.Empty;
    private string _title2 = string.Empty;
    private BindableCollection<PriceSignalEntity> _signals = new();

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

    public ISeries[] Series1 
    {
        get { return _series1; }
        set { _series1 = value; NotifyOfPropertyChange(() => Series1); }
    }
    public ISeries[] Series2
    {
        get { return _series2; }
        set { _series2 = value; NotifyOfPropertyChange(() => Series2); }
    }

    #endregion

    #region Chart Properties     
    public string Title1
    {
        get { return _title1; }
        set { _title1 = value; NotifyOfPropertyChange(() => Title1); }
    }
    public string Title2
    {
        get { return _title2; }
        set { _title2 = value; NotifyOfPropertyChange(() => Title2); }
    }    
    public Axis[] XAxes => new Axis[] { XAxis("Date") };
    public Axis[] YAxes1
    {
        get { return _yAxes1; }
        set { _yAxes1 = value; NotifyOfPropertyChange(() => YAxes1); }
    } 
    public Axis[] YAxes2
    {
        get { return _yAxes2; }
        set { _yAxes2 = value; NotifyOfPropertyChange(() => YAxes2); }
    } 
    private static Axis XAxis(string axisLabel) => new()
    {
        Name = axisLabel,
        Labeler = value => new DateTime((long)value).ToString("MM-dd"),
        NameTextSize = 14,
        TextSize = 8,
    };

    private static Axis YAxis(string axisLabel) => new()
    {
        Name = axisLabel,
        Labeler = Labelers.SixRepresentativeDigits,
        NameTextSize = 14,
        TextSize = 12,
        //MinLimit = 0,       
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

            // TODO: get stock data
            // compute parameter permutations for strategy ad resultant pnl for each OptimHelper.OptimSingleName(data, SelectedSignalType, SelectedStrategyType, IsReinvest
            // store results into PnLRankingTable above

            return Task.CompletedTask;
        });
    }

    protected override async void OnActivate()
    {
        base.OnActivate();

        const int movingWindow = 5;        
        PnLRankingTable = PnlRankingTableDefaults(movingWindow).Build();
        await DisplayPriceAndSignalViewAsync(movingWindow);

        static PnlRankingTableBuilder PnlRankingTableDefaults(int movingWindow, double signalIn = 0, double signalOut = 0)
        {
            return new PnlRankingTableBuilder(
            new List<(string ticker, int bar, double zin, double zout, int numTrades, double pnlCum, double sharpe)>()
            {
                ("IBM", movingWindow, signalIn, signalOut, 0, 0, 0),
            });
        }
    }
  
    public async void SelectedCellChanged(object sender, SelectedCellsChangedEventArgs e)
    {
        await Task.Run(() =>
        {            
            var selectedCells = e.AddedCells;
            if (selectedCells.Count - 1 <= 0) return;            
            DataRowView row = (DataRowView)selectedCells[selectedCells.Count - 1].Item;            
            int movingWindow = Convert.ToInt32(row[1]);
            double signalIn = Convert.ToDouble(row[2]);
            double signalOut = Convert.ToDouble(row[3]);

            DisplayAccumulatedPnlAndDrawdownForStrategyViewAsync(movingWindow, signalIn, signalOut);    
        });
    }

    /// <summary>
    /// Price (1) accompanied by Signals chart (2)
    /// </summary>    
    private async Task DisplayPriceAndSignalViewAsync(int movingWindow)
    {
        try
        {
            var smoothenedSignals = await _stockSignalService.GetSignalUsingMovingAverageByDefault(Ticker, FromDate, ToDate, movingWindow);
            _signals = new BindableCollection<PriceSignalEntity>(smoothenedSignals);

            var priceChart = PlotPriceChart(_signals);
            Series1 = priceChart.series;
            Title1 = priceChart.title;
            YAxes1 = priceChart.yAxis;

            var signalChart = PlotSignalChart(_signals);
            Series2 = signalChart.series;
            Title2 = signalChart.title;
            YAxes2 = signalChart.yAxis;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to process stock market data\nReason:'{ex.Message}", "Stock signals issue");
        }
    }

    private async Task DisplayAccumulatedPnlAndDrawdownForStrategyViewAsync(int movingWindow, double signalIn, double signalOut)
    {
        var smoothenedSignals = await _stockSignalService.GetSignalUsingMovingAverageByDefault(Ticker, FromDate, ToDate, movingWindow);
        // TODO: Compute PnL Long Short strategy
        // var pnls = BacktestHelper.ComputeLongShortPnl(signal, 10_000.0, signalIn, signalOut, SelectedStrategyType, IsReinvest).ToList();
        PnLTable.Clear();
        PnLTable.AddRange(DummyData.DummyPnlTable);

        // filled pnl for strategy

        // do the aggregate yearly stats
        // TODO: Compute Yearly PnL
        // BacktestHelper.GetYearlyPnl(PnLTable.ToList())
        var builder = new YearlyPnLTableBuilder();
        builder.SetRows(DummyData.YearlyPnL);
        YearlyPnLTable = builder.Build();

        // Drawdown Graphs
        // TODO: BacktestHelper code get drawdown
        // var results = BacktestHelper.GetDrawDown(PnLCollection.ToList(), 10_000.0);
        // DataTable dt2 = new DataTable();

        // Plot Accumulated PnL chart (1) accompanied by the Drawdown Chart (2)
        // TODO: 
        // Add PnlCharts code here for Chart1
        // Add Drawdown(usingStrategy=true) for Chart2 

    }

    private (ISeries[] series, string title, Axis[] yAxis) PlotPriceChart(IEnumerable<PriceSignalEntity> signals) 
    {
        var series = new ISeries[]
        {
            new LineSeries<PriceSignalEntity>
            {
                Values = signals,
                Name = "Original Price",
                Mapping = (x, y) => y.Coordinate = new (x.Date.Ticks, (double)x.Price),
                Stroke = new SolidColorPaint(SKColors.Blue),                
                DataLabelsPaint = new SolidColorPaint(SKColors.Blue),
                DataLabelsPosition = LiveChartsCore.Measure.DataLabelsPosition.Top,
                DataLabelsFormatter = (point) => point.Coordinate.PrimaryValue.ToString("N1"),
                DataLabelsSize=10,
                ScalesYAt = 0
            },
            new LineSeries<PriceSignalEntity>
            {
                Values = signals,
                Name = "Predicted Price",
                Mapping = (x, y) => y.Coordinate = new (x.Date.Ticks, (double)x.PricePredicted),
                Stroke = new SolidColorPaint(SKColors.Red),
                DataLabelsPaint = new SolidColorPaint(SKColors.Red),
                DataLabelsPosition = LiveChartsCore.Measure.DataLabelsPosition.Top,
                DataLabelsFormatter = (point) => point.Coordinate.PrimaryValue.ToString("N1"),
                DataLabelsSize=10,
                ScalesYAt = 0
            },
             new LineSeries<PriceSignalEntity>
            {
                Values = signals,
                Name = "Upper Band",
                Stroke = new SolidColorPaint(SKColors.DarkGreen),
                Mapping = (x, y) => y.Coordinate = new(x.Date.Ticks, (double)x.UpperBand),
                DataLabelsPaint = new SolidColorPaint(SKColors.DarkGreen),
                DataLabelsPosition = LiveChartsCore.Measure.DataLabelsPosition.Top,
                DataLabelsFormatter = (point) => point.Coordinate.PrimaryValue.ToString("N1"),
                DataLabelsSize=10,                
                ScalesYAt = 0
            },
            new LineSeries<PriceSignalEntity>
            {
                Values = signals,
                Name = "Lower Band",
                Stroke = new SolidColorPaint(SKColors.DarkGreen),
                Mapping = (x, y) => y.Coordinate = new(x.Date.Ticks, (double)x.LowerBand),
                DataLabelsPaint = new SolidColorPaint(SKColors.DarkGreen),
                DataLabelsPosition = LiveChartsCore.Measure.DataLabelsPosition.Top,
                DataLabelsFormatter = (point) => point.Coordinate.PrimaryValue.ToString("N1"),
                DataLabelsSize = 10,
                ScalesYAt = 0,
            },
        };
        var title = $"{Ticker}: Stock Price (Price Type = {priceType}, Signal Type = {signalType})";
        var yAxes = new[] { YAxis("Stock Price") };

        return (series, title, yAxes);
    }

    private (ISeries[] series, string title, Axis[] yAxis) PlotSignalChart(IEnumerable<PriceSignalEntity> signals)
    {
       var series = new ISeries[]
       {
            new LineSeries<PriceSignalEntity>
            {
                Values = signals,
                Name = "Signal",                
                Mapping = (x, y) => y.Coordinate = new(x.Date.Ticks, (double)x.Signal),
                DataLabelsPaint = new SolidColorPaint(SKColors.DarkGreen),
                DataLabelsPosition = LiveChartsCore.Measure.DataLabelsPosition.Top,
                DataLabelsFormatter = (point) => point.Coordinate.PrimaryValue.ToString("N1"),
                DataLabelsSize = 10,
                ScalesYAt = 0,
            },
       };
       var title = $"{Ticker}: Signal (Price Type = {priceType}, Signal Type = {signalType})";
       var yAxes = new[] { YAxis("Signal") };

       return (series, title, yAxes);
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

        public static IEnumerable<PriceSignalEntity> Signals => new ProjectX.Core.SignalBuilder("IBM").Build(10, 20);        
    }
}
