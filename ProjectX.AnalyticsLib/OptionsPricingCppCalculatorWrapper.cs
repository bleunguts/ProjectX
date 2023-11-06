using ProjectX.Core;
using ProjectXAnalyticsCppLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OptionType = ProjectX.Core.OptionType;

namespace ProjectX.AnalyticsLib
{
    public class OptionsPricingCppCalculatorWrapper : IBlackScholesOptionsPricingCalculator
    {
        private readonly OptionsPricingCppCalculator _calculator;

        public OptionsPricingCppCalculatorWrapper()
        {
            _calculator = new OptionsPricingCppCalculator(new RandomWalk(RandomAlgorithm.BoxMuller));
        }
        public double BlackScholes(OptionType optionType, double spot, double strike, double rate, double carry, double maturity, double volatility)
        {
            throw new NotImplementedException();
        }

        public double BlackScholes_Delta(OptionType optionType, double spot, double strike, double rate, double carry, double maturity, double volatility)
        {
            throw new NotImplementedException();
        }

        public double BlackScholes_Gamma(double spot, double strike, double rate, double carry, double maturity, double volatility)
        {
            throw new NotImplementedException();
        }

        public double BlackScholes_ImpliedVol(OptionType optionType, double spot, double strike, double rate, double carry, double maturity, double price)
        {
            throw new NotImplementedException();
        }

        public double BlackScholes_Rho(OptionType optionType, double spot, double strike, double rate, double carry, double maturity, double volatility)
        {
            throw new NotImplementedException();
        }

        public double BlackScholes_Theta(OptionType optionType, double spot, double strike, double rate, double carry, double maturity, double volatility)
        {
            throw new NotImplementedException();
        }

        public double BlackScholes_Vega(double spot, double strike, double rate, double carry, double maturity, double vol)
        {
            throw new NotImplementedException();
        }
    }
}
