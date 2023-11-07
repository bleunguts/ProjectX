using Microsoft.Extensions.Options;
using ProjectX.Core;
using ProjectXAnalyticsCppLib;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OptionType = ProjectX.Core.OptionType;

namespace ProjectX.AnalyticsLib
{
    public interface IMonteCarloOptionsPricingCppCalculator : IOptionsGreeksCalculator
    {
    }
    [Export(typeof(IMonteCarloOptionsPricingCppCalculator)), PartCreationPolicy(CreationPolicy.Shared)]
    public class OptionsPricingCppCalculatorWrapper : IMonteCarloOptionsPricingCppCalculator, IBlackScholesOptionsGreeksCalculator
    {        
        private readonly OptionsPricingCppCalculator _calculator;
        private readonly ulong _numOfMcPaths;

        [ImportingConstructor]
        public OptionsPricingCppCalculatorWrapper(IOptions<OptionsPricingCppCalculatorWrapperOptions> options)
        {
            var algo = options?.Value?.RandomAlgo ?? RandomAlgorithm.BoxMuller;
            _calculator = new OptionsPricingCppCalculator(new RandomWalk(algo));
            _numOfMcPaths = options?.Value?.NumOfMcPaths ?? 1000;
        }
        public double PV(OptionType optionType, double spot, double strike, double rate, double carry, double maturity, double volatility)
        {
            var param = new VanillaOptionParameters(ToNativeOptionType(optionType), strike, maturity);
            var sw = Stopwatch.StartNew();
            var value = _calculator.MCValue(ref param, spot, volatility, rate, _numOfMcPaths);
            sw.Stop();
            Console.WriteLine($"BlackScholes with {_numOfMcPaths} paths took {sw.ElapsedMilliseconds} ms.");
            return value;
        }        

        public double Delta(OptionType optionType, double spot, double strike, double rate, double carry, double maturity, double volatility)
        {            
            var param = new VanillaOptionParameters(ToNativeOptionType(optionType), strike, maturity);
            return _calculator.DeltaMC(ref param, spot, volatility, rate, _numOfMcPaths);            
        }

        public double Gamma(OptionType optionType, double spot, double strike, double rate, double carry, double maturity, double volatility)
        {            
            var param = new VanillaOptionParameters(ToNativeOptionType(optionType), strike, maturity);
            return _calculator.GammaMC(ref param, spot, volatility, rate, _numOfMcPaths);
        }

        public double ImpliedVol(OptionType optionType, double spot, double strike, double rate, double carry, double maturity, double price)
        {            
            // Calculate implied volatility using Monte Carlo simulation
            var param = new VanillaOptionParameters(ToNativeOptionType(optionType), strike, maturity);
            return _calculator.ImpliedVolatilityMC(ref param, spot, rate, _numOfMcPaths, price);
        }

        public double Rho(OptionType optionType, double spot, double strike, double rate, double carry, double maturity, double volatility)
        {
            var param = new VanillaOptionParameters(ToNativeOptionType(optionType), strike, maturity);
            return _calculator.RhoMC(ref param, spot, volatility, rate, _numOfMcPaths);
        }

        public double Theta(OptionType optionType, double spot, double strike, double rate, double carry, double maturity, double volatility)
        {
            double timeStep = 0.01;
            var param = new VanillaOptionParameters(ToNativeOptionType(optionType), strike, maturity);
            //double theta = _calculator.Theta(ref param, spot, volatility, rate, _numOfMcPaths);
            double theta = _calculator.ThetaMC(ref param, spot, volatility, rate, _numOfMcPaths, timeStep);
            return double.IsNaN(theta) ? 0.0 : theta;   
        }

        public double Vega(OptionType optionType, double spot, double strike, double rate, double carry, double maturity, double volatility)
        {
            var param = new VanillaOptionParameters(ToNativeOptionType(optionType), strike, maturity);
            return _calculator.VegaMC(ref param, spot, volatility, rate, _numOfMcPaths);
        }

        static ProjectXAnalyticsCppLib.OptionType ToNativeOptionType(OptionType optionType) => optionType switch
        {
            OptionType.Call => ProjectXAnalyticsCppLib.OptionType.Call,
            OptionType.Put => ProjectXAnalyticsCppLib.OptionType.Put,
            _ => throw new NotImplementedException($"{nameof(optionType)} not supported"),
        };

        double IBlackScholesOptionsGreeksCalculator.BlackScholes_PV(OptionType optionType, double spot, double strike, double rate, double carry, double maturity, double volatility)
        {
            var param = new VanillaOptionParameters(ToNativeOptionType(optionType), strike, maturity);
            return _calculator.Value(ref param, spot, volatility, rate);
        }

        double IBlackScholesOptionsGreeksCalculator.BlackScholes_Delta(OptionType optionType, double spot, double strike, double rate, double carry, double maturity, double volatility)
        {
            var param = new VanillaOptionParameters(ToNativeOptionType(optionType), strike, maturity);
            return _calculator.Delta(ref param, spot, volatility, rate, _numOfMcPaths);
        }

        double IBlackScholesOptionsGreeksCalculator.BlackScholes_Gamma(OptionType optionType, double spot, double strike, double rate, double carry, double maturity, double volatility)
        {
            var param = new VanillaOptionParameters(ToNativeOptionType(optionType), strike, maturity);
            return _calculator.Gamma(ref param, spot, volatility, rate, _numOfMcPaths);
        }

        double IBlackScholesOptionsGreeksCalculator.BlackScholes_ImpliedVol(OptionType optionType, double spot, double strike, double rate, double carry, double maturity, double price)
        {
            var param = new VanillaOptionParameters(ToNativeOptionType(optionType), strike, maturity);            
            return _calculator.ImpliedVolatility(ref param, spot, rate, _numOfMcPaths, price);
        }

        double IBlackScholesOptionsGreeksCalculator.BlackScholes_Rho(OptionType optionType, double spot, double strike, double rate, double carry, double maturity, double volatility)
        {
            var param = new VanillaOptionParameters(ToNativeOptionType(optionType), strike, maturity);
            return _calculator.Rho(ref param, spot, volatility, rate, _numOfMcPaths);
        }

        double IBlackScholesOptionsGreeksCalculator.BlackScholes_Theta(OptionType optionType, double spot, double strike, double rate, double carry, double maturity, double volatility)
        {
            var param = new VanillaOptionParameters(ToNativeOptionType(optionType), strike, maturity);
            return _calculator.Theta(ref param, spot, volatility, rate, _numOfMcPaths);
        }

        double IBlackScholesOptionsGreeksCalculator.BlackScholes_Vega(OptionType optionType, double spot, double strike, double rate, double carry, double maturity, double volatility)
        {
            var param = new VanillaOptionParameters(ToNativeOptionType(optionType), strike, maturity);
            return _calculator.Vega(ref param, spot, volatility, rate, _numOfMcPaths);
        }
    }
}