using Accord.Math;
using Caliburn.Micro;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using ProjectX.MachineLearning;
using Skender.Stock.Indicators;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ISeries = LiveChartsCore.ISeries;

namespace Shell.Screens.MachineLearning
{
    [Export(typeof(IScreen)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PriceMovementPredictionViewModel : Screen
    {
        private readonly IEventAggregator eventAggregator;
        private readonly IMachineLearningApiClient machineLearningApiClient;

        [ImportingConstructor]
        public PriceMovementPredictionViewModel(
            IEventAggregator eventAggregator,
            IMachineLearningApiClient machineLearningApiClient)
        {
            this.eventAggregator = eventAggregator;
            this.machineLearningApiClient = machineLearningApiClient;

            DisplayName = "Stock Price Direction Prediction using Classification (MachineLearning)";
        }

        private string ticker = "GE";
        private DateTime fromDate = new(2023, 5, 1);
        private DateTime toDate = new(2023, 9, 25);

        private DateTime trainingFromDate = new(2023, 5, 1);
        private DateTime trainingToDate = new(2023, 9, 25);

        private ISeries[] _series1 = Array.Empty<ISeries>();
        private ISeries[] _series2 = Array.Empty<ISeries>();
        private Axis[] _yAxes1 = Array.Empty<Axis>();
        private Axis[] _yAxes2 = Array.Empty<Axis>();
        private string _title1 = string.Empty;
        private string _title2 = string.Empty;
        private DataTable stockTable = new DataTable();
        private DataTable confusionMatrixTrainingSetTable;
        private DataTable confusionMatrixPredictionSetTable;

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
        public DateTime TrainingFromDate
        {
            get { return fromDate; }
            set { fromDate = value; NotifyOfPropertyChange(() => FromDate); }
        }
        public DateTime TrainingToDate
        {
            get { return toDate; }
            set { toDate = value; NotifyOfPropertyChange(() => ToDate); }
        }

        public DataTable StockTable
        {
            get { return stockTable; }
            set { stockTable = value; NotifyOfPropertyChange(() => StockTable); }
        }

        public DataTable ConfusionMatrixTrainingSetTable
        {
            get { return confusionMatrixTrainingSetTable; }
            set { confusionMatrixTrainingSetTable = value; NotifyOfPropertyChange(() => ConfusionMatrixTrainingSetTable); }
        }

        public DataTable ConfusionMatrixPredictionSetTable
        {
            get { return confusionMatrixPredictionSetTable; }
            set { confusionMatrixPredictionSetTable = value; NotifyOfPropertyChange(() => ConfusionMatrixPredictionSetTable); }
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
        public Axis[] XAxes1 => new Axis[] { XAxis("Date") };
        public Axis[] XAxes2 => new Axis[] { XAxis("K") };
        public Axis[] YAxes1 => new Axis[] { YAxis("Stock Price") };
        public Axis[] YAxes2 => new Axis[] { YAxis(("AccuracyPredict")), YAxis(("AccuracyTraining")) };
        private static Axis XAxis(string axisLabel) => new()
        {
            Name = axisLabel,
            Labeler = value => new DateTime((long)value).ToString("MM-dd"),
            NameTextSize = 14,
            TextSize = 12,
        };

        private static Axis YAxis(string axisLabel) => new()
        {
            Name = axisLabel,
            Labeler = Labelers.SixRepresentativeDigits,
            NameTextSize = 14,
            TextSize = 12,
            //MinLimit = 0,       
        };

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

        StockTableBuilder _stockTableBuilder = new StockTableBuilder();
        public async Task Load()
        {
            try
            {
                // load symbol dates from market data source into tableBuilder
                var prices = await machineLearningApiClient.LoadPrices(Ticker, FromDate, ToDate);

                _stockTableBuilder.SetRows(prices);

                StockTable = _stockTableBuilder.Build();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void Expected()
        {
            if (StockTable.Rows.Count == 0) return;

            for (int i = 1; i < StockTable.Rows.Count; i++)
            {
                double prev = (double)StockTable.Rows[i - 1]["Close"];
                double curr = (double)StockTable.Rows[i]["Close"];
                StockPriceTrendDirection expected = curr switch {
                    _ when curr > prev => StockPriceTrendDirection.Upward,
                    _ when curr == prev => StockPriceTrendDirection.Flat,
                    _ when curr < prev => StockPriceTrendDirection.Downward,
                    _ => StockPriceTrendDirection.Unset,
                };
                StockTable.Rows[i - 1]["Expected"] = expected;
            }
            StockTable.Rows.RemoveAt(StockTable.Rows.Count - 1);
        }

        public async Task Compute()
        {
            try
            {
                // Extract inputTrain from StockTable DataTable
                string[] trainingDataHeaders = ["Open", "High", "Low", "Close"];
                double[][] inputs = StockTable.ToJagged<double>(trainingDataHeaders);
                var outputStrings = StockTable.ToArray<string>("Expected");
                int[] outputs = outputStrings.Select(o => (int)Enum.Parse(typeof(StockPriceTrendDirection), o)).ToArray();

                Burn(StockTable);
                DataTable dtb = StockTable.DefaultView.ToTable(false, trainingDataHeaders);                
                var result = await this.machineLearningApiClient.Svm(TrainingFromDate, TrainingToDate, inputs, outputs);

                // TODO interpret Knn results and show them on the screen xx
            }
            catch (Exception exp) 
            {
                MessageBox.Show("Error occured running ML model exp: 'Reason:" + exp.Message + "'");
            }
        }

        private void Burn(DataTable stockTable)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Date, Open, High, Low, Close, Expected, Predicted");
            foreach (DataRow row in stockTable.Rows)
            {
                var date = (string) row["Date"];
                var open = row["Open"];
                var high = row["High"];
                var low = row["Low"];
                var close = row["Close"];
                var expected = row["Expected"];
                var predicted = row["Predicted"];

                sb.AppendLine($"{date},{open},{high},{low},{close},{expected},{predicted}");
            }
            var filename = $"{Ticker}_{FromDate.ToString("ddMMyy")}_{ToDate.ToString("ddMMyy")}.csv";
            File.WriteAllText(filename, sb.ToString());
        }
    }
}
