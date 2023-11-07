using ProjectX.Core;
using ProjectXAnalyticsCppLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OptionType = ProjectX.Core.OptionType;

namespace ProjectX.AnalyticsLib
{
    public class OptionsPricingCppCalculatorWrapper : IBlackScholesOptionsPricingCalculator
    {        
        private readonly OptionsPricingCppCalculator _calculator;
        private readonly ulong _numOfMcPaths;

        public OptionsPricingCppCalculatorWrapper(ulong numOfMcPaths = 1000, RandomAlgorithm algo = RandomAlgorithm.BoxMuller)
        {
            _calculator = new OptionsPricingCppCalculator(new RandomWalk(algo));
            _numOfMcPaths = numOfMcPaths;
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
            double epsilon = 0.01;
            var param = new VanillaOptionParameters(ToCppOptionType(optionType), strike, maturity);
            var delta = _calculator.Delta(ref param, spot, volatility, rate, epsilon, _numOfMcPaths);
            return delta;
        }

        public double BlackScholes_Gamma(double spot, double strike, double rate, double carry, double maturity, double volatility)
        {
            double epsilon = 0.01;
            var param = new VanillaOptionParameters(ToCppOptionType(OptionType.Call), strike, maturity);
            return _calculator.Gamma(ref param, spot, volatility, rate, epsilon, _numOfMcPaths);
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

        static ProjectXAnalyticsCppLib.OptionType ToCppOptionType(OptionType optionType) => optionType switch
        {
            OptionType.Call => ProjectXAnalyticsCppLib.OptionType.Call,
            OptionType.Put => ProjectXAnalyticsCppLib.OptionType.Put,
            _ => throw new NotImplementedException($"{nameof(optionType)} not supported"),
        };
    }
}