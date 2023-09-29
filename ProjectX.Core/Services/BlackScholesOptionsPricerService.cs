using ProjectX.Core.Analytics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectX.Core.Services
{
    public record OptionRiskResult(double price, double delta, double gamma, double theta, double rho, double vega);    

    public class BlackScholesOptionsPricerService
    {
        public IEnumerable<(double maturity, OptionRiskResult optionRiskResult)> PriceForXTimeSlices(int timeSlices, string? optionTypeString, double spot, double strike, double rate, double carry, double vol)
        {
            if (optionTypeString == null)
            {
                throw new ArgumentNullException(nameof(optionTypeString));
            }

            OptionType optionType = ToOptionType(optionTypeString);

            var results = new List<(double maturity, OptionRiskResult optionRiskResult)>();
            for (int i = 0; i < 10; i++)
            {
                // break out into 10 time slices until maturity
                double maturity = (i + 1.0) / 10.0;

                //price & greeks
                results.Add(
                (
                    maturity,
                    new OptionRiskResult(                    
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

        static OptionType ToOptionType(string? inputOptionType) => inputOptionType == "Call" ? OptionType.Call : OptionType.Put;
    }
}
