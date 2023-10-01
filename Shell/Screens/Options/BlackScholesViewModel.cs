using Caliburn.Micro;
using Chart3DControl;
using ProjectX.Core;
using ProjectX.Core.Services;
using System;
using System.ComponentModel.Composition;
using System.Data;
using System.Windows.Media;

namespace Shell.Screens.Options
{
    [Export(typeof(IScreen)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class BlackScholesViewModel : Screen
    {
        private readonly IEventAggregator _events;
        private readonly IBlackScholesOptionsPricerService _blackScholesPricerService;

        [ImportingConstructor]
        public BlackScholesViewModel(IEventAggregator events, IBlackScholesOptionsPricerService blackScholesOptionsPricerService)
        {
            this._events = events;
            this._blackScholesPricerService = blackScholesOptionsPricerService;
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
        private DataTable optionInputTable = new DataTable();
        private DataTable optionTable = new DataTable();
        private double zmin = 0;
        private double zmax = 0;
        private string zLabel = string.Empty;
        private double zTick = 0;
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
        #endregion
     
        public void CalculatePrice()
        {
            string? optionType = OptionInputTable.Rows[0]["Value"].ToString();
            double spot = Convert.ToDouble(OptionInputTable.Rows[1]["Value"]);
            double strike = Convert.ToDouble(OptionInputTable.Rows[2]["Value"]);
            double rate = Convert.ToDouble(OptionInputTable.Rows[3]["Value"]);
            double carry = Convert.ToDouble(OptionInputTable.Rows[4]["Value"]);
            double vol = Convert.ToDouble(OptionInputTable.Rows[5]["Value"]);        
            var results = _blackScholesPricerService.PriceFor(10, optionType.ToOptionType(), spot, strike, rate, carry, vol);

            OptionTable.Clear();
            foreach (var(maturity, riskResult) in results) 
            {
                OptionTable.Rows.Add(maturity, riskResult.price, riskResult.delta, riskResult.gamma, riskResult.theta, riskResult.rho, riskResult.vega); 
            }
        }
        public void PlotPrice() => Plot(OptionGreeks.Price, "Price", 1, 1);
        public void PlotDelta() => Plot(OptionGreeks.Delta, "Delta", 1, 1);
        public void PlotGamma() => Plot(OptionGreeks.Gamma, "Gamma", 2, 3);
        public void PlotTheta() => Plot(OptionGreeks.Theta, "Theta", 0, 0);
        public void PlotRho() => Plot(OptionGreeks.Rho, "Rho", 0, 0);
        public void PlotVega() => Plot(OptionGreeks.Vega, "Vega", 0, 0);
        private void Plot(OptionGreeks greekType, string zLabel, int zDecimalPlaces, int zTickDecimalPlaces)
        {            
            ZLabel = zLabel;

            string? optionType = OptionInputTable.Rows[0]["Value"].ToString();            
            double spot = Convert.ToDouble(OptionInputTable.Rows[1]["Value"]);
            double strike = Convert.ToDouble(OptionInputTable.Rows[2]["Value"]);
            double rate = Convert.ToDouble(OptionInputTable.Rows[3]["Value"]);
            double carry = Convert.ToDouble(OptionInputTable.Rows[4]["Value"]);
            double vol = Convert.ToDouble(OptionInputTable.Rows[5]["Value"]);
            var plotResult = _blackScholesPricerService.PlotGreeks(greekType, optionType.ToOptionType(), strike, rate, carry, vol);

            Zmin = Math.Round(plotResult.zmin, zDecimalPlaces);
            Zmax = Math.Round(plotResult.zmax, zDecimalPlaces);
            ZTick = Math.Round((plotResult.zmax - plotResult.zmin) / 5.0, zTickDecimalPlaces);
            DataCollection.Clear();
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
        }
    }
}
