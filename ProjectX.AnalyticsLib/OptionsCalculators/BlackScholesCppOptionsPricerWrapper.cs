using ProjectX.Core;
using ProjectXAnalyticsCppLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OptionType = ProjectX.Core.OptionType;

namespace ProjectX.AnalyticsLib.OptionsCalculators
{
    public class BlackScholesCppOptionsPricerWrapper : IBlackScholesCppPricer
    {
        private BlackScholesCppPricer _pricer = new BlackScholesCppPricer();
        public double PV(OptionType optionType, double spot, double strike, double rate, double carry, double maturity, double volatility)
        {
            var param = new VanillaOptionParameters(optionType.ToNativeOptionType(), strike, maturity);
            return _pricer.Value(ref param, spot, volatility, rate);
        }

        public double Delta(OptionType optionType, double spot, double strike, double rate, double carry, double maturity, double volatility)
        {
            var param = new VanillaOptionParameters(optionType.ToNativeOptionType(), strike, maturity);
            return _pricer.Delta(ref param, spot, volatility, rate);
        }

        public double Gamma(OptionType optionType, double spot, double strike, double rate, double carry, double maturity, double volatility)
        {
            var param = new VanillaOptionParameters(optionType.ToNativeOptionType(), strike, maturity);
            return _pricer.Gamma(ref param, spot, volatility, rate, 0.01);
        }

        public double ImpliedVol(OptionType optionType, double spot, double strike, double rate, double carry, double maturity, double price)
        {
            var param = new VanillaOptionParameters(optionType.ToNativeOptionType(), strike, maturity);
            return _pricer.ImpliedVolatility(ref param, spot, rate, price);
        }

        public double Rho(OptionType optionType, double spot, double strike, double rate, double carry, double maturity, double volatility)
        {
            var param = new VanillaOptionParameters(optionType.ToNativeOptionType(), strike, maturity);
            return _pricer.Rho(ref param, spot, volatility, rate);
        }

        public double Theta(OptionType optionType, double spot, double strike, double rate, double carry, double maturity, double volatility)
        {
            var param = new VanillaOptionParameters(optionType.ToNativeOptionType(), strike, maturity);
            return _pricer.Theta(ref param, spot, volatility, rate);
        }

        public double Vega(OptionType optionType, double spot, double strike, double rate, double carry, double maturity, double volatility)
        {
            var param = new VanillaOptionParameters(optionType.ToNativeOptionType(), strike, maturity);
            return _pricer.Vega(ref param, spot, volatility, rate);
        }
    }
}
