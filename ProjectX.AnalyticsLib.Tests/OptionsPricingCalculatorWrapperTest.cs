using ProjectXAnalyticsCppLib;
using System.Diagnostics;

namespace ProjectX.AnalyticsLib.Tests;

public class OptionsPricingCalculatorWrapperTest
{
    private OptionsPricingCppCalculatorWrapper _calculator = new OptionsPricingCppCalculatorWrapper();

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

    [Test]
    public void WhenPricingACallOptionShouldBehaveSameAsCSharpVersion()
    {
        // For a call option that is deep ITM price is gt 0 should be expensive
        var deepItmPrice = _calculator.BlackScholes(Core.OptionType.Call, 510, 100, 0.1, 0.04, 2.0, 0.3);
        Assert.Greater(deepItmPrice, 0);
        Assert.That(deepItmPrice, Is.EqualTo(370.4568).Within(1).Percent);

        // For a call option that is ITM price is gt 0 should be relative expensive
        var itmPrice = _calculator.BlackScholes(Core.OptionType.Call, 110, 100, 0.1, 0.04, 2.0, 0.3);
        Assert.Greater(itmPrice, 0);
        Assert.That(itmPrice, Is.EqualTo(24.1620).Within(1).Percent);

        // For a call option that is ATM price is gt 0 should be fair priced
        var atmPrice = _calculator.BlackScholes(Core.OptionType.Call, 100, 100, 0.1, 0.04, 2.0, 0.3);
        Assert.Greater(atmPrice, 0);
        Assert.That(atmPrice, Is.EqualTo(17.9866).Within(1).Percent);

        // For a call option that is OTM price is cheaper
        var otmPrice = _calculator.BlackScholes(Core.OptionType.Call, 70, 100, 0.1, 0.04, 2.0, 0.3);
        Assert.Greater(otmPrice, 0);
        Assert.That(otmPrice, Is.EqualTo(4.6253).Within(1).Percent);

        // For a call option that is Deep OTM price is worthless
        var deepOtmPrice = _calculator.BlackScholes(Core.OptionType.Call, 2, 100, 0.1, 0.04, 2.0, 0.3);
        Assert.AreEqual(deepOtmPrice, 0);
        Assert.That(deepOtmPrice, Is.EqualTo(0).Within(1).Percent);
    }
}