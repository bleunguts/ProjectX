using Microsoft.Extensions.Options;

namespace ProjectX.AnalyticsLib.Tests;

public class OptionsPricingCppCalculatorWrapperTest
{
    static IOptions<OptionsPricingCppCalculatorWrapperOptions> options = Options.Create<OptionsPricingCppCalculatorWrapperOptions>(new OptionsPricingCppCalculatorWrapperOptions() {  NumOfMcPaths = 250_000, RandomAlgo = RandomAlgorithm.BoxMuller });
    static IEnumerable<(IOptionsGreeksCalculator calculator, double percentError)> VanillaOptionCalculators()
    {
        yield return (new BlackScholesOptionsPricingCalculator(), 42.0);
        yield return (new OptionsPricingCppCalculatorWrapper(options), 1.75);
    }
    
    [TestCaseSource(nameof(VanillaOptionCalculators))]
    public void WhenPricingACallOptionRealWorldExampleShouldReturnSimilarValuesToCSharpVersion((IOptionsGreeksCalculator calculator, double percentError) td)
    {
        // Based on real number example 
        var realExamplePrice = td.calculator.PV(Core.OptionType.Call, 195.0, 200.0, 0.05, 0.0, 0.25, 0.3);
        Assert.That(realExamplePrice, Is.EqualTo(10.5).Within(td.percentError).Percent);
    }
    
    [TestCaseSource(nameof(VanillaOptionCalculators))]
    public void WhenPricingACallOptionShouldBehaveSameAsCSharpVersion((IOptionsGreeksCalculator calculator, double percentError) td)
    {        
        // For a call option that is deep ITM price is gt 0 should be expensive
        var deepItmPrice = td.calculator.PV(Core.OptionType.Call, 510, 100, 0.1, 0.04, 2.0, 0.3);
        Assert.That(deepItmPrice, Is.EqualTo(428.751).Within(td.percentError).Percent);

        // For a call option that is ITM price is gt 0 should be relative expensive
        var itmPrice = td.calculator.PV(Core.OptionType.Call, 110, 100, 0.1, 0.04, 2.0, 0.3);
        Assert.That(itmPrice, Is.EqualTo(33.9755).Within(td.percentError).Percent);

        // For a call option that is ATM price is gt 0 should be fair priced
        var atmPrice = td.calculator.PV(Core.OptionType.Call, 100, 100, 0.1, 0.04, 2.0, 0.3);
        Assert.That(atmPrice, Is.EqualTo(26.0246).Within(td.percentError).Percent);

        // For a call option that is OTM price is cheaper
        var otmPrice = td.calculator.PV(Core.OptionType.Call, 70, 100, 0.1, 0.04, 2.0, 0.3);
        Assert.That(otmPrice, Is.EqualTo(7.75053).Within(td.percentError).Percent);

        // For a call option that is Deep OTM price is worthless
        var deepOtmPrice = td.calculator.PV(Core.OptionType.Call, 2, 100, 0.1, 0.04, 2.0, 0.3);        
        Assert.That(deepOtmPrice, Is.EqualTo(0).Within(td.percentError).Percent);
    }

    [TestCaseSource(nameof(VanillaOptionCalculators))]
    public void WhenCalculatingAllGreeksForAnOptionShouldBeSimilarToCSharpVersion((IOptionsGreeksCalculator calculator, double percentError) td)
    {
        // stock with 6 months expiration, stock price is 100, strike price is 110, risk free interest rate 0.1 per year, continuous dividend yield 0.06 and volatility is 0.3 

        // Generalised model we use b differently

        // b = r: standard Black Scholes 1973 stock option model
        // b = r - q: gives the Merton 19973 stock option model with contionuous dividend yield q

        var spot = 100;
        var strike = 110;
        var r = 0.1;
        var q = 0.06;
        var b = r - q;
        var maturity = 0.5;
        var vol = 0.3;

        // PV
        var call = td.calculator.PV(Core.OptionType.Call, spot, strike, r, b, maturity, vol);
        Console.WriteLine($"Price of call is {call}");
        Assert.That(call, Is.EqualTo(6.5312).Within(td.percentError).Percent);

        var put = td.calculator.PV(Core.OptionType.Put, spot, strike, r, b, maturity, vol);
        Console.WriteLine($"Price of put is {put}");
        Assert.That(put, Is.EqualTo(11.155).Within(td.percentError).Percent);

        // Delta
        var delta = td.calculator.Delta(Core.OptionType.Call, spot, strike, r, b, maturity, vol);
        Console.WriteLine($"Delta of call {delta}");
        Assert.That(delta, Is.EqualTo(0.0317).Within(1.5));

        var deltaPut = td.calculator.Delta(Core.OptionType.Put, spot, strike, r, b, maturity, vol);
        Console.WriteLine($"Delta of put {deltaPut}");
        Assert.That(deltaPut, Is.EqualTo(0.0317).Within(1.5));

        // Gamma
        var gamma = td.calculator.Gamma(Core.OptionType.Call, spot, strike, r, b, maturity, vol);
        Console.WriteLine($"Gamma of call/put {gamma}");
        Assert.That(gamma, Is.EqualTo(0.038).Within(1));

        // Theta
        var theta = td.calculator.Theta(Core.OptionType.Call, spot, strike, r, b, maturity, vol);
        Console.WriteLine($"Theta for a call is {theta}");
        Assert.That(theta, Is.EqualTo(-12.3338).Within(td.percentError).Percent);

        var thetaPut = td.calculator.Theta(Core.OptionType.Put, spot, strike, r, b, maturity, vol);
        Console.WriteLine($"Theta for a Put is {thetaPut}");
        Assert.That(thetaPut, Is.EqualTo(-12.3338).Within(td.percentError).Percent);

        // Rho
        var rho = td.calculator.Rho(Core.OptionType.Call, spot, strike, r, b, maturity, vol);
        Console.WriteLine($"Rho of call is {rho}");
        Assert.That(rho, Is.EqualTo(-14.0376).Within(60));

        var rhoPut = td.calculator.Rho(Core.OptionType.Put, spot, strike, r, b, maturity, vol);
        Console.WriteLine($"Rho of put is {rho}");
        Assert.That(rhoPut, Is.EqualTo(19.5988).Within(60));

        // Vega
        var vega = td.calculator.Vega(Core.OptionType.Call, spot, strike, r, b, maturity, vol);
        Console.WriteLine($"Vega of call/put is {vega}");
        Assert.That(vega, Is.EqualTo(27.7411).Within(10).Percent);
    }

    [TestCaseSource(nameof(VanillaOptionCalculators))]
    public void WhenCalculatingImpliedVol((IOptionsGreeksCalculator calculator, double percentError) td)    
    {
        // stock with 6 months expiration, stock price is 100, strike price is 110, risk free interest rate 0.1 per year, continuous dividend yield 0.06 and volatility is 0.3 

        // Generalised model we use b differently

        // b = r: standard Black Scholes 1973 stock option model
        // b = r - q: gives the Merton 19973 stock option model with contionuous dividend yield q

        var spot = 100;
        var strike = 110;
        var r = 0.1;
        var q = 0.06;
        var b = r - q;

        double[] prices = new double[] { 0.15, 0.2, 0.25, 0.3, 0.35, 0.4, 0.45, 0.5, 0.55, 0.6 };
        for (int i = 0; i < 10; i++)
        {
            double maturity = (i + 1.0) / 10.0;
            var price = prices[i];
            var impliedVol = td.calculator.ImpliedVol(Core.OptionType.Call, spot, strike, r, b, maturity, price);
            Console.WriteLine($"ImpliedVol for price {price} and maturity {maturity} is {impliedVol}");
        }
    }
}