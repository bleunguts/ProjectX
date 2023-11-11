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
    public class MonteCarloCppOptionsPricerWrapper : IMonteCarloCppOptionsPricer
    {
        private readonly MonteCarloCppPricer _calculator;
        private readonly BlackScholesCppPricer _blackScholes;
        private readonly ulong _numOfMcPaths;

        [ImportingConstructor]
        public MonteCarloCppOptionsPricerWrapper(IOptions<OptionsPricerCppWrapperOptions> options)
        {
            var algo = options?.Value?.RandomAlgo ?? RandomAlgorithm.BoxMuller;
            _calculator = new MonteCarloCppPricer(new RandomWalk(algo));
            _numOfMcPaths = options?.Value?.NumOfMcPaths ?? 1000;
            _blackScholes = new BlackScholesCppPricer();
        }
        public double PV(OptionType optionType, double spot, double strike, double rate, double carry, double maturity, double volatility)
        {
            var param = new VanillaOptionParameters(optionType.ToNativeOptionType(), strike, maturity);
            var sw = Stopwatch.StartNew();
            var result = _calculator.MCValue(ref param, spot, volatility, rate, _numOfMcPaths);
            sw.Stop();
            return result.PV;
        }

        public double Delta(OptionType optionType, double spot, double strike, double rate, double carry, double maturity, double volatility)
        {
            var param = new VanillaOptionParameters(optionType.ToNativeOptionType(), strike, maturity);
            return _calculator.DeltaMC(ref param, spot, volatility, rate, _numOfMcPaths);
        }

        public double Gamma(OptionType optionType, double spot, double strike, double rate, double carry, double maturity, double volatility)
        {
            var param = new VanillaOptionParameters(optionType.ToNativeOptionType(), strike, maturity);
            return _calculator.GammaMC(ref param, spot, volatility, rate, _numOfMcPaths);
        }

        public double ImpliedVol(OptionType optionType, double spot, double strike, double rate, double carry, double maturity, double price)
        {
            // Calculate implied volatility using Monte Carlo simulation
            var param = new VanillaOptionParameters(optionType.ToNativeOptionType(), strike, maturity);
            return _calculator.ImpliedVolatilityMC(ref param, spot, rate, _numOfMcPaths, price);
        }

        public double Rho(OptionType optionType, double spot, double strike, double rate, double carry, double maturity, double volatility)
        {
            var param = new VanillaOptionParameters(optionType.ToNativeOptionType(), strike, maturity);
            return _calculator.RhoMC(ref param, spot, volatility, rate, _numOfMcPaths);
        }

        public double Theta(OptionType optionType, double spot, double strike, double rate, double carry, double maturity, double volatility)
        {
            double timeStep = 0.01;
            var param = new VanillaOptionParameters(optionType.ToNativeOptionType(), strike, maturity);
            //double theta = _calculator.Theta(ref param, spot, volatility, rate, _numOfMcPaths);
            double theta = _calculator.ThetaMC(ref param, spot, volatility, rate, _numOfMcPaths, timeStep);
            return double.IsNaN(theta) ? 0.0 : theta;
        }

        public double Vega(OptionType optionType, double spot, double strike, double rate, double carry, double maturity, double volatility)
        {
            var param = new VanillaOptionParameters(optionType.ToNativeOptionType(), strike, maturity);
            return _calculator.VegaMC(ref param, spot, volatility, rate, _numOfMcPaths);
        }      
    }
}