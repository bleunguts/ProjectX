using ProjectXAnalyticsCppLib;
using System.Diagnostics;

namespace ProjectX.AnalyticsLib.Tests;

public class AnalyticsCppOptionsPricingCalculatorTest
{
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
        var calculator = new OptionsPricingCalculator();

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
}