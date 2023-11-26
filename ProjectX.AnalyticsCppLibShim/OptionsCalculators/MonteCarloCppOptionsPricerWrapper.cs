using Microsoft.Extensions.Options;
using ProjectX.AnalyticsCppLibShim;
using ProjectX.AnalyticsLib.Shared;
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
    [Export(typeof(IMonteCarloHestonCppOptionsPricer)), PartCreationPolicy(CreationPolicy.Shared)]
    public class MonteCarloHestonCppOptionsPricerWrapper : IMonteCarloHestonCppOptionsPricer
    {
        private readonly MonteCarloHestonCppPricer2 _calculator;
        private readonly ulong _numOfMcPaths;
        private readonly ulong _numOfSteps;

        [ImportingConstructor]
        public MonteCarloHestonCppOptionsPricerWrapper(IOptions<HestonOptionsPricerCppWrapperOptions> options)
        {
            _calculator = new MonteCarloHestonCppPricer2();
            _numOfMcPaths = options?.Value?.NumOfMcPaths ?? 1000;
            _numOfSteps = options?.Value?.NumOfSteps ?? 1000;
        }

        public double PV(OptionType optionType, double spot, double strike, double rate, double carry, double maturity, double volatility)
        {                              
            double rho = -0.7;
            double volOfVol = Math.Sqrt(volatility);
            double longTermVolatility = volatility;
            double speedOfReversion = 6.21;

            var param = new VanillaOptionParameters(optionType.ToNativeOptionType(), strike, maturity);            
            var volParams = new HestonStochasticVolalityParameters(volatility, longTermVolatility, speedOfReversion, volOfVol, rho); 
            GreekResults results = _calculator.MCValue(ref param, spot, rate, carry, _numOfSteps, _numOfMcPaths, ref volParams);
            
            return optionType == OptionType.Call ? results.PV : results.PVPut;
        }

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

    [Export(typeof(IMonteCarloCppOptionsPricer)), PartCreationPolicy(CreationPolicy.Shared)]
    public class MonteCarloCppOptionsPricerWrapper: IMonteCarloCppOptionsPricer
    {
        private readonly MonteCarloCppPricer _calculator;
        private readonly MonteCarloCppSimulationCache _cachedSimulation;
        private readonly ulong _numOfMcPaths;

        [ImportingConstructor]
        public MonteCarloCppOptionsPricerWrapper(IOptions<OptionsPricerCppWrapperOptions> options)
        {
            var algo = options?.Value?.RandomAlgo ?? RandomAlgorithm.BoxMuller;
            _numOfMcPaths = options?.Value?.NumOfMcPaths ?? 1000;
            _calculator = new MonteCarloCppPricer(new RandomWalk(algo));
            _cachedSimulation = new MonteCarloCppSimulationCache(_calculator);
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
            return optionType == OptionType.Call ? greekResult.Delta!.Value : greekResult.DeltaPut!.Value;
        }

        public double Gamma(OptionType optionType, double spot, double strike, double rate, double carry, double maturity, double volatility)
        {
            var key = Key(spot, strike, rate, carry, maturity, volatility);
            GreekResults greekResult = _cachedSimulation.RunSimulation(key, optionType, spot, strike, rate, maturity, volatility, _numOfMcPaths);
            return greekResult.Gamma!.Value;
        }  

        public double Rho(OptionType optionType, double spot, double strike, double rate, double carry, double maturity, double volatility)
        {
            var key = Key(spot, strike, rate, carry, maturity, volatility);
            GreekResults greekResult = _cachedSimulation.RunSimulation(key, optionType, spot, strike, rate, maturity, volatility, _numOfMcPaths);
            return optionType == OptionType.Call ? greekResult.Rho!.Value: greekResult.RhoPut!.Value;
        }

        public double Theta(OptionType optionType, double spot, double strike, double rate, double carry, double maturity, double volatility)
        {
            var key = Key(spot, strike, rate, carry, maturity, volatility);
            GreekResults greekResult = _cachedSimulation.RunSimulation(key, optionType, spot, strike, rate, maturity, volatility, _numOfMcPaths);
            return optionType == OptionType.Call ? greekResult.Theta!.Value : greekResult.ThetaPut!.Value;
        }

        public double Vega(OptionType optionType, double spot, double strike, double rate, double carry, double maturity, double volatility)
        {
            var key = Key(spot, strike, rate, carry, maturity, volatility);
            GreekResults greekResult = _cachedSimulation.RunSimulation(key, optionType, spot, strike, rate, maturity, volatility, _numOfMcPaths);
            return greekResult.Vega!.Value;
        }
        public double ImpliedVol(OptionType optionType, double spot, double strike, double rate, double carry, double maturity, double price)
        {
            // Calculate implied volatility using Monte Carlo simulation
            var param = new VanillaOptionParameters(optionType.ToNativeOptionType(), strike, maturity);
            return _calculator.ImpliedVolatilityMC(ref param, spot, rate, _numOfMcPaths, price);
        }
        private static MonteCarloCppSimulationCache.ExecutionKey Key(double spot, double strike, double rate, double carry, double maturity, double volatility)
        {
            return MonteCarloCppSimulationCache.Key(spot, strike, rate, carry, maturity, volatility);
        }
    }
}