using ProjectX.Core.Analytics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using System.Threading.Tasks;

namespace ProjectX.Core.Services
{
    public class BlackScholesOptionsPricerService
    {
        public IEnumerable<(double maturity, OptionPriceResult optionPriceResult)> PriceFor(int timeSlices, OptionType optionType, double spot, double strike, double rate, double carry, double vol)
        {
            var results = new List<(double maturity, OptionPriceResult optionPriceResult)>();
            for (int i = 0; i < 10; i++)
            {
                // break out into 10 time slices until maturity
                double maturity = (i + 1.0) / 10.0;

                //price & greeks
                results.Add(
                (
                    maturity,
                    new OptionPriceResult(                    
                        OptionHelper.BlackScholes(optionType, spot, strike, rate, carry, maturity, vol),
                        OptionHelper.BlackScholes_Delta(optionType, spot, strike, rate, carry, maturity, vol),
                        OptionHelper.BlackScholes_Gamma(spot, strike, rate, carry, maturity, vol),
                        OptionHelper.BlackScholes_Theta(optionType, spot, strike, rate, carry, maturity, vol),
                        OptionHelper.BlackScholes_Rho(optionType, spot, strike, rate, carry, maturity, vol),
                        OptionHelper.BlackScholes_Vega(spot, strike, rate, carry, maturity, vol))
                    )
                );
            }
            return results;
        }

        public PlotResults PlotGreeks(GreekTypeEnum greekType, OptionType optionType, double strike, double rate, double carry, double vol)
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
                        case GreekTypeEnum.Delta:
                            z = OptionHelper.BlackScholes_Delta(optionType, spot, strike, rate, carry, maturity, vol);
                            break;
                        case GreekTypeEnum.Gamma:
                            z = OptionHelper.BlackScholes_Gamma(spot, strike, rate, carry, maturity, vol);
                            break;
                        case GreekTypeEnum.Theta:
                            z = OptionHelper.BlackScholes_Theta(optionType, spot, strike, rate, carry, maturity, vol);
                            break;
                        case GreekTypeEnum.Rho:
                            z = OptionHelper.BlackScholes_Rho(optionType, spot, strike, rate, carry, maturity, vol);
                            break;
                        case GreekTypeEnum.Vega:
                            z = OptionHelper.BlackScholes_Vega(spot, strike, rate, carry, maturity, vol);
                            break;
                        case GreekTypeEnum.Price:
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
