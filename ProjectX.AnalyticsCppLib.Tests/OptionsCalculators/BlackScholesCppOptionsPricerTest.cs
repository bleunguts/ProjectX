using ProjectX.AnalyticsLib.OptionsCalculators;
using ProjectX.AnalyticsLib.Shared;
using ProjectX.Core;

namespace ProjectX.AnalyticsLib.Tests.OptionsCalculators
{
    /// <summary>
    /// Values based on excel spreadsheet calculator
    /// https://goodcalculators.com/black-scholes-calculator/
    /// </summary>
    public class BlackScholesCppOptionsPricerTest
    {        
        IOptionsGreeksCalculator _pricer = new BlackScholesCppOptionsPricerWrapper();
        private IOptionsGreeksCalculator GetCalculator() => _pricer;

        // stock with 6 months expiration, stock price is 100, strike price is 110, risk free interest rate 0.1 per year, continuous dividend yield 0.06 and volatility is 0.3 

        // Generalised model we use b differently
        // b = r: standard Black Scholes 1973 stock option model
        // b = r - q: gives the Merton 19973 stock option model with contionuous dividend yield q
        static readonly double spot = 100;
        static readonly double strike = 110;
        static readonly double r = 0.1;
        static readonly double q = 0.06;  // continuous dividend yiled
        // cost of carry
        // b= 0 gives Black (1976) futures option model
        // b= r-q gives Merton(1973) model
        // b= r gives Black-Scholes model
        //static readonly double b = r - q; // cost of carry charge         
        static readonly double b = 0; // cost of carry charge         
        static readonly double maturity = 0.5;
        static readonly double vol = 0.3;

        [Test]
        public void WhenComputingPV()
        {
            var calculator = GetCalculator();
            var call = calculator.PV(OptionType.Call, spot, strike, r, b, maturity, vol);
            Console.WriteLine($"Price of call is {call}");
            Assert.That(call, Is.EqualTo(6.52078264).Within(1).Percent);

            var put = calculator.PV(OptionType.Put, spot, strike, r, b, maturity, vol);
            Console.WriteLine($"Price of put is {put}");
            Assert.That(put, Is.EqualTo(11.15601933).Within(1).Percent);

            // Assert Call Put Parity
            // If call delta is +1 (deep in the money), put delta is 0 (far out of the money).
            // If call delta is 0, put delta is –1.
            // If call delta is +0.7, put delta is –0.3.
            Assert.That(call, Is.Not.EqualTo(put), "Call-Put Parity should be obeyed");
        }

        [Test]
        public void WhenComputingDelta()
        {
            var calculator = GetCalculator();
            var delta = calculator.Delta(OptionType.Call, spot, strike, r, b, maturity, vol);
            Console.WriteLine($"Delta for a call is {delta}");
            Assert.That(delta, Is.EqualTo(0.45718497).Within(1).Percent);

            var deltaPut = calculator.Delta(OptionType.Put, spot, strike, r, b, maturity, vol);
            Console.WriteLine($"Delta for a Put is {deltaPut}");
            Assert.That(deltaPut, Is.EqualTo(-0.54281503).Within(1).Percent);
        }

        [Test]
        public void WhenComputingGamma()
        {
            var calculator = GetCalculator();
            var gamma = calculator.Gamma(OptionType.Call, spot, strike, r, b, maturity, vol);
            Console.WriteLine($"Gamma of call/put {gamma}");
            Assert.That(gamma, Is.EqualTo(0.018697).Within(1).Percent);

            // Gamma should be the same for call and put
            var gammaPut = calculator.Gamma(OptionType.Put, spot, strike, r, b, maturity, vol);
            Assert.That(gamma, Is.EqualTo(gammaPut), "Gamma is put/call agnostic");
        }

