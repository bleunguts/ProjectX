using Microsoft.Extensions.Options;
using ProjectX.Core;
using ProjectXAnalyticsCppLib;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using OptionType = ProjectX.Core.OptionType;

namespace ProjectX.AnalyticsLib.OptionsCalculators
{
    [Export(typeof(IMonteCarloCppOptionsPricer)), PartCreationPolicy(CreationPolicy.Shared)]
    public class MonteCarloCppOptionsPricerWrapper: IMonteCarloCppOptionsPricer
    {
        private readonly MonteCarloCppPricer _calculator;
        private readonly MonteCarloSimulationCache _cachedSimulation;
        private readonly ulong _numOfMcPaths;

        [ImportingConstructor]
        public MonteCarloCppOptionsPricerWrapper(IOptions<OptionsPricerCppWrapperOptions> options)
        {
            var algo = options?.Value?.RandomAlgo ?? RandomAlgorithm.BoxMuller;
            _numOfMcPaths = options?.Value?.NumOfMcPaths ?? 1000;
            _calculator = new MonteCarloCppPricer(new RandomWalk(algo));
            _cachedSimulation = new MonteCarloSimulationCache(_calculator);
        }        

        public double PV(OptionType optionType, double spot, double strike, double rate, double carry, double maturity, double volatility)
        {
            var key = Key(spot, strike, rate, carry, maturity, volatility);
            GreekResults greekResult= _cachedSimulation.RunSimulation(key, optionType, spot, strike, rate, maturity, volatility, _numOfMcPaths);
            return optionType == OptionType.Call ? greekResult.PV : greekResult.PVPut;
        }        

        public double Delta(OptionType optionType, double spot, double strike, double rate, double carry, double maturity, double volatility)
        {
            var key = Key(spot, strike, rate, carry, maturity, volatility);
            GreekResults greekResult = _cachedSimulation.RunSimulation(key, optionType, spot, strike, rate, maturity, volatility, _numOfMcPaths);
            return optionType == OptionType.Call ? greekResult.Delta : greekResult.DeltaPut;
        }

        public double Gamma(OptionType optionType, double spot, double strike, double rate, double carry, double maturity, double volatility)
        {
            var key = Key(spot, strike, rate, carry, maturity, volatility);
            GreekResults greekResult = _cachedSimulation.RunSimulation(key, optionType, spot, strike, rate, maturity, volatility, _numOfMcPaths);
            return greekResult.Gamma;
        }  

        public double Rho(OptionType optionType, double spot, double strike, double rate, double carry, double maturity, double volatility)
        {
            var key = Key(spot, strike, rate, carry, maturity, volatility);
            GreekResults greekResult = _cachedSimulation.RunSimulation(key, optionType, spot, strike, rate, maturity, volatility, _numOfMcPaths);
            return optionType == OptionType.Call ? greekResult.Rho : greekResult.RhoPut;
        }

        public double Theta(OptionType optionType, double spot, double strike, double rate, double carry, double maturity, double volatility)
        {
            var key = Key(spot, strike, rate, carry, maturity, volatility);
            GreekResults greekResult = _cachedSimulation.RunSimulation(key, optionType, spot, strike, rate, maturity, volatility, _numOfMcPaths);
            return optionType == OptionType.Call ? greekResult.Theta : greekResult.ThetaPut;
        }

        public double Vega(OptionType optionType, double spot, double strike, double rate, double carry, double maturity, double volatility)
        {
            var key = Key(spot, strike, rate, carry, maturity, volatility);
            GreekResults greekResult = _cachedSimulation.RunSimulation(key, optionType, spot, strike, rate, maturity, volatility, _numOfMcPaths);
            return greekResult.Vega;
        }
        public double ImpliedVol(OptionType optionType, double spot, double strike, double rate, double carry, double maturity, double price)
        {
            // Calculate implied volatility using Monte Carlo simulation
            var param = new VanillaOptionParameters(optionType.ToNativeOptionType(), strike, maturity);
            return _calculator.ImpliedVolatilityMC(ref param, spot, rate, _numOfMcPaths, price);
        }
        private static MonteCarloSimulationCache.ExecutionKey Key(double spot, double strike, double rate, double carry, double maturity, double volatility)
        {
            return MonteCarloSimulationCache.Key(spot, strike, rate, carry, maturity, volatility);
        }
    }
}