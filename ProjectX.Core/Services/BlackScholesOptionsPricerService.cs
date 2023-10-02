using ProjectX.Core.Analytics;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using System.Threading.Tasks;

namespace ProjectX.Core.Services
{
    public interface IBlackScholesOptionsPricerService
    {
        PlotResults PlotGreeks(OptionGreeks greekType, OptionType optionType, double strike, double rate, double carry, double vol);
        IEnumerable<(double maturity, OptionPricerResult optionPriceResult)> PriceFor(int timeSlices, OptionType optionType, double spot, double strike, double rate, double carry, double vol);
    }

    [Export(typeof(IBlackScholesOptionsPricerService)), PartCreationPolicy(CreationPolicy.NonShared)]    
    public class BlackScholesOptionsPricerService : IBlackScholesOptionsPricerService
    {
        public IEnumerable<(double maturity, OptionPricerResult optionPriceResult)> PriceFor(int timeSlices, OptionType optionType, double spot, double strike, double rate, double carry, double vol)
        {
            var results = new List<(double maturity, OptionPricerResult optionPriceResult)>();
            for (int i = 0; i < 10; i++)
            {
                // break out into 10 time slices until maturity
                double maturity = (i + 1.0) / 10.0;

                // price option
                double price = OptionHelper.BlackScholes(optionType, spot, strike, rate, carry, maturity, vol);
                double delta = OptionHelper.BlackScholes_Delta(optionType, spot, strike, rate, carry, maturity, vol);
                double gamma = OptionHelper.BlackScholes_Gamma(spot, strike, rate, carry, maturity, vol);
                double theta = OptionHelper.BlackScholes_Theta(optionType, spot, strike, rate, carry, maturity, vol);
                double rho = OptionHelper.BlackScholes_Rho(optionType, spot, strike, rate, carry, maturity, vol);
                double vega = OptionHelper.BlackScholes_Vega(spot, strike, rate, carry, maturity, vol);

                // return price & greeks
                var greeks = new OptionPricerResult(
                    price,
                    delta,
                    gamma,
                    theta,
                    rho,
                    vega);                
                results.Add((maturity,greeks));
            }
            return results;
        }

        public PlotResults PlotGreeks(OptionGreeks greekType, OptionType optionType, double strike, double rate, double carry, double vol)
        {
            double xmin = 0.1;
            double xmax = 3.0;
            double ymin = 10;
            double ymax = 190;
            var XLimitMin = xmin;
            var YLimitMin = ymin;
            var XSpacing = 0.1;
            var YSpacing = 5;
            var XNumber = Convert.ToInt16((xmax - xmin) / XSpacing) + 1;
            var YNumber = Convert.ToInt16((ymax - ymin) / YSpacing) + 1;

            Point3D[,] pts = new Point3D[XNumber, YNumber];
            double zmin = 10_000;
            double zmax = -10_000;
            for (int i = 0; i < XNumber; i++)
            {
                for (int j = 0; j < YNumber; j++)
                {
                    double x = XLimitMin + i * XSpacing;
                    double y = YLimitMin + j * YSpacing;
                    double z = Double.NaN;
                    var spot = y;
                    var maturity = x;
                    switch (greekType)
                    {
                        case OptionGreeks.Delta:
                            z = OptionHelper.BlackScholes_Delta(optionType, spot, strike, rate, carry, maturity, vol);
                            break;
                        case OptionGreeks.Gamma:
                            z = OptionHelper.BlackScholes_Gamma(spot, strike, rate, carry, maturity, vol);
                            break;
                        case OptionGreeks.Theta:
                            z = OptionHelper.BlackScholes_Theta(optionType, spot, strike, rate, carry, maturity, vol);
                            break;
                        case OptionGreeks.Rho:
                            z = OptionHelper.BlackScholes_Rho(optionType, spot, strike, rate, carry, maturity, vol);
                            break;
                        case OptionGreeks.Vega:
                            z = OptionHelper.BlackScholes_Vega(spot, strike, rate, carry, maturity, vol);
                            break;
                        case OptionGreeks.Price:
                            z = OptionHelper.BlackScholes(optionType, spot, strike, rate, carry, maturity, vol);
                            break;
                    }
                    if (!double.IsNaN(z))
                    {
                        pts[i, j] = new Point3D(x, y, z);
                        zmin = Math.Min(zmin, z);
                        zmax = Math.Max(zmax, z);
                    }
                }
            }
            return new PlotResults
            {
                PointArray = pts,
                zmin = zmin,
                zmax = zmax,
                XLimitMin = XLimitMin,
                YLimitMin = YLimitMin,
                XSpacing = XSpacing,
                YSpacing = YSpacing,
                XNumber = XNumber,
                YNumber = YNumber,
            };
        }
    }
}