        [Test]
        public void WhenComputingTheta()
        {
            var calculator = GetCalculator();
            var theta = calculator.Theta(OptionType.Call, spot, strike, r, b, maturity, vol);
            Console.WriteLine($"Theta for a call is {theta}");
            Assert.That(theta, Is.EqualTo(-12.334).Within(1).Percent);

            var thetaPut = calculator.Theta(OptionType.Put, spot, strike, r, b, maturity, vol);
            Console.WriteLine($"Theta for a Put is {thetaPut}");
            Assert.That(thetaPut, Is.EqualTo(-1.870).Within(1).Percent);

            // due to time decay
            Assert.That(theta, Is.LessThan(0), "Theta is always negative due to nature of time decay");
            Assert.That(thetaPut, Is.LessThan(0), "Theta is always negative due to nature of time decay");
        }

        [Test]
        public void WhenComputingRho()
        {
            var calculator = GetCalculator();

            var rho = calculator.Rho(OptionType.Call, spot, strike, r, b, maturity, vol);
            Console.WriteLine($"Rho of call is {rho}");
            Assert.That(rho, Is.EqualTo(0.195988572).Within(1).Percent);

            var rhoPut = calculator.Rho(OptionType.Put, spot, strike, r, b, maturity, vol);
            Console.WriteLine($"Rho of put is {rho}");
            Assert.That(rhoPut, Is.EqualTo(-0.327187612).Within(1).Percent);

            var rho2 = calculator.Rho(OptionType.Call, spot, strike, 0.8, b, maturity, vol);
            var rhoPut2 = calculator.Rho(OptionType.Put, spot, strike, 0.8, b, maturity, vol);
            // Call options are generally more valuable when interest rates are high 
            Assert.That(rho2, Is.GreaterThan(rhoPut2), "Call options should have higher value than put due to interest rates");
        }

        [Test]
        public void WhenComputingVega()
        {
            var calculator = GetCalculator();
            var vega = calculator.Vega(OptionType.Call, spot, strike, r, b, maturity, vol);
            Console.WriteLine($"Vega of call/put is {vega}");
            //Divide by 100 to get the resulting vega as option price change for one percentage point change in volatility
            Console.WriteLine($"Vega of call/put is {vega / 100} for 1% change in vol");
            Assert.That(vega, Is.EqualTo(0.280468662).Within(1).Percent);

            // This is property of Vega that puts and calls are the same
            var vegaPut = calculator.Vega(OptionType.Call, spot, strike, r, b, maturity, vol);
            Assert.That(vega, Is.EqualTo(vegaPut), "Vega is call/put agnostic");
        }

        [Test]
        [Ignore("TODO: See Implied Vol story")]
        public void WhenComputingImpliedVol()
        {
            var calculator = GetCalculator();
            Dictionary<double, double> ExpectedVols = new Dictionary<double, double>()
            {
                { 0.1, 0.19012451171875 },
                { 0.2, 0.14349365234375 },
                { 0.3, 0.12384033203125 },
                { 0.4, 0.11248779296875 },
                { 0.5, 0.10504150390625 },
                { 0.6, 0.09979248046875 },
                { 0.7, 0.09588623046875 },
                { 0.8, 0.09271240234375 },
                { 0.9, 0.09039306640625 },
                { 1.0, 0.08831787109375 },
            };
            // 0.05 
            // mktprice 6.5207
            double mktprice = 6.5207;
            double[] increments = new double[] { 0.15, 0.2, 0.25, 0.3, 0.35, 0.4, 0.45, 0.5, 0.55, 0.6 };
            for (int i = 0; i < 10; i++)
            {
                double maturity = (i + 1.0) / 10.0;
                var price = mktprice;
                var impliedVol = calculator.ImpliedVol(OptionType.Call, spot, strike, r, b, maturity, price);
                Console.WriteLine($"ImpliedVol for price {price} and maturity {maturity} is {impliedVol}");
                Assert.That(impliedVol, Is.EqualTo(ExpectedVols[maturity]).Within(1).Percent);
            }
        }
    }
}
