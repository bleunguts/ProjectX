
using ProjectXAnalyticsCppLib;
using System.Diagnostics;

namespace ProjectX.AnalyticsCppLib.Tests
{
    public class Tests
    {
        [Test]
        public void ShallBeAbleToPriceOptionWithCppOptionsPricingCalculator()
        {
            var calculator = new MonteCarloCppPricer(new RandomWalk(RandomAlgorithm.BoxMuller));

            VanillaOptionParameters theOption = new(OptionType.Call, 15.0, 0.9);
            double spot = 10.0;
            double vol = 0.3;
            double r = 0.1;
            uint numberOfPaths = 500;
            var sw = Stopwatch.StartNew();
            {
                var results = calculator.MCValue(ref theOption, spot, vol, r, numberOfPaths);
                double price = results.PV;
                Assert.That(Math.Round(price, 1), Is.EqualTo(0.2));
                sw.Stop();
                Console.WriteLine($"Completed {numberOfPaths} #MC paths in {sw.ElapsedMilliseconds} ms");
            }
        }
    }
}