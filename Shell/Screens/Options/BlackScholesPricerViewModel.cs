using Caliburn.Micro;
using Chart3DControl;
using Microsoft.AspNetCore.SignalR.Client;
using ProjectX.Core;
using ProjectX.Core.Requests;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data;
using System.Net.Http;
using System.Net.Sockets;
using System.Net.WebSockets;
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

            static DataColumn[] optionTableHeaders()
            {
                return new[]
                {
                    new DataColumn("Maturity", typeof(double)),
                    new DataColumn("Price", typeof(double)),
                    new DataColumn("Delta", typeof(double)),
                    new DataColumn("Gamma", typeof(double)),
                    new DataColumn("Theta", typeof(double)),
                    new DataColumn("Rho", typeof(double)),
                    new DataColumn("Vega", typeof(double)),
                };
            }
            OptionTable.Columns.AddRange(optionTableHeaders());
            OptionTable2.Columns.AddRange(optionTableHeaders());
            static void AddSomeBlankData(DataTable optionTable)
            {
                double col = 0.1;
                for (int i = 0; i <= 10; i++)
                {
                    var r = optionTable.NewRow();
                    r[0] = col;
                    optionTable.Rows.Add(r);
                    col =+ (double)(i + 1) / 10;
                }
            }
            AddSomeBlankData(OptionTable);
            AddSomeBlankData(OptionTable2);

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
        private string _calculatorTypeDesc = "Default pricer";

        private DataTable optionTable2 = new();
        private OptionsPricingCalculatorType _calculatorType2 = OptionsPricingCalculatorType.OptionsPricerCpp;
        private string _calculatorTypeDesc2 = "Default c++ pricer";

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

        public string CalculatorTypeDesc
        {
            get { return _calculatorTypeDesc; }
            set { _calculatorTypeDesc = value; NotifyOfPropertyChange(() => CalculatorTypeDesc); }
        }

        public DataTable OptionTable2
        {
            get { return optionTable2; }
            set { optionTable2 = value; NotifyOfPropertyChange(() => OptionTable2); }
        }

        public OptionsPricingCalculatorType CalculatorType2
        {
            get { return _calculatorType2; }
            set { _calculatorType2 = value; NotifyOfPropertyChange(() => CalculatorType2); }
        }

        public string CalculatorTypeDesc2
        {
            get { return _calculatorTypeDesc2; }
            set { _calculatorTypeDesc2 = value; NotifyOfPropertyChange(() => CalculatorTypeDesc2); }
        }
        #endregion
        protected override async void OnActivate()
        {
            _cts = new CancellationTokenSource();
            try
            {
                UpdateStatus($"Connecting to backend... {_gatewayApiClient.ToString()}");
                await _gatewayApiClient.StartHubAsync();
            }
            catch(HttpRequestException socketExp)
            {
                MessageBox.Show("Your connection string to the backend is incorrect.  Hint: it is unset by default.", "You made it this far...");
            }            
            try
            {
                _gatewayApiClient.HubConnection.On("PricingResults", (Action<OptionsPricingByMaturityResults>)(pricingResult =>
                {
                    Console.WriteLine($"Received Pricing Result: {pricingResult.ResultsCount} results, requestId: {pricingResult.RequestId}, time: {pricingResult.AuditTrail.ElapsedMilliseconds} ms");
                    UpdateStatus($"Pricing Result with {pricingResult.ResultsCount} results received. Backed completed operation in {pricingResult.AuditTrail.ElapsedMilliseconds} ms using calculator {pricingResult.AuditTrail.CalculatorType}");
                    App.Current.Dispatcher.Invoke((System.Action)delegate
                    {
                        DataTable optionTable = GetOptionTableToModify(pricingResult.AuditTrail.CalculatorType);
                        optionTable.Clear();
                        foreach (var (maturity, riskResult) in pricingResult.Results)
                        {
                            optionTable.Rows.Add(maturity, riskResult.price, riskResult.delta, riskResult.gamma, riskResult.theta, riskResult.rho, riskResult.vega);
                        }
                    });
                }));
            }
            catch(Exception ex) 
            {
                MessageBox.Show($"Disabling backend responses as we cannot stream from SignalR Hub, Reason: '{ex.Message}'", "Press ok to continue.");
            }
            
            try
            {
                _gatewayApiClient.HubConnection.On<PlotOptionsPricingResult>("PlotResults", plotPricingResult =>
                {
                    var plotResult = plotPricingResult.PlotResults;
                    var zDecimalPlaces = plotPricingResult.Request.ZDecimalPlaces;
                    var zTickDecimalPlaces = plotPricingResult.Request.ZTickDecimalPlaces;

                    Console.WriteLine($"Received Pricing 3D Plot Result: {plotResult.PointArray.Length} coordinates, requestId: {plotPricingResult.RequestId}");
                    UpdateStatus($"Pricing 3D Plot Result: {plotResult.PointArray.Length} coordinates completed. Backed completed operation in {plotPricingResult.AuditTrail.ElapsedMilliseconds} ms using calculator {plotPricingResult.AuditTrail.CalculatorType}");
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
            }
            catch(Exception ex)
            {
                MessageBox.Show($"Disabling backend responses as we cannot stream from SignalR Hub, Reason: '{ex.Message}'", "Press ok to continue.");
            }
            
            base.OnActivate();
            
            DataTable GetOptionTableToModify(OptionsPricingCalculatorType c)
            {                
                if (c == CalculatorType)
                {
                    return OptionTable;
                }
                else if (c == CalculatorType2)
                {
                    return OptionTable2;
                }
                throw new Exception($"Can't find which option table to update based on {c}");
            }
        }       

        protected override async void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);

            _cts.Cancel();
            await _gatewayApiClient.StopHubAsync();
        }
        public async Task CalculatePrice2()
        {
            if(CalculatorType2 == CalculatorType)
            {
                MessageBox.Show("You must set different calculator types for comparison");
                return;
            }
            OptionTable2.Clear();
            await CalculatePrice(CalculatorType2);
        }
        public async Task CalculatePrice()
        {
            OptionTable.Clear();
            await CalculatePrice(CalculatorType);
        }
        private async Task CalculatePrice(OptionsPricingCalculatorType calculatorType)
        {
            try
            {
                string? optionType = OptionInputTable.Rows[0]["Value"].ToString();
                double spot = Convert.ToDouble(OptionInputTable.Rows[1]["Value"]);
                double strike = Convert.ToDouble(OptionInputTable.Rows[2]["Value"]);
                double rate = Convert.ToDouble(OptionInputTable.Rows[3]["Value"]);
                double carry = Convert.ToDouble(OptionInputTable.Rows[4]["Value"]);
                double vol = Convert.ToDouble(OptionInputTable.Rows[5]["Value"]);
                var request = new OptionsPricingByMaturitiesRequest(10, optionType.ToOptionType(), spot, strike, rate, carry, vol, calculatorType);

                await _gatewayApiClient.SubmitPricingRequest(request, _cts.Token);
                UpdateStatus("Price submitted to backend.");
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
                UpdateStatus("Submitted Plot Price Request to backend.");
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
        public void CalculatorTypeToggle2()
        {
            (OptionsPricingCalculatorType next, string desc) = NextCalculator(CalculatorType2);

            CalculatorType2 = next;
            CalculatorTypeDesc2 = desc;
        }
        public void CalculatorTypeToggle()
        {
            (OptionsPricingCalculatorType next, string desc) = NextCalculator(CalculatorType);

            CalculatorType = next;
            CalculatorTypeDesc = desc;
        }        
        private static (OptionsPricingCalculatorType, string) NextCalculator(OptionsPricingCalculatorType calculatorType)
        {
            return calculatorType switch
            {
                OptionsPricingCalculatorType.OptionsPricer =>
                (
                    OptionsPricingCalculatorType.OptionsPricerCpp,
                    "BlackScholes Options Pricer native C++"
                ),
                OptionsPricingCalculatorType.OptionsPricerCpp =>
                (
                    OptionsPricingCalculatorType.MonteCarloCppPricer,
                    "MonteCarlo Pricing native C++ (1e8 MC Paths)"
                ),
                OptionsPricingCalculatorType.MonteCarloCppPricer =>
               (
                    OptionsPricingCalculatorType.OptionsPricer,
                    "Vanilla BlackScholes Options Pricer C#"
               ),
                _ => throw new NotImplementedException(),
            };
        }
        private void UpdateStatus(string status)
        {
            _events.PublishOnUIThread(new ModelEvents(new List<object>(new object[] { status })));
        }
    }
}
