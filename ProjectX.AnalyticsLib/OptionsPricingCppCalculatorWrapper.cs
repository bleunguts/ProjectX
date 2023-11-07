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
    public interface ICPlusPlusBlackScholesOptionsPricingCalculator : ICSharpBlackScholesOptionsPricingCalculator
    {
    }
    [Export(typeof(ICPlusPlusBlackScholesOptionsPricingCalculator)), PartCreationPolicy(CreationPolicy.Shared)]
    public class OptionsPricingCppCalculatorWrapper : ICPlusPlusBlackScholesOptionsPricingCalculator
    {        
        private readonly OptionsPricingCppCalculator _calculator;
        private readonly ulong _numOfMcPaths;

        [ImportingConstructor]
        public OptionsPricingCppCalculatorWrapper(IOptions<OptionsPricingCppCalculatorWrapperOptions> options)
        {
            var algo = options?.Value?.RandomAlgo ?? RandomAlgorithm.BoxMuller;
            _calculator = new OptionsPricingCppCalculator(new RandomWalk(algo));
            _numOfMcPaths = options?.Value?.NumOfMcPaths ?? 10_000;
        }
        public double BlackScholes(OptionType optionType, double spot, double strike, double rate, double carry, double maturity, double volatility)
        {
            var param = new VanillaOptionParameters(ToCppOptionType(optionType), strike, maturity);
            var sw = Stopwatch.StartNew();
            var value = _calculator.MCValue(ref param, spot, volatility, rate, _numOfMcPaths);
            sw.Stop();
            Console.WriteLine($"BlackScholes with {_numOfMcPaths} paths took {sw.ElapsedMilliseconds} ms.");
            return value;
        }        

        public double BlackScholes_Delta(OptionType optionType, double spot, double strike, double rate, double carry, double maturity, double volatility)
        {            
            var param = new VanillaOptionParameters(ToCppOptionType(optionType), strike, maturity);
            return _calculator.DeltaMC(ref param, spot, volatility, rate, _numOfMcPaths);            
        }

        public double BlackScholes_Gamma(OptionType optionType, double spot, double strike, double rate, double carry, double maturity, double volatility)
        {            
            var param = new VanillaOptionParameters(ToCppOptionType(optionType), strike, maturity);
            return _calculator.GammaMC(ref param, spot, volatility, rate, _numOfMcPaths);
        }

        public double BlackScholes_ImpliedVol(OptionType optionType, double spot, double strike, double rate, double carry, double maturity, double price)
        {            
            // Calculate implied volatility using Monte Carlo simulation
            var param = new VanillaOptionParameters(ToCppOptionType(optionType), strike, maturity);
            return _calculator.ImpliedVolatilityMC(ref param, spot, rate, _numOfMcPaths, price);
        }

        public double BlackScholes_Rho(OptionType optionType, double spot, double strike, double rate, double carry, double maturity, double volatility)
        {
            var param = new VanillaOptionParameters(ToCppOptionType(optionType), strike, maturity);
            return _calculator.RhoMC(ref param, spot, volatility, rate, _numOfMcPaths);
        }

        public double BlackScholes_Theta(OptionType optionType, double spot, double strike, double rate, double carry, double maturity, double volatility)
        {
            double timeStep = 0.01;
            var param = new VanillaOptionParameters(ToCppOptionType(optionType), strike, maturity);
            //double theta = _calculator.Theta(ref param, spot, volatility, rate, _numOfMcPaths);
            double theta = _calculator.ThetaMC(ref param, spot, volatility, rate, _numOfMcPaths, timeStep);
            return double.IsNaN(theta) ? 0.0 : theta;   
        }

        public double BlackScholes_Vega(OptionType optionType, double spot, double strike, double rate, double carry, double maturity, double vol)
        {
            var param = new VanillaOptionParameters(ToCppOptionType(optionType), strike, maturity);
            return _calculator.VegaMC(ref param, spot, vol, rate, _numOfMcPaths);
        }

        static ProjectXAnalyticsCppLib.OptionType ToCppOptionType(OptionType optionType) => optionType switch
        {
            OptionType.Call => ProjectXAnalyticsCppLib.OptionType.Call,
            OptionType.Put => ProjectXAnalyticsCppLib.OptionType.Put,
            _ => throw new NotImplementedException($"{nameof(optionType)} not supported"),
        };
    }
}