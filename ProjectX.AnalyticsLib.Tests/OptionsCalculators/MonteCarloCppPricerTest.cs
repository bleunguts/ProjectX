using ProjectXAnalyticsCppLib;
using System.Diagnostics;

namespace ProjectX.AnalyticsLib.Tests.OptionsCalculators;

public class MonteCarloCppPricerTest
{
    MonteCarloCppPricer mc = new(new RandomWalk(RandomAlgorithm.BoxMuller));   

    // real numbers example http://financetrain.com/option-pricing-using-monte-carlo-simulation/
    // expiry = 0.25 (time to expire)
    // strike = 200 
    // spot = 195
    // vol = 0.30 
    // r = 0.05 (risk free rate)
    // n = 1000 paths
    // Result 10.5126 / 12.739
    [Test]
    public void WhenValueShouldReturnValidPV()
    {
        VanillaOptionParameters theOption = new(OptionType.Call, 200.0, 0.25);
        double spot = 195.0;
        double vol = 0.30;
        double r = 0.05;
        uint numberOfPaths = 250_000;
        var sw = Stopwatch.StartNew();
        var pv = mc.MCValue(ref theOption, spot, vol, r, numberOfPaths);        
        sw.Stop();        
        Console.WriteLine($"Completed {numberOfPaths} #MC paths in {sw.ElapsedMilliseconds} ms");

        Assert.That(pv, Is.EqualTo(10.5).Within(1).Percent);        
    }

    [Test]
    [Ignore("TODO FIX")]
    public void ShallBeAbleToThetaMCOnAnOption()
    {
        VanillaOptionParameters theOption = new(OptionType.Call, 200.0, 0.95);
        double spot = 195.0;
        double vol = 0.30;
        double r = 0.05;
        uint numberOfPaths = 1000;

        double thetaMC = mc.ThetaMC(ref theOption, spot, vol, r, numberOfPaths, 0.01);
        Assert.That(thetaMC, Is.LessThan(0));
        Assert.That(double.IsRealNumber(thetaMC), Is.True);        
    }

}
