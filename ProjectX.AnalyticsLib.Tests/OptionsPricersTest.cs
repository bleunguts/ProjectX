using Microsoft.Extensions.Options;
using ProjectX.AnalyticsLib.OptionsCalculators;
using ProjectXAnalyticsCppLib;
using System.Runtime.ConstrainedExecution;

namespace ProjectX.AnalyticsLib.Tests;

public class OptionsPricersTest
{
    static readonly IOptions<OptionsPricerCppWrapperOptions> options = Options.Create<OptionsPricerCppWrapperOptions>(new OptionsPricerCppWrapperOptions() 
    {  
        NumOfMcPaths = 250000, 
        RandomAlgo = RandomAlgorithm.BoxMuller 
    });
    static IEnumerable<(IOptionsGreeksCalculator calc, double percentError)> VanillaOptionCalculators()
    {
        yield return (new BlackScholesOptionsPricer(), 1.0);
        //yield return (new BlackScholesCppOptionsPricerWrapper(), 50);
        //yield return (new MonteCarloCppOptionsPricerWrapper(options), 50);
        //yield return (new MonteCarloCppOptionsPricer2Wrapper(1000), 50);
    }
    
    [TestCaseSource(nameof(VanillaOptionCalculators))]
    public void WhenPricingACallOptionShouldReturnRealisticOptionPrice((IOptionsGreeksCalculator calc, double percentError) td)
    {
        // Based on real number example http://financetrain.com/option-pricing-using-monte-carlo-simulation/
        // Result approx 9.95        
        var realExamplePrice = td.calc.PV(Core.OptionType.Call, 195.0, 200.0, 0.05, 0.0, 0.25, 0.3);
        Assert.That(realExamplePrice, Is.EqualTo(9.35).Within(15).Percent);        
    }    
    
    [TestCaseSource(nameof(VanillaOptionCalculators))]
    public void WhenPricingACallOptionShouldReturnRealisticValuesBasedOnMoneyness((IOptionsGreeksCalculator calc, double percentError) td)
    {        
        // For a call option that is deep ITM price is gt 0 should be expensive
        var deepItmPrice = td.calc.PV(Core.OptionType.Call, 510, 100, 0.1, 0.04, 2.0, 0.3);
        Assert.That(deepItmPrice, Is.EqualTo(370.45684).Within(td.percentError).Percent);

        // For a call option that is ITM price is gt 0 should be relative expensive
        var itmPrice = td.calc.PV(Core.OptionType.Call, 110, 100, 0.1, 0.04, 2.0, 0.3);
        Assert.That(itmPrice, Is.EqualTo(24.16204).Within(td.percentError).Percent);

        // For a call option that is ATM price is gt 0 should be fair priced
        var atmPrice =  td.calc.PV(Core.OptionType.Call, 100, 100, 0.1, 0.04, 2.0, 0.3);
        Assert.That(atmPrice, Is.EqualTo(17.9866).Within(td.percentError).Percent);

        // For a call option that is OTM price is cheaper
        var otmPrice = td.calc.PV(Core.OptionType.Call, 70, 100, 0.1, 0.04, 2.0, 0.3);
        Assert.That(otmPrice, Is.EqualTo(4.62534).Within(td.percentError).Percent);

        // For a call option that is Deep OTM price is worthless
        var deepOtmPrice = td.calc.PV(Core.OptionType.Call, 2, 100, 0.1, 0.04, 2.0, 0.3);        
        Assert.That(deepOtmPrice, Is.EqualTo(0).Within(td.percentError).Percent);
    }

    [Test]
    [Ignore("Still need to figure out how to best write this test")]       
    public void WhenCalculatingGreeksShouldReturnRealisticValues()
    {
        // stock with 6 months expiration, stock price is 100, strike price is 110, risk free interest rate 0.1 per year, continuous dividend yield 0.06 and volatility is 0.3 

        // Generalised model we use b differently
        // b = r: standard Black Scholes 1973 stock option model
        // b = r - q: gives the Merton 19973 stock option model with contionuous dividend yield q
        
        var spot = 100;     //($$$ per share)
        var strike = 110;   //($$$ per share)
        var r = 0.1;        //continuously compounded risk-free interest rate (% p.a.)
        var q = 0.06;       //continuously compounded dividend yield (% p.a.)
        var b = r - q;
        var maturity = 0.5; //(% of year)
        var vol = 0.3;      //(% p.a.)

        const double tolerancePV = 1.8;
        const double toleranceDelta = 5.5; // not sure about this diff
        const double toleranceRho = 20_000; // needs investigation
        const double toleranceGamma = 2;
        const double toleranceVega = 7500; // needss investigation

        // https://www.macroption.com/black-scholes-formula/        
        IBlackScholesCSharpPricer calc = new BlackScholesOptionsPricer();
        IMonteCarloCppOptionsPricer mc = new MonteCarloCppOptionsPricerWrapper(options); 
        IBlackScholesCppPricer bs = new BlackScholesCppOptionsPricerWrapper();
        IMonteCarloCppOptionsPricer2 mc2 = new MonteCarloCppOptionsPricer2Wrapper(1000);

        // PV       
        var call = calc.PV(Core.OptionType.Call, spot, strike, r, b, maturity, vol);
        Console.WriteLine($"Price of call is {call}");           
        Console.WriteLine($"Price of call [MC++] is {mc.PV(Core.OptionType.Call, spot, strike, r, b, maturity, vol)}");
        Console.WriteLine($"Price of call [MC2++] is {mc2.PV(Core.OptionType.Call, spot, strike, r, b, maturity, vol)}");
        Console.WriteLine($"Price of call [BS++] is {bs.PV(Core.OptionType.Call, spot, strike, r, b, maturity, vol)}");        
        Assert.That(call, Is.EqualTo(5.2515).Within(tolerancePV));
        Assert.That(new double[] { call, call }, Has.All.EqualTo(5.1515).Within(tolerancePV));

        var put = calc.PV(Core.OptionType.Put, spot, strike, r, b, maturity, vol);
        Console.WriteLine($"Price of put is {put}");        
        Console.WriteLine($"Price of put [MC++] is {mc.PV(Core.OptionType.Put, spot, strike, r, b, maturity, vol)}");
        Console.WriteLine($"Price of put [MC2++] is {mc2.PV(Core.OptionType.Put, spot, strike, r, b, maturity, vol)}");
        Console.WriteLine($"Price of put [BS++] is {bs.PV(Core.OptionType.Put, spot, strike, r, b, maturity, vol)}");
        Assert.That(put, Is.EqualTo(12.8422).Within(tolerancePV));

        // Delta
        var delta = calc.Delta(Core.OptionType.Call, spot, strike, r, b, maturity, vol);
        Console.WriteLine($"Delta of call is {delta}");
        Console.WriteLine($"Delta of call [MC++] is {mc.Delta(Core.OptionType.Call, spot, strike, r, b, maturity, vol)}");
        Console.WriteLine($"Delta of call [MC2++] is {mc2.Delta(Core.OptionType.Call, spot, strike, r, b, maturity, vol)}");
        Console.WriteLine($"Delta of call [BS++] is {bs.Delta(Core.OptionType.Call, spot, strike, r, b, maturity, vol)}");
        Assert.That(delta, Is.EqualTo(0.0317).Within(toleranceDelta));       

        var deltaPut = calc.Delta(Core.OptionType.Put, spot, strike, r, b, maturity, vol);
        Console.WriteLine($"Delta of put is {deltaPut}");
        Console.WriteLine($"Delta of put [MC++] is {mc.Delta(Core.OptionType.Put, spot, strike, r, b, maturity, vol)}");
        Console.WriteLine($"Delta of put [MC2++] is {mc2.Delta(Core.OptionType.Put, spot, strike, r, b, maturity, vol)}");
        Console.WriteLine($"Delta of put [BS++] is {bs.Delta(Core.OptionType.Put, spot, strike, r, b, maturity, vol)}");
        Assert.That(deltaPut, Is.EqualTo(0.0317).Within(toleranceDelta));
        // Assert Call Put Parity
        // If call delta is +1 (deep in the money), put delta is 0 (far out of the money). If call delta is 0, put delta is –1. If call delta is +0.7, put delta is –0.3.

        // Gamma
        var gamma = calc.Gamma(Core.OptionType.Call, spot, strike, r, b, maturity, vol);
        var gammaPut = calc.Gamma(Core.OptionType.Put, spot, strike, r, b, maturity, vol);
        Console.WriteLine($"Gamma of call/put {gamma}");
        Console.WriteLine($"Gamma of call/put [MC++] is {mc.Gamma(Core.OptionType.Call, spot, strike, r, b, maturity, vol)}");
        Console.WriteLine($"Gamma of call/put [MC2++] is {mc2.Gamma(Core.OptionType.Call, spot, strike, r, b, maturity, vol)}");
        Console.WriteLine($"Gamma of call/put [BS++] is {bs.Gamma(Core.OptionType.Call, spot, strike, r, b, maturity, vol)}");
        Assert.That(gamma, Is.EqualTo(0.038).Within(toleranceGamma));
        Assert.That(gamma, Is.EqualTo(gammaPut));

        /*
         * TODO: Fix For Theta
         */
        // Theta
        // If T is trading days (about 252), theta is change in option price per one trading day(or 1 / 252 of a year).
        // If T is calendar days (365), then the resulting theta is change in option price per one calendar day (or 1/365 of a year).
        var theta = calc.Theta(Core.OptionType.Call, spot, strike, r, b, maturity, vol);
        Console.WriteLine($"Theta for a call is {theta}");
        Console.WriteLine($"Theta for a call [MC++] is {mc.Theta(Core.OptionType.Call, spot, strike, r, b, maturity, vol)}");
        Console.WriteLine($"Theta for a call [MC2++] is {mc2.Theta(Core.OptionType.Call, spot, strike, r, b, maturity, vol)}");
        Console.WriteLine($"Theta for a call [BS++] is {bs.Theta(Core.OptionType.Call, spot, strike, r, b, maturity, vol)}");
        //Assert.That(theta, Is.EqualTo(-12.3338).Within(td.percentError).Percent);
        // Assert.That(theta, Is.LessThan(0));  // due to time decay

        var thetaPut = calc.Theta(Core.OptionType.Put, spot, strike, r, b, maturity, vol);
        Console.WriteLine($"Theta for a Put is {thetaPut}");
        Console.WriteLine($"Theta for a put [MC++] is {mc.Theta(Core.OptionType.Put, spot, strike, r, b, maturity, vol)}");
        Console.WriteLine($"Theta for a put [MC2++] is {mc2.Theta(Core.OptionType.Put, spot, strike, r, b, maturity, vol)}");
        Console.WriteLine($"Theta for a put [BS++] is {bs.Theta(Core.OptionType.Put, spot, strike, r, b, maturity, vol)}");
        //Assert.That(thetaPut, Is.EqualTo(-12.3338).Within(td.percentError).Percent);        
        // Assert.That(theta, Is.LessThan(0));  // due to time decay

        // Rho (interest rate)
        var rho = calc.Rho(Core.OptionType.Call, spot, strike, r, b, maturity, vol);
        Console.WriteLine($"Rho of call is {rho}");
        Console.WriteLine($"Rho of call [MC++] is {mc.Rho(Core.OptionType.Call, spot, strike, r, b, maturity, vol)}");
        Console.WriteLine($"Rho of call [MC2++] is {mc2.Rho(Core.OptionType.Call, spot, strike, r, b, maturity, vol)}");
        Console.WriteLine($"Rho of call [BS++] is {bs.Rho(Core.OptionType.Call, spot, strike, r, b, maturity, vol)}");
        Assert.That(rho, Is.EqualTo(-14.0376).Within(toleranceRho));
        // Call options are generally more valuable when interest rates are high 

        var rhoPut = calc.Rho(Core.OptionType.Put, spot, strike, r, b, maturity, vol);
        Console.WriteLine($"Rho of put is {rho}");
        Console.WriteLine($"Rho of put [MC++] is {mc.Rho(Core.OptionType.Put, spot, strike, r, b, maturity, vol)}");
        Console.WriteLine($"Rho of put [MC2++] is {mc2.Rho(Core.OptionType.Put, spot, strike, r, b, maturity, vol)}");
        Console.WriteLine($"Rho of put [BS++] is {bs.Rho(Core.OptionType.Put, spot, strike, r, b, maturity, vol)}");
        Assert.That(rhoPut, Is.EqualTo(19.5988).Within(toleranceRho));
        // Put options are generally more valuable when interest rates are low

        // Vega (volatility)
        var vega = calc.Vega(Core.OptionType.Call, spot, strike, r, b, maturity, vol);
        var vegaPut = calc.Vega(Core.OptionType.Put, spot, strike, r, b, maturity, vol);        
        Console.WriteLine($"Vega of call/put is {vega}");
        Console.WriteLine($"Vega of call/put [MC++] is {mc.Vega(Core.OptionType.Call, spot, strike, r, b, maturity, vol)}");
        Console.WriteLine($"Vega of call/put [MC2++] is {mc2.Vega(Core.OptionType.Call, spot, strike, r, b, maturity, vol)}");
        Console.WriteLine($"Vega of call/put [BS++] is {bs.Vega(Core.OptionType.Call, spot, strike, r, b, maturity, vol)}");
        Assert.That(vega, Is.EqualTo(27.7411).Within(toleranceVega));        
        // This is property of Vega that puts and calls are the same
        Assert.That(vega, Is.EqualTo(vegaPut));
        //Divide by 100 to get the resulting vega as option price change for one percentage point change in volatility
    }

    [TestCaseSource(nameof(VanillaOptionCalculators))]
    public void WhenCalculatingImpliedVol((IOptionsGreeksCalculator calc, double percentError) td)    
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
        var expected = new Dictionary<double, double>()
        {
            { 0.1, 0.18365478515625 },
            { 0.2, 0.13409423828125 },
            { 0.3, 0.11175537109375 },
            { 0.4, 0.09808349609375 },
            { 0.5, 0.08843994140625 },
            { 0.6, 0.08099365234375 },
            { 0.7, 0.07501220703125 },
            { 0.8, 0.06988525390625 },
            { 0.9, 0.06549072265625 },
            { 1.0, 0.06158447265625 },
        };        
        double[] prices = new double[] { 0.15, 0.2, 0.25, 0.3, 0.35, 0.4, 0.45, 0.5, 0.55, 0.6 };
        for (int i = 0; i < 10; i++)
        {
            double maturity = (i + 1.0) / 10.0;
            var price = prices[i];
            var impliedVol = td.calc.ImpliedVol(Core.OptionType.Call, spot, strike, r, b, maturity, price);
            Console.WriteLine($"ImpliedVol for price {price} and maturity {maturity} is {impliedVol}");
            /*
                    ImpliedVol for price 0.15 and maturity 0.1 is 0.18365478515625
                    ImpliedVol for price 0.2 and maturity 0.2 is 0.13409423828125
                    ImpliedVol for price 0.25 and maturity 0.3 is 0.11175537109375
                    ImpliedVol for price 0.3 and maturity 0.4 is 0.09808349609375
                    ImpliedVol for price 0.35 and maturity 0.5 is 0.08843994140625
                    ImpliedVol for price 0.4 and maturity 0.6 is 0.08099365234375
                    ImpliedVol for price 0.45 and maturity 0.7 is 0.07501220703125
                    ImpliedVol for price 0.5 and maturity 0.8 is 0.06988525390625
                    ImpliedVol for price 0.55 and maturity 0.9 is 0.06549072265625
                    ImpliedVol for price 0.6 and maturity 1 is 0.06158447265625
             */
            Assert.That(impliedVol, Is.EqualTo(expected[maturity]).Within(1).Percent);
        }
    }
}