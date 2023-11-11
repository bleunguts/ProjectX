﻿using Microsoft.Extensions.Options;
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
    public class MonteCarloCppOptionsPricerWrapper : IMonteCarloCppOptionsPricer
    {
        private readonly MonteCarloCppPricer _calculator;
        private readonly BlackScholesCppPricer _blackScholes;
        private readonly ulong _numOfMcPaths;
        private readonly Dictionary<ExecutionKey, GreekResults> _cache = new();

        [ImportingConstructor]
        public MonteCarloCppOptionsPricerWrapper(IOptions<OptionsPricerCppWrapperOptions> options)
        {
            var algo = options?.Value?.RandomAlgo ?? RandomAlgorithm.BoxMuller;
            _calculator = new MonteCarloCppPricer(new RandomWalk(algo));
            _numOfMcPaths = options?.Value?.NumOfMcPaths ?? 1000;
            _blackScholes = new BlackScholesCppPricer();
        }

        private GreekResults RunSimulation(ExecutionKey key, OptionType optionType, double spot, double strike, double rate, double maturity, double volatility)
        {
            if (!_cache.TryGetValue(key, out GreekResults? greekResult))
            {
                var param = new VanillaOptionParameters(optionType.ToNativeOptionType(), strike, maturity);
                var result = _calculator.MCValue(ref param, spot, volatility, rate, _numOfMcPaths);

                _cache.Add(key, result);
                greekResult = result;
            }
            return greekResult;
        }

        public double PV(OptionType optionType, double spot, double strike, double rate, double carry, double maturity, double volatility)
        {
            var key = Key(optionType, spot, strike, rate, carry, maturity, volatility);
            GreekResults greekResult= RunSimulation(key, optionType, spot, strike, rate, maturity, volatility);
            return greekResult.PV;
        }        

        public double Delta(OptionType optionType, double spot, double strike, double rate, double carry, double maturity, double volatility)
        {
            var key = Key(optionType, spot, strike, rate, carry, maturity, volatility);
            GreekResults greekResult = RunSimulation(key, optionType, spot, strike, rate, maturity, volatility);
            return greekResult.Delta;
        }

        public double Gamma(OptionType optionType, double spot, double strike, double rate, double carry, double maturity, double volatility)
        {
            var key = Key(optionType, spot, strike, rate, carry, maturity, volatility);
            GreekResults greekResult = RunSimulation(key, optionType, spot, strike, rate, maturity, volatility);
            return greekResult.Gamma;
        }

        public double ImpliedVol(OptionType optionType, double spot, double strike, double rate, double carry, double maturity, double price)
        {
            // Calculate implied volatility using Monte Carlo simulation
            var param = new VanillaOptionParameters(optionType.ToNativeOptionType(), strike, maturity);
            return _calculator.ImpliedVolatilityMC(ref param, spot, rate, _numOfMcPaths, price);
        }

        public double Rho(OptionType optionType, double spot, double strike, double rate, double carry, double maturity, double volatility)
        {
            var key = Key(optionType, spot, strike, rate, carry, maturity, volatility);
            GreekResults greekResult = RunSimulation(key, optionType, spot, strike, rate, maturity, volatility);
            return greekResult.Rho;
        }

        public double Theta(OptionType optionType, double spot, double strike, double rate, double carry, double maturity, double volatility)
        {
            var key = Key(optionType, spot, strike, rate, carry, maturity, volatility);
            GreekResults greekResult = RunSimulation(key, optionType, spot, strike, rate, maturity, volatility);
            return greekResult.Theta;
        }

        public double Vega(OptionType optionType, double spot, double strike, double rate, double carry, double maturity, double volatility)
        {
            var key = Key(optionType, spot, strike, rate, carry, maturity, volatility);
            GreekResults greekResult = RunSimulation(key, optionType, spot, strike, rate, maturity, volatility);
            return greekResult.Vega;
        }

        private static ExecutionKey Key(OptionType optionType, double spot, double strike, double rate, double carry, double maturity, double volatility)
        {
            return new ExecutionKey(optionType, spot, strike, rate, carry, maturity, volatility);
        }

        class ExecutionKey
        {
            public ExecutionKey(OptionType optionType, double spot, double strike, double rate, double carry, double maturity, double volatility)
            {
                OptionType = optionType;
                Spot = spot;
                Strike = strike;
                Rate = rate;
                Carry = carry;
                Maturity = maturity;
                Volatility = volatility;
            }

            public OptionType OptionType { get; }
            public double Spot { get; }
            public double Strike { get; }
            public double Rate { get; }
            public double Carry { get; }
            public double Maturity { get; }
            public double Volatility { get; }
        }
    }
}