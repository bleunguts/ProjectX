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
        private readonly MonteCarloHestonCppPricer calculator = new MonteCarloHestonCppPricer();                       

        // Protect against fuel prices by buying Long Call
        // 2009 fuel price was $1.9
        // 2019 rocketed to $2.97 would of lost 1.44 mio with notional of 2 mio
        // 2019 $1.39

        static readonly double spot = 2.0;   // $2 
        static readonly double strike = 2.0; // $2
        static readonly double r = 0.0319; // 3.19 %
        static readonly double q = 0;          
        static readonly double T = 1.0; // 1 year to expiry
        static readonly ulong n_TimeSteps = 200;
        static readonly ulong m_Simulations = 200;

        static readonly double v0 = 0.010201; // starting volatility
        static readonly double theta = 0.019; // Long-term mean volatility
        static readonly double kappa = 6.21; // speed of reversion
        static readonly double sigma = 0.61;   // vol of vol
        static readonly double rho = -0.9;   // correlation between Spot and Vol brownian motions

        [TestCase(typeof(BlackScholesOptionsPricer))]
        public void WhenComputingPV(Type calculatorType)
        {
            var volParams = new HestonStochasticVolalityParameters(v0, theta, kappa, sigma, rho);
            var callOption = new VanillaOptionParameters(ProjectXAnalyticsCppLib.OptionType.Call, strike, T);
            var result = calculator.MCValue(ref callOption, spot, r, q, n_TimeSteps, m_Simulations, ref volParams);
            var call = result.PV;
            var put = result.PVPut;
            Console.WriteLine($"Price of call is {call}");
            Console.WriteLine($"Price of put is {put}");
            Console.WriteLine("DEBUG:");
            Console.WriteLine($"callsCount={result.Debug.Value.callsCount} putsCount={result.Debug.Value.callsCount} sims={result.Debug.Value.callsCount}");
            Console.WriteLine($"Rhos used=[{string.Join(",", result.Debug.Value.rhos.Distinct())}] for {result.Debug.Value.rhos.Count} calc paths");            
            Assert.That(call, Is.EqualTo(0.1421).Within(0.2));            
            Assert.That(put, Is.EqualTo(0.075).Within(0.08));            

            // Assert Call Put Parity
            // If call delta is +1 (deep in the money), put delta is 0 (far out of the money).
            // If call delta is 0, put delta is –1.
            // If call delta is +0.7, put delta is –0.3.
            Assert.That(call, Is.Not.EqualTo(put), "Call-Put Parity should be obeyed");
        }           
    }
}
