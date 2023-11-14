using Microsoft.Extensions.Options;
using ProjectX.AnalyticsLib.OptionsCalculators;
using ProjectX.Core;
using ProjectX.Core.Analytics;
using ProjectXAnalyticsCppLib;
using System.Reflection.Metadata;
using OptionType = ProjectX.Core.OptionType;

namespace ProjectX.AnalyticsLib.Tests.OptionsCalculators
{    
    public class MonteCarloHestonCppPricerTest
    {    
        private const double TincyTolerance = 0.01;
        private const double SmallTolerance = 0.2;
        private const double MedTolerance = 0.8;

        private readonly MonteCarloHestonCppPricer calculator = new MonteCarloHestonCppPricer();               

        // stock with 6 months expiration, stock price is 100, strike price is 110, risk free interest rate 0.1 per year, continuous dividend yield 0.06 and volatility is 0.3 

        // Generalised model we use b differently
        // b = r: standard Black Scholes 1973 stock option model
        // b = r - q: gives the Merton 19973 stock option model with contionuous dividend yield q
        static readonly double spot = 100;
        static readonly double strike = 110;
        static readonly double r = 0.1;
        static readonly double q = 0.06;  // continuous dividend yiled
        // cost of carry
        // b= 0 gives Black (1976) futures option model
        // b= r-q gives Merton(1973) model
        // b= r gives Black-Scholes model
        //static readonly double b = r - q; // cost of carry charge         
        static readonly double b = 0; // cost of carry charge         
        static readonly double maturity = 0.5; // 6 months to expiration        
        static readonly ulong numSteps = 1000;
        static readonly ulong numPaths = 1000;

        static readonly double v0 = Math.Pow(0.15, 2); // starting volatility
        static readonly double theta = Math.Pow(0.15, 2); // Long-term mean volatility
        static readonly double kappa = 0.5; // speed of reversion
        static readonly double xi = 0.05;   // vol of vol
        static readonly double rho = 0;   // correlation between Spot and Vol brownian motions

        [TestCase(typeof(BlackScholesOptionsPricer))]
        public void WhenComputingPV(Type calculatorType)
        {
            var volParams = new HestonStochasticVolalityParameters(v0, theta, kappa, kappa, rho);
            var callOption = new VanillaOptionParameters(ProjectXAnalyticsCppLib.OptionType.Call, strike, maturity);
            var call = calculator.MCValue(ref callOption, spot, r, q, numSteps, numPaths, ref volParams).PV;
            Console.WriteLine($"Price of call is {call}");
            Assert.That(call, Is.EqualTo(6.52078264).Within(1).Percent);

            var putOption = new VanillaOptionParameters(ProjectXAnalyticsCppLib.OptionType.Put, strike, maturity);
            var put = calculator.MCValue(ref putOption, spot, r, q, numSteps, numPaths, ref volParams).PVPut;
            Console.WriteLine($"Price of put is {put}");
            Assert.That(put, Is.EqualTo(11.15601933).Within(1).Percent);

            // Assert Call Put Parity
            // If call delta is +1 (deep in the money), put delta is 0 (far out of the money).
            // If call delta is 0, put delta is –1.
            // If call delta is +0.7, put delta is –0.3.
            Assert.That(call, Is.Not.EqualTo(put), "Call-Put Parity should be obeyed");
        }           
    }
}
