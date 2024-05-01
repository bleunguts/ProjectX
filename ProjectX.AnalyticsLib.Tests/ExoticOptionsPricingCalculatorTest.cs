using NUnit.Framework;
using ProjectX.AnalyticsLib;
using ProjectX.AnalyticsLib.OptionsCalculators;
using ProjectX.Core;
using ProjectX.Core.Analytics;
using ProjectX.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectX.AnalyticsLib.Tests;

public class ExoticOptionsPricingCalculatorTest
{
    private ExoticOptionsPricingCalculator _calculator = new ExoticOptionsPricingCalculator(new BlackScholesOptionsPricer());

    [Test]
    public void WhenPricingAnAmericanOption()
    {
        var spot = 90.0;
        var strike = 100.0;
        var rate = 0.1;
        var divYield = 0.1;
        var maturity = 0.10;
        var vol = 0.15;

        var price = _calculator.American_BaroneAdesiWhaley(OptionType.Call, spot, strike, rate, divYield, maturity, vol);
        Console.WriteLine($"Price of call american option is {price}");
        Assert.That(price, Is.EqualTo(0.0260).Within(1).Percent);

        var putPrice = _calculator.American_BaroneAdesiWhaley(OptionType.Put, spot, strike, rate, divYield, maturity, vol);
        Console.WriteLine($"Price of put american option is {putPrice}");
        Assert.That(putPrice, Is.EqualTo(10.00).Within(1).Percent);
    }

    [Test]
    public void WhenPricingABarrierOption()
    {
        var spot = 100.0;
        var strike = 100.0;
        var rate = 0.1;
        var yield = 0.06;
        var maturity = 0.1;
        var vol = 0.3;
        var barrier = 90;
        var rebate = 0;

        var price = _calculator.BarrierOptions(OptionType.Call, BarrierType.DownIn, spot, strike, rate, yield, maturity, vol, barrier, rebate);
        Console.WriteLine($"Price of call barrier option is {price}");
        Assert.That(price, Is.EqualTo(0.0444).Within(1).Percent);

        var putPrice = _calculator.BarrierOptions(OptionType.Put, BarrierType.DownIn, spot, strike, rate, yield, maturity, vol, barrier, rebate);
        Console.WriteLine($"Price of put barrier option is {putPrice}");
        Assert.That(putPrice, Is.EqualTo(2.65).Within(1).Percent);
    }
}
