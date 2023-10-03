using Newtonsoft.Json;
using ProjectX.Core.Requests;
using ProjectX.Core.Services;

namespace ProjectX.Core.Tests
{
    public class BlackScholesOptionsPricingModelTest
    {
        private readonly BlackScholesOptionsPricingModel _sut = new BlackScholesOptionsPricingModel();

        [Test]
        public async Task WhenPricingBlackScholesOptionItShouldReturnValidResultsAsync()
        {
            var request = new OptionsPricingByMaturitiesRequest(10, OptionType.Call, 100.0, 150.0, 1.0, 1.0, 0.3);
            
            Console.WriteLine($"JSON={JsonConvert.SerializeObject(request)}");
            var actual = _sut.Price(request);
            Assert.That(actual, Is.Not.Null);            
            Assert.That(actual.ResultsCount, Is.EqualTo(10));
            Assert.That(actual[0].Maturity, Is.EqualTo(0.1));
            Assert.That(actual[0].OptionGreeks.price, Is.Not.EqualTo(0));           
        }

        [Test]
        public void WhenPlottingGreeksZValuesAreValid()
        {                        
            var result = _sut.PlotGreeks(new PlotOptionsPricingRequest(OptionGreeks.Price, OptionType.Call, 100, 0.1, 0.04, 0.3));
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
