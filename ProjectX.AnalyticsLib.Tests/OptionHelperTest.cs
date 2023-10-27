﻿using NUnit.Framework;
using ProjectX.Core.Analytics;
using ProjectX.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectX.Core.Tests
{
    public class OptionHelperTest
    {            
        [Test]
        public void WhenPricingACallOption()
        {
            // For a call option that is deep ITM price is gt 0 should be expensive
            var deepItmPrice = OptionsCalculatorFunctions.BlackScholes(OptionType.Call, 510, 100, 0.1, 0.04, 2.0, 0.3);
            Assert.Greater(deepItmPrice, 0);
            Assert.That(deepItmPrice, Is.EqualTo(370.4568).Within(1).Percent);

            // For a call option that is ITM price is gt 0 should be relative expensive
            var itmPrice = OptionsCalculatorFunctions.BlackScholes(OptionType.Call, 110, 100, 0.1, 0.04, 2.0, 0.3);
            Assert.Greater(itmPrice, 0);
            Assert.That(itmPrice, Is.EqualTo(24.1620).Within(1).Percent);

            // For a call option that is ATM price is gt 0 should be fair priced
            var atmPrice = OptionsCalculatorFunctions.BlackScholes(OptionType.Call, 100, 100, 0.1, 0.04, 2.0, 0.3);
            Assert.Greater(atmPrice, 0);
            Assert.That(atmPrice, Is.EqualTo(17.9866).Within(1).Percent);

            // For a call option that is OTM price is cheaper
            var otmPrice = OptionsCalculatorFunctions.BlackScholes(OptionType.Call, 70, 100, 0.1, 0.04, 2.0, 0.3);
            Assert.Greater(otmPrice, 0);
            Assert.That(otmPrice, Is.EqualTo(4.6253).Within(1).Percent);

            // For a call option that is Deep OTM price is worthless
            var deepOtmPrice = OptionsCalculatorFunctions.BlackScholes(OptionType.Call, 2, 100, 0.1, 0.04, 2.0, 0.3);
            Assert.AreEqual(deepOtmPrice, 0);
            Assert.That(deepOtmPrice, Is.EqualTo(0).Within(1).Percent);
        }

        [Test]
        public void WhenCalculatingGammaForAnOption()
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

            var call = OptionsCalculatorFunctions.BlackScholes(OptionType.Call, spot, strike, r, b, maturity, vol);
            Console.WriteLine($"Price of call is {call}");
            Assert.That(call, Is.EqualTo(5.2515).Within(1).Percent);

            var put = OptionsCalculatorFunctions.BlackScholes(OptionType.Put, spot, strike, r, b, maturity, vol);
            Console.WriteLine($"Price of put is {put}");
            Assert.That(put, Is.EqualTo(12.8422).Within(1).Percent);

            var gamma = OptionsCalculatorFunctions.BlackScholes_Gamma(spot, strike, r, b, maturity, vol);
            Console.WriteLine($"Gamma of call/put {gamma}");
            Assert.That(gamma, Is.EqualTo(0.01769).Within(1).Percent);
        }

        [Test]
        public void WhenCalculatingThetaForAnOption()
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

            var call = OptionsCalculatorFunctions.BlackScholes(OptionType.Call, spot, strike, r, b, maturity, vol);
            Console.WriteLine($"Price of call is {call}");
            Assert.That(call, Is.EqualTo(5.2515).Within(1).Percent);

            var put = OptionsCalculatorFunctions.BlackScholes(OptionType.Put, spot, strike, r, b, maturity, vol);
            Console.WriteLine($"Price of put is {put}");
            Assert.That(put, Is.EqualTo(12.8422).Within(1).Percent);

            var theta = OptionsCalculatorFunctions.BlackScholes_Theta(OptionType.Call, spot, strike, r, b, maturity, vol);
            Console.WriteLine($"Theta for a call is {theta}");
            Assert.That(theta, Is.EqualTo(-8.9962).Within(1).Percent);

            var thetaPut = OptionsCalculatorFunctions.BlackScholes_Theta(OptionType.Put, spot, strike, r, b, maturity, vol);
            Console.WriteLine($"Theta for a Put is {thetaPut}");
            Assert.That(thetaPut, Is.EqualTo(-4.3554).Within(1).Percent);
        }

        [Test]
        public void WhenCalculatingRhoForAnoption()
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

            var call = OptionsCalculatorFunctions.BlackScholes(OptionType.Call, spot, strike, r, b, maturity, vol);
            Console.WriteLine($"Price of call is {call}");
            Assert.That(call, Is.EqualTo(5.2515).Within(1).Percent);

            var put = OptionsCalculatorFunctions.BlackScholes(OptionType.Put, spot, strike, r, b, maturity, vol);
            Console.WriteLine($"Price of put is {put}");
            Assert.That(put, Is.EqualTo(12.8422).Within(1).Percent);

            var rho = OptionsCalculatorFunctions.BlackScholes_Rho(OptionType.Call, spot, strike, r, b, maturity, vol);
            Console.WriteLine($"Rho of call is {rho}");
            Assert.That(rho, Is.EqualTo(16.8656).Within(1).Percent);

            var rhoPut = OptionsCalculatorFunctions.BlackScholes_Rho(OptionType.Put, spot, strike, r, b, maturity, vol);
            Console.WriteLine($"Rho of put is {rho}");
            Assert.That(rhoPut, Is.EqualTo(-35.4519).Within(1).Percent);
        }

        [Test]
        public void WhenCalculatingVegaForAnOption()
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

            var call = OptionsCalculatorFunctions.BlackScholes(OptionType.Call, spot, strike, r, b, maturity, vol);
            Console.WriteLine($"Price of call is {call}");
            Assert.That(call, Is.EqualTo(5.2515).Within(1).Percent);

            var put = OptionsCalculatorFunctions.BlackScholes(OptionType.Put, spot, strike, r, b, maturity, vol);
            Console.WriteLine($"Price of put is {put}");
            Assert.That(put, Is.EqualTo(12.8422).Within(1).Percent);

            var vega = OptionsCalculatorFunctions.BlackScholes_Vega(spot, strike, r, b, maturity, vol);
            Console.WriteLine($"Vega of call/put is {vega}");
            Assert.That(vega, Is.EqualTo(27.5649).Within(1).Percent);
        }

        [Test]
        public void WhenCalculatingImpliedVol()
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
                var impliedVol = OptionsCalculatorFunctions.BlackScholes_ImpliedVol(OptionType.Call, spot, strike, r, b, maturity, price);
                Console.WriteLine($"ImpliedVol for price {price} and maturity {maturity} is {impliedVol}");
            }
        }

        [Test]
        public void WhenPricingAnAmericanOption()
        {
            var spot = 90.0;
            var strike = 100.0;
            var rate = 0.1;
            var divYield = 0.1;
            var maturity = 0.10;
            var vol = 0.15;

            var price = OptionsCalculatorFunctions.American_BaroneAdesiWhaley(OptionType.Call, spot, strike, rate, divYield, maturity, vol);
            Console.WriteLine($"Price of call american option is {price}");
            Assert.That(price, Is.EqualTo(0.0260).Within(1).Percent);

            var putPrice = OptionsCalculatorFunctions.American_BaroneAdesiWhaley(OptionType.Put, spot, strike, rate, divYield, maturity, vol);
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

            var price = OptionsCalculatorFunctions.BarrierOptions(OptionType.Call, BarrierType.DownIn, spot, strike, rate, yield, maturity, vol, barrier, rebate);
            Console.WriteLine($"Price of call barrier option is {price}");
            Assert.That(price, Is.EqualTo(0.0444).Within(1).Percent);

            var putPrice = OptionsCalculatorFunctions.BarrierOptions(OptionType.Put, BarrierType.DownIn, spot, strike, rate, yield, maturity, vol, barrier, rebate);
            Console.WriteLine($"Price of put barrier option is {putPrice}");
            Assert.That(putPrice, Is.EqualTo(2.65).Within(1).Percent);
        }
    }
}
