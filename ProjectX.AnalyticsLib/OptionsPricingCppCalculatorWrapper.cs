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

        public OptionsPricingCppCalculatorWrapper(ulong numOfMcPaths = 1000)
        {
            _calculator = new OptionsPricingCppCalculator(new RandomWalk(RandomAlgorithm.BoxMuller));
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

        private static ProjectXAnalyticsCppLib.OptionType ToCppOptionType(OptionType optionType)
        {
            switch (optionType)
            {
                case OptionType.Call: return ProjectXAnalyticsCppLib.OptionType.Call;
                case OptionType.Put: return ProjectXAnalyticsCppLib.OptionType.Put;
            }
            throw new NotImplementedException($"{nameof(optionType)} not supported");
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
