using ProjectXAnalyticsCppLib;
using System.Diagnostics;

namespace ProjectX.AnalyticsLib.Tests;

public class OptionsPricingCalculatorWrapperTest
{
    static IEnumerable<(IBlackScholesOptionsPricingCalculator calculator, double percentError)> VanillaOptionCalculators()
    {
        yield return (new BlackScholesOptionsPricingCalculator(), 42.0);
        yield return (new OptionsPricingCppCalculatorWrapper(200_000, RandomAlgorithm.BoxMuller), 1.3);
    }

    // real numbers example http://financetrain.com/option-pricing-using-monte-carlo-simulation/
    // expiry = 0.25 (time to expire)
    // strike = 200 
    // spot = 195
    // vol = 0.30 
    // r = 0.05 (risk free rate)
    // n = 1000 paths
    // Result 10.5126 / 12.739
    [Test]
    public void ShallBeAbleToPriceOptionWithCppOptionsPricingCalculator()
    {
        var calculator = new OptionsPricingCppCalculator(new RandomWalk(RandomAlgorithm.BoxMuller));

        VanillaOptionParameters theOption = new(OptionType.Call, 200.0, 0.25);
        double spot = 195.0;
        double vol = 0.30;
        double r = 0.05;
        uint numberOfPaths = 1000;
        var sw = Stopwatch.StartNew();
        {
            double price = calculator.MCValue(ref theOption, spot, vol, r, numberOfPaths);
            Assert.That(price, Is.EqualTo(10.5).Within(1).Percent);
            sw.Stop();
            Console.WriteLine($"Completed {numberOfPaths} #MC paths in {sw.ElapsedMilliseconds} ms");
        }
    }
    
    [TestCaseSource(nameof(VanillaOptionCalculators))]
    public void WhenPricingACallOptionRealWorldExampleShouldReturnSimilarValuesToCSharpVersion((IBlackScholesOptionsPricingCalculator calculator, double percentError) td)
    {
        // Based on real number example 
        var realExamplePrice = td.calculator.BlackScholes(Core.OptionType.Call, 195.0, 200.0, 0.05, 0.0, 0.25, 0.3);
        Assert.That(realExamplePrice, Is.EqualTo(10.5).Within(td.percentError).Percent);
    }
    
    [TestCaseSource(nameof(VanillaOptionCalculators))]
    public void WhenPricingACallOptionShouldBehaveSameAsCSharpVersion((IBlackScholesOptionsPricingCalculator calculator, double percentError) td)
    {        
        // For a call option that is deep ITM price is gt 0 should be expensive
        var deepItmPrice = td.calculator.BlackScholes(Core.OptionType.Call, 510, 100, 0.1, 0.04, 2.0, 0.3);
        Assert.That(deepItmPrice, Is.EqualTo(428.751).Within(td.percentError).Percent);

        // For a call option that is ITM price is gt 0 should be relative expensive
        var itmPrice = td.calculator.BlackScholes(Core.OptionType.Call, 110, 100, 0.1, 0.04, 2.0, 0.3);
        Assert.That(itmPrice, Is.EqualTo(33.9755).Within(td.percentError).Percent);

        // For a call option that is ATM price is gt 0 should be fair priced
        var atmPrice = td.calculator.BlackScholes(Core.OptionType.Call, 100, 100, 0.1, 0.04, 2.0, 0.3);
        Assert.That(atmPrice, Is.EqualTo(26.0246).Within(td.percentError).Percent);

        // For a call option that is OTM price is cheaper
        var otmPrice = td.calculator.BlackScholes(Core.OptionType.Call, 70, 100, 0.1, 0.04, 2.0, 0.3);
        Assert.That(otmPrice, Is.EqualTo(7.75053).Within(td.percentError).Percent);

        // For a call option that is Deep OTM price is worthless
        var deepOtmPrice = td.calculator.BlackScholes(Core.OptionType.Call, 2, 100, 0.1, 0.04, 2.0, 0.3);        
        Assert.That(deepOtmPrice, Is.EqualTo(0).Within(td.percentError).Percent);
    }
}