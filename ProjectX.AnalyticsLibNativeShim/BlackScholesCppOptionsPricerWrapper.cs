using ProjectX.AnalyticsLib.Shared;
using ProjectX.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectX.AnalyticsLibNativeShim
{
    public class BlackScholesCppOptionsPricerWrapper : IBlackScholesCppPricer
    {
        public double Delta(OptionType optionType, double spot, double strike, double rate, double carry, double maturity, double volatility)
        {
            throw new NotImplementedException();
        }

        public double Gamma(OptionType optionType, double spot, double strike, double rate, double carry, double maturity, double volatility)
        {
            throw new NotImplementedException();
        }

        public double ImpliedVol(OptionType optionType, double spot, double strike, double rate, double carry, double maturity, double price)
        {
            throw new NotImplementedException();
        }

        public double PV(OptionType optionType, double spot, double strike, double rate, double carry, double maturity, double volatility)
        {
            throw new NotImplementedException();
        }

        public double Rho(OptionType optionType, double spot, double strike, double rate, double carry, double maturity, double volatility)
        {
            throw new NotImplementedException();
        }

        public double Theta(OptionType optionType, double spot, double strike, double rate, double carry, double maturity, double volatility)
        {
            throw new NotImplementedException();
        }

        public double Vega(OptionType optionType, double spot, double strike, double rate, double carry, double maturity, double vol)
        {
            throw new NotImplementedException();
        }
    }
}
