using Caliburn.Micro;
using Chart3DControl;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Shell.Screens.Options
{
    [Export(typeof(IScreen)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class BlackScholesViewModel : Screen
    {
        private readonly IEventAggregator events;

        [ImportingConstructor]
        public BlackScholesViewModel(IEventAggregator events)
        {
            this.events = events;
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

        /*
        public void CalculatePrice()
        {
            (OptionType optionType, double spot, double strike, double rate, double carry, double vol) = FromUI();
            OptionTable.Clear();
            for (int i = 0; i < 10; i++)
            {
                // break out into 10 time slices until maturity
                double maturity = (i + 1.0) / 10.0;

                //price & greeks
                _ = OptionTable.Rows.Add(
                    maturity,
                    OptionHelper.BlackScholes(optionType, spot, strike, rate, carry, maturity, vol),
                    OptionHelper.BlackScholes_Delta(optionType, spot, strike, rate, carry, maturity, vol),
                    OptionHelper.BlackScholes_Gamma(spot, strike, rate, carry, maturity, vol),
                    OptionHelper.BlackScholes_Theta(optionType, spot, strike, rate, carry, maturity, vol),
                    OptionHelper.BlackScholes_Rho(optionType, spot, strike, rate, carry, maturity, vol),
                    OptionHelper.BlackScholes_Vega(spot, strike, rate, carry, maturity, vol));
            }
        }

        public void PlotPrice() => Plot(GreekTypeEnum.Price, "Price", 1, 1);
        public void PlotDelta() => Plot(GreekTypeEnum.Delta, "Delta", 1, 1);
        public void PlotGamma() => Plot(GreekTypeEnum.Gamma, "Gamma", 2, 3);
        public void PlotTheta() => Plot(GreekTypeEnum.Theta, "Theta", 0, 0);
        public void PlotRho() => Plot(GreekTypeEnum.Rho, "Rho", 0, 0);
        public void PlotVega() => Plot(GreekTypeEnum.Vega, "Vega", 0, 0);
        private void Plot(GreekTypeEnum greekType, string zLabel, int zDecimalPlaces, int zTickDecimalPlaces)
        {
            ZLabel = zLabel;
            (OptionType optionType, double spot, double strike, double rate, double carry, double vol) = FromUI();
            DataCollection.Clear();
            DataSeries3D ds = new DataSeries3D() { LineColor = Brushes.Black };
            double[] z = OptionPlotHelper.PlotGreeks(ds, greekType, optionType, strike, rate, carry, vol);
            Zmin = Math.Round(z[0], zDecimalPlaces);
            Zmax = Math.Round(z[1], zDecimalPlaces);
            ZTick = Math.Round((z[1] - z[0]) / 5.0, zTickDecimalPlaces);
            DataCollection.Add(ds);
        }

        private (OptionType optionType, double spot, double strike, double rate, double carry, double vol) FromUI()
        {
            OptionType optionType = optionInputTable.Rows[0]["Value"].ToString() == "Call" ? OptionType.Call : OptionType.Put;
            double spot = Convert.ToDouble(OptionInputTable.Rows[1]["Value"]);
            double strike = Convert.ToDouble(OptionInputTable.Rows[2]["Value"]);
            double rate = Convert.ToDouble(OptionInputTable.Rows[3]["Value"]);
            double carry = Convert.ToDouble(OptionInputTable.Rows[4]["Value"]);
            double vol = Convert.ToDouble(OptionInputTable.Rows[5]["Value"]);
            return (optionType, spot, strike, rate, carry, vol);
        }
        
         */
    }
}
