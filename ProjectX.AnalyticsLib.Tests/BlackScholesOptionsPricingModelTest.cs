using Newtonsoft.Json;
using ProjectX.Core.Requests;
using ProjectX.Core.Services;

namespace ProjectX.Core.Tests
{
    public class BlackScholesOptionsPricingModelTest
    {
        private readonly BlackScholesOptionsPricingModel _sut = new BlackScholesOptionsPricingModel();

        [Test]
        public void WhenPricingBlackScholesOptionItShouldReturnValidResults()
        {
            var request = new MultipleTimeslicesOptionsPricingRequest(10, OptionType.Call, 100.0, 150.0, 1.0, 1.0, 0.3);
            
            Console.WriteLine($"JSON={JsonConvert.SerializeObject(request)}");
            var actual = _sut.PriceFor(request);
            Assert.That(actual, Is.Not.Null);
            var r = actual.ToList();
            Assert.That(r, Has.Count.EqualTo(10));
            Assert.That(r.First().maturity, Is.EqualTo(0.1));
            Assert.That(r.First().optionPriceResult.price, Is.Not.EqualTo(0));           
        }

        [Test]
        public void WhenPlottingGreeksZValuesAreValid()
        {                        
            var result = _sut.PlotGreeks(OptionGreeks.Price, OptionType.Call, 100, 0.1, 0.04, 0.3);
            AssertZValue(result.zmin, result.zmax, rounding: 1);
        }

        private static void AssertZValue(double zmin, double zmax, int rounding)
        {
            var theZmin = Math.Round(zmin, rounding);
            var theZmax = Math.Round(zmax, rounding);
            var theZTick = Math.Round((zmax - zmin) / 5.0, rounding);

            Assert.IsTrue(zmin < zmax);
            Assert.IsTrue(theZTick > 0);
        }       
    }
}
