using ProjectX.AnalyticsLib.Shared;
using ProjectX.AnalyticsLibNativeShim.Interop;
using ProjectX.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OptionType = ProjectX.Core.OptionType;

namespace ProjectX.AnalyticsLibNativeShim
{
    public class BlackScholesCppOptionsPricerWrapper : IBlackScholesCppPricer
    {
        private API _api;
        private const double epsilon = 1e-6;
        
        public BlackScholesCppOptionsPricerWrapper() 
        {
            _api = API.Instance;
        }

        public double Delta(OptionType optionType, double spot, double strike, double rate, double carry, double maturity, double volatility) 
            => _api.BlackScholes_Delta(_api.ToOption(optionType, strike, maturity), spot, volatility, rate);        

        public double Gamma(OptionType optionType, double spot, double strike, double rate, double carry, double maturity, double volatility)
            => _api.BlackScholes_Gamma(_api.ToOption(optionType, strike, maturity), spot, volatility, rate, epsilon);

        public double ImpliedVol(OptionType optionType, double spot, double strike, double rate, double carry, double maturity, double price)
            => _api.BlackScholes_ImpliedVolatility(_api.ToOption(optionType, strike, maturity), spot, rate, price);
            
        public double PV(OptionType optionType, double spot, double strike, double rate, double carry, double maturity, double volatility) => 
            _api.BlackScholes_PV(_api.ToOption(optionType, strike, maturity), spot, volatility, rate);

        public double Rho(OptionType optionType, double spot, double strike, double rate, double carry, double maturity, double volatility) =>
            _api.BlackScholes_Rho(_api.ToOption(optionType, strike, maturity), spot, volatility, rate);
        
        public double Theta(OptionType optionType, double spot, double strike, double rate, double carry, double maturity, double volatility) =>
            _api.BlackScholes_Theta(_api.ToOption(optionType, strike, maturity), spot, volatility, rate);

        public double Vega(OptionType optionType, double spot, double strike, double rate, double carry, double maturity, double volatility) =>
            _api.BlackScholes_Vega(_api.ToOption(optionType, strike, maturity), spot, volatility, rate);
    }
}
