﻿using ProjectX.AnalyticsLib;
using ProjectX.Core.Analytics;

namespace ProjectX.Core.Tests
{
    public class BlackScholesOptionsPricingCalculatorTest
    {
        private BlackScholesOptionsPricingCalculator _calculator = new BlackScholesOptionsPricingCalculator();
        
        [Test]
        public void WhenPricingACallOption()
        {
            // For a call option that is deep ITM price is gt 0 should be expensive
            var deepItmPrice = _calculator.BlackScholes(OptionType.Call, 510, 100, 0.1, 0.04, 2.0, 0.3);
            Assert.Greater(deepItmPrice, 0);
            Assert.That(deepItmPrice, Is.EqualTo(370.4568).Within(1).Percent);

            // For a call option that is ITM price is gt 0 should be relative expensive
            var itmPrice = _calculator.BlackScholes(OptionType.Call, 110, 100, 0.1, 0.04, 2.0, 0.3);
            Assert.Greater(itmPrice, 0);
            Assert.That(itmPrice, Is.EqualTo(24.1620).Within(1).Percent);

            // For a call option that is ATM price is gt 0 should be fair priced
            var atmPrice = _calculator.BlackScholes(OptionType.Call, 100, 100, 0.1, 0.04, 2.0, 0.3);
            Assert.Greater(atmPrice, 0);
            Assert.That(atmPrice, Is.EqualTo(17.9866).Within(1).Percent);

            // For a call option that is OTM price is cheaper
            var otmPrice = _calculator.BlackScholes(OptionType.Call, 70, 100, 0.1, 0.04, 2.0, 0.3);
            Assert.Greater(otmPrice, 0);
            Assert.That(otmPrice, Is.EqualTo(4.6253).Within(1).Percent);

            // For a call option that is Deep OTM price is worthless
            var deepOtmPrice = _calculator.BlackScholes(OptionType.Call, 2, 100, 0.1, 0.04, 2.0, 0.3);
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

            var call = _calculator.BlackScholes(OptionType.Call, spot, strike, r, b, maturity, vol);
            Console.WriteLine($"Price of call is {call}");
            Assert.That(call, Is.EqualTo(5.2515).Within(1).Percent);

            var put = _calculator.BlackScholes(OptionType.Put, spot, strike, r, b, maturity, vol);
            Console.WriteLine($"Price of put is {put}");
            Assert.That(put, Is.EqualTo(12.8422).Within(1).Percent);

            var gamma = _calculator.BlackScholes_Gamma(spot, strike, r, b, maturity, vol);
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

            var call = _calculator.BlackScholes(OptionType.Call, spot, strike, r, b, maturity, vol);
            Console.WriteLine($"Price of call is {call}");
            Assert.That(call, Is.EqualTo(5.2515).Within(1).Percent);

            var put = _calculator.BlackScholes(OptionType.Put, spot, strike, r, b, maturity, vol);
            Console.WriteLine($"Price of put is {put}");
            Assert.That(put, Is.EqualTo(12.8422).Within(1).Percent);

            var theta = _calculator.BlackScholes_Theta(OptionType.Call, spot, strike, r, b, maturity, vol);
            Console.WriteLine($"Theta for a call is {theta}");
            Assert.That(theta, Is.EqualTo(-8.9962).Within(1).Percent);

            var thetaPut = _calculator.BlackScholes_Theta(OptionType.Put, spot, strike, r, b, maturity, vol);
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

            var call = _calculator.BlackScholes(OptionType.Call, spot, strike, r, b, maturity, vol);
            Console.WriteLine($"Price of call is {call}");
            Assert.That(call, Is.EqualTo(5.2515).Within(1).Percent);

            var put = _calculator.BlackScholes(OptionType.Put, spot, strike, r, b, maturity, vol);
            Console.WriteLine($"Price of put is {put}");
            Assert.That(put, Is.EqualTo(12.8422).Within(1).Percent);

            var rho = _calculator.BlackScholes_Rho(OptionType.Call, spot, strike, r, b, maturity, vol);
            Console.WriteLine($"Rho of call is {rho}");
            Assert.That(rho, Is.EqualTo(16.8656).Within(1).Percent);

            var rhoPut = _calculator.BlackScholes_Rho(OptionType.Put, spot, strike, r, b, maturity, vol);
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

            var call = _calculator.BlackScholes(OptionType.Call, spot, strike, r, b, maturity, vol);
            Console.WriteLine($"Price of call is {call}");
            Assert.That(call, Is.EqualTo(5.2515).Within(1).Percent);

            var put = _calculator.BlackScholes(OptionType.Put, spot, strike, r, b, maturity, vol);
            Console.WriteLine($"Price of put is {put}");
            Assert.That(put, Is.EqualTo(12.8422).Within(1).Percent);

            var vega = _calculator.BlackScholes_Vega(spot, strike, r, b, maturity, vol);
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
                var impliedVol = _calculator.BlackScholes_ImpliedVol(OptionType.Call, spot, strike, r, b, maturity, price);
                Console.WriteLine($"ImpliedVol for price {price} and maturity {maturity} is {impliedVol}");
            }
        }
    }
}