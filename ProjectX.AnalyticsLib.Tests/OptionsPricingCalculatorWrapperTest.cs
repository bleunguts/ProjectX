using ProjectXAnalyticsCppLib;
using System.Diagnostics;

namespace ProjectX.AnalyticsLib.Tests;

public class OptionsPricingCalculatorWrapperTest
{
    static IEnumerable<(IBlackScholesOptionsPricingCalculator calculator, double percentError)> VanillaOptionCalculators()
    {
        yield return (new BlackScholesOptionsPricingCalculator(), 42.0);
        yield return (new OptionsPricingCppCalculatorWrapper(200_000, RandomAlgorithm.BoxMuller), 1.75);
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

    [TestCaseSource(nameof(VanillaOptionCalculators))]
    public void WhenCalculatingDeltaForAnOption((IBlackScholesOptionsPricingCalculator calculator, double percentError) td)
    {       
        var spot = 100;
        var strike = 100;
        var r = 0.05;        
        var maturity = 1.0;
        var vol = 0.2;
        var b = 0.0;

        var call = td.calculator.BlackScholes(Core.OptionType.Call, spot, strike, r, b, maturity, vol);
        Console.WriteLine($"Price of call is {call}");
        Assert.That(call, Is.EqualTo(10.4879).Within(td.percentError).Percent);    

        var delta = td.calculator.BlackScholes_Delta(Core.OptionType.Call, spot, strike, r, b, maturity, vol);
        Console.WriteLine($"Delta of call {delta}");
        Assert.That(delta, Is.EqualTo(6.3608).Within(td.percentError).Percent);
    }

    [TestCaseSource(nameof(VanillaOptionCalculators))]
    public void WhenCalculatingGammaForAnOption((IBlackScholesOptionsPricingCalculator calculator, double percentError) td)
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

        var call = td.calculator.BlackScholes(Core.OptionType.Call, spot, strike, r, b, maturity, vol);
        Console.WriteLine($"Price of call is {call}");
        Assert.That(call, Is.EqualTo(6.5312).Within(td.percentError).Percent);

        var put = td.calculator.BlackScholes(Core.OptionType.Put, spot, strike, r, b, maturity, vol);
        Console.WriteLine($"Price of put is {put}");
        Assert.That(put, Is.EqualTo(11.155).Within(td.percentError).Percent);

        var gamma = td.calculator.BlackScholes_Gamma(Core.OptionType.Call, spot, strike, r, b, maturity, vol);
        Console.WriteLine($"Gamma of call/put {gamma}");
        Assert.That(gamma, Is.EqualTo(0.01869).Within(td.percentError).Percent);
    }


    [TestCaseSource(nameof(VanillaOptionCalculators))]
    public void WhenCalculatingThetaForAnOption((IBlackScholesOptionsPricingCalculator calculator, double percentError) td)    
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

        var call = td.calculator.BlackScholes(Core.OptionType.Call, spot, strike, r, b, maturity, vol);
        Console.WriteLine($"Price of call is {call}");
        Assert.That(call, Is.EqualTo(6.5217).Within(td.percentError).Percent);

        var put = td.calculator.BlackScholes(Core.OptionType.Put, spot, strike, r, b, maturity, vol);
        Console.WriteLine($"Price of put is {put}");
        Assert.That(put, Is.EqualTo(11.1553).Within(td.percentError).Percent);

        var theta = td.calculator.BlackScholes_Theta(Core.OptionType.Call, spot, strike, r, b, maturity, vol);
        Console.WriteLine($"Theta for a call is {theta}");
        Assert.That(theta, Is.EqualTo(-12.3338).Within(td.percentError).Percent);

        var thetaPut = td.calculator.BlackScholes_Theta(Core.OptionType.Put, spot, strike, r, b, maturity, vol);
        Console.WriteLine($"Theta for a Put is {thetaPut}");
        Assert.That(thetaPut, Is.EqualTo(-12.3338).Within(td.percentError).Percent);
    }

    [TestCaseSource(nameof(VanillaOptionCalculators))]
    public void WhenCalculatingRhoForAnoption((IBlackScholesOptionsPricingCalculator calculator, double percentError) td)    
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

        var call = td.calculator.BlackScholes(Core.OptionType.Call, spot, strike, r, b, maturity, vol);
        Console.WriteLine($"Price of call is {call}");
        Assert.That(call, Is.EqualTo(6.5562).Within(td.percentError).Percent);

        var put = td.calculator.BlackScholes(Core.OptionType.Put, spot, strike, r, b, maturity, vol);
        Console.WriteLine($"Price of put is {put}");
        Assert.That(put, Is.EqualTo(11.15532).Within(td.percentError).Percent);

        var rho = td.calculator.BlackScholes_Rho(Core.OptionType.Call, spot, strike, r, b, maturity, vol);
        Console.WriteLine($"Rho of call is {rho}");
        Assert.That(rho, Is.EqualTo(19.598).Within(td.percentError).Percent);

        var rhoPut = td.calculator.BlackScholes_Rho(Core.OptionType.Put, spot, strike, r, b, maturity, vol);
        Console.WriteLine($"Rho of put is {rho}");
        Assert.That(rhoPut, Is.EqualTo(19.5988).Within(td.percentError).Percent);
    }

    [TestCaseSource(nameof(VanillaOptionCalculators))]
    public void WhenCalculatingVegaForAnOption((IBlackScholesOptionsPricingCalculator calculator, double percentError) td)    
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

        var call = td.calculator.BlackScholes(Core.OptionType.Call, spot, strike, r, b, maturity, vol);
        Console.WriteLine($"Price of call is {call}");
        Assert.That(call, Is.EqualTo(6.49988).Within(td.percentError).Percent);

        var put = td.calculator.BlackScholes(Core.OptionType.Put, spot, strike, r, b, maturity, vol);
        Console.WriteLine($"Price of put is {put}");
        Assert.That(put, Is.EqualTo(11.1553).Within(td.percentError).Percent);

        var vega = td.calculator.BlackScholes_Vega(Core.OptionType.Call, spot, strike, r, b, maturity, vol);
        Console.WriteLine($"Vega of call/put is {vega}");
        Assert.That(vega, Is.EqualTo(28.04686).Within(td.percentError).Percent);
    }

    [TestCaseSource(nameof(VanillaOptionCalculators))]
    public void WhenCalculatingImpliedVol((IBlackScholesOptionsPricingCalculator calculator, double percentError) td)    
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
            var impliedVol = td.calculator.BlackScholes_ImpliedVol(Core.OptionType.Call, spot, strike, r, b, maturity, price);
            Console.WriteLine($"ImpliedVol for price {price} and maturity {maturity} is {impliedVol}");
        }
    }
}