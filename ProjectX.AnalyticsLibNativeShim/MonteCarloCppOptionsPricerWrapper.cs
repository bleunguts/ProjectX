using ProjectX.AnalyticsLib.Shared;
using ProjectX.AnalyticsLibNativeShim.Interop;
using ProjectX.Core;
using System.ComponentModel.Composition;
using OptionType = ProjectX.Core.OptionType;

namespace ProjectX.AnalyticsLibNativeShim
{
    public class MonteCarloCppOptionsPricerWrapper : IMonteCarloCppOptionsPricer
    {
        private API _api;
        private ulong _numberOfPaths;
        
        public MonteCarloCppOptionsPricerWrapper()
        {
            _api = API.Instance;
            _numberOfPaths = 1000;
        }
        public double Delta(OptionType optionType, double spot, double strike, double rate, double carry, double maturity, double volatility)
            => RunSimulation(optionType, spot, rate, maturity, volatility).PV;

        private GreekResults RunSimulation(OptionType optionType, double spot, double rate, double maturity, double volatility)
        {
            return _api.MonteCarlo_PV(_api.ToOption(optionType, spot, maturity), spot, volatility, rate, _numberOfPaths);
        }

        public double Gamma(OptionType optionType, double spot, double strike, double rate, double carry, double maturity, double volatility)
            => RunSimulation(optionType, spot, rate, maturity, volatility).Gamma;

        public double ImpliedVol(OptionType optionType, double spot, double strike, double rate, double carry, double maturity, double price)
            => _api.MonteCarlo_ImpliedVolatility(_api.ToOption(optionType, spot, maturity), spot, rate, _numberOfPaths, price);

        public double PV(OptionType optionType, double spot, double strike, double rate, double carry, double maturity, double volatility)
            => RunSimulation(optionType, spot, rate, maturity, volatility).PV;

        public double Rho(OptionType optionType, double spot, double strike, double rate, double carry, double maturity, double volatility)
            => RunSimulation(optionType, spot, rate, maturity, volatility).Rho;

        public double Theta(OptionType optionType, double spot, double strike, double rate, double carry, double maturity, double volatility)
            => RunSimulation(optionType, spot, rate, maturity, volatility).Theta;

        public double Vega(OptionType optionType, double spot, double strike, double rate, double carry, double maturity, double volatility)
            => RunSimulation(optionType, spot, rate, maturity, volatility).Vega;
    }
}
