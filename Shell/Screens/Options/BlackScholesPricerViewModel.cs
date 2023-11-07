using Caliburn.Micro;
using Chart3DControl;
using Microsoft.AspNetCore.SignalR.Client;
using ProjectX.Core;
using ProjectX.Core.Requests;
using System;
using System.ComponentModel.Composition;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Shell.Screens.Options
{
    [Export(typeof(IScreen)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class BlackScholesPricerViewModel : Screen
    {
        private readonly IEventAggregator _events;
        private readonly IGatewayApiClient _gatewayApiClient;

        [ImportingConstructor]
        public BlackScholesPricerViewModel(IEventAggregator events, IGatewayApiClient gatewayApiClient)
        {
            this._events = events;
            this._gatewayApiClient = gatewayApiClient;
            DisplayName = "Black-Scholes (Options)";

            OptionTable.Columns.AddRange(new[]
            {
                new DataColumn("Maturity", typeof(double)),
                new DataColumn("Price", typeof(double)),
                new DataColumn("Delta", typeof(double)),
                new DataColumn("Gamma", typeof(double)),
                new DataColumn("Theta", typeof(double)),
                new DataColumn("Rho", typeof(double)),
                new DataColumn("Vega", typeof(double)),
            });

            OptionInputTable.Columns.AddRange(new[]
            {
                new DataColumn("Parameter", typeof(string)),
                new DataColumn("Value", typeof(string)),
                new DataColumn("Description", typeof(string)),
            });
            OptionInputTable.Rows.Add("OptionType", "Call", "Call for a call option, Put for a putoption");
            OptionInputTable.Rows.Add("Spot", 100, "Current price of the underlying asset");
            OptionInputTable.Rows.Add("Strike", 100, "Strike price");
            OptionInputTable.Rows.Add("Rate", 0.1, "Interest rate");
            OptionInputTable.Rows.Add("Carry", 0.04, "Cost of carry");
            OptionInputTable.Rows.Add("Vol", 0.3, "Volatility");            
        }

        #region Bindable Properties
        private DataTable optionInputTable = new();
        private DataTable optionTable = new();
        private double zmin = 0;
        private double zmax = 1;
        private string zLabel = string.Empty;
        private double zTick = 0.2;
        private CancellationTokenSource _cts = new();
        private OptionsPricingCalculatorType _calculatorType = OptionsPricingCalculatorType.OptionsPricer;

        public BindableCollection<DataSeries3D> DataCollection { get; set; } = new BindableCollection<DataSeries3D>();        
        
        public DataTable OptionInputTable
        {
            get { return optionInputTable; }
            set { optionInputTable = value; NotifyOfPropertyChange(() => OptionInputTable); }
        }
        public DataTable OptionTable
        {
            get { return optionTable; }
            set { optionTable = value; NotifyOfPropertyChange(() => OptionTable); }
        }
        public double Zmin
        {
            get { return zmin; }
            set { zmin = value; NotifyOfPropertyChange(() => Zmin); }
        }
        public double Zmax
        {
            get { return zmax; }
            set { zmax = value; NotifyOfPropertyChange(() => Zmax); }
        }
        public string ZLabel
        {
            get { return zLabel; }
            set { zLabel = value; NotifyOfPropertyChange(() => ZLabel); }
        }
        public double ZTick
        {
            get { return zTick; }
            set { zTick = value; NotifyOfPropertyChange(() => ZTick); }
        }

        public OptionsPricingCalculatorType CalculatorType
        {
            get { return _calculatorType; }
            set { _calculatorType = value; NotifyOfPropertyChange(() => CalculatorType); }
        }

        #endregion
        protected override async void OnActivate()
        {
            _cts = new CancellationTokenSource();

            await _gatewayApiClient.StartHubAsync();
            _gatewayApiClient.HubConnection.On<OptionsPricingByMaturityResults>("PricingResults", pricingResult =>
            {
                Console.WriteLine($"Received Pricing Result: {pricingResult.ResultsCount} results, requestId: {pricingResult.RequestId}");
                App.Current.Dispatcher.Invoke((System.Action)delegate
                {
                    OptionTable.Clear();
                    foreach (var (maturity, riskResult) in pricingResult.Results)
                    {
                        OptionTable.Rows.Add(maturity, riskResult.price, riskResult.delta, riskResult.gamma, riskResult.theta, riskResult.rho, riskResult.vega);
                    }
                });
            });

            _gatewayApiClient.HubConnection.On<PlotOptionsPricingResult>("PlotResults", plotPricingResult =>
            {
                var plotResult = plotPricingResult.PlotResults;
                var zDecimalPlaces = plotPricingResult.Request.ZDecimalPlaces;
                var zTickDecimalPlaces = plotPricingResult.Request.ZTickDecimalPlaces;

                Console.WriteLine($"Received Pricing 3D Plot Result: {plotResult.PointArray.Length} coordinates, requestId: {plotPricingResult.RequestId}");

                App.Current.Dispatcher.Invoke((System.Action)delegate
                {
                    DataCollection.Clear();
                    ZLabel = zLabel;
                    Zmin = Math.Round(plotResult.zmin, zDecimalPlaces);
                    Zmax = Math.Round(plotResult.zmax, zDecimalPlaces);
                    ZTick = Math.Round((plotResult.zmax - plotResult.zmin) / 5.0, zTickDecimalPlaces);            

                    DataCollection.Add(new DataSeries3D()
                    {
                        LineColor = Brushes.Black,
                        PointArray = plotResult.PointArray.ToChartablePointArray(),
                        XLimitMin = plotResult.XLimitMin,
                        YLimitMin = plotResult.YLimitMin,
                        XSpacing = plotResult.XSpacing,
                        YSpacing = plotResult.YSpacing,
                        XNumber = plotResult.XNumber,
                        YNumber = plotResult.YNumber
                    });
                });
            });

            base.OnActivate();
        }

        protected override async void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);

            _cts.Cancel();
            await _gatewayApiClient.StopHubAsync();
        }

        public async Task CalculatePrice()
        {            
            try
            {                
                string? optionType = OptionInputTable.Rows[0]["Value"].ToString();
                double spot = Convert.ToDouble(OptionInputTable.Rows[1]["Value"]);
                double strike = Convert.ToDouble(OptionInputTable.Rows[2]["Value"]);
                double rate = Convert.ToDouble(OptionInputTable.Rows[3]["Value"]);
                double carry = Convert.ToDouble(OptionInputTable.Rows[4]["Value"]);
                double vol = Convert.ToDouble(OptionInputTable.Rows[5]["Value"]);
                var request = new OptionsPricingByMaturitiesRequest(10, optionType.ToOptionType(), spot, strike, rate, carry, vol, CalculatorType);
                OptionTable.Clear();
                await _gatewayApiClient.SubmitPricingRequest(request, _cts.Token);                                            
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to send pricing request to backend to price\nReason:'{ex.Message}", "Calulate Price issue");
            }            
        }
        public void PlotPrice() => Plot(OptionGreeks.Price, "Price", 1, 1);
        public void PlotDelta() => Plot(OptionGreeks.Delta, "Delta", 1, 1);
        public void PlotGamma() => Plot(OptionGreeks.Gamma, "Gamma", 2, 3);
        public void PlotTheta() => Plot(OptionGreeks.Theta, "Theta", 0, 0);
        public void PlotRho() => Plot(OptionGreeks.Rho, "Rho", 0, 0);
        public void PlotVega() => Plot(OptionGreeks.Vega, "Vega", 0, 0);
        private async void Plot(OptionGreeks greekType, string zLabel, int zDecimalPlaces, int zTickDecimalPlaces)
        {            
            try
            {
                string? optionType = OptionInputTable.Rows[0]["Value"].ToString();
                double spot = Convert.ToDouble(OptionInputTable.Rows[1]["Value"]);
                double strike = Convert.ToDouble(OptionInputTable.Rows[2]["Value"]);
                double rate = Convert.ToDouble(OptionInputTable.Rows[3]["Value"]);
                double carry = Convert.ToDouble(OptionInputTable.Rows[4]["Value"]);
                double vol = Convert.ToDouble(OptionInputTable.Rows[5]["Value"]);
                var request = new PlotOptionsPricingRequest(greekType, optionType!.ToOptionType(), strike, rate, carry, vol, CalculatorType);
                request.ZLabel = zLabel;
                request.ZDecimalPlaces = zDecimalPlaces;
                request.ZTickDecimalPlaces = zTickDecimalPlaces;
                await _gatewayApiClient.SubmitPlotRequest(request, _cts.Token);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to send plot request to backend to price\nReason:'{ex.Message}", "Calulate Plot parameters issue");
            }                        
        }

        public void PlotExperimental()
        {
            DataCollection.Clear();
            var ds = new DataSeries3D();
            ds.LineColor = Brushes.Black;
            ChartFunctions.Peak3D(ds);
            Zmin = -8;
            Zmax = 8;
            ZTick = 4;            
            DataCollection.Add(ds);
        }

        public void CalculatorTypeToggle()
        {
            var target = CalculatorType switch
            {
                OptionsPricingCalculatorType.OptionsPricer => OptionsPricingCalculatorType.OptionsPricerCpp,
                OptionsPricingCalculatorType.OptionsPricerCpp => OptionsPricingCalculatorType.OptionsPricer,
                _ => throw new NotImplementedException(),
            };

            CalculatorType = target;
        }
    }
}
