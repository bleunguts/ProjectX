using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectX.Core.Services;

namespace ProjectX.Core.Tests
{
    public class ExtensionsTest
    {        
        [Test]
        public void WhenPrettifyBidAskSpotPriceItShouldConvertToAPrettyStringToBeDisplayedToTheUser()
        {
            var spotPrice = new SpotPrice("GBPUSD", 1.6020M, 1.8343M);
            Assert.That(spotPrice.ToPrettifiedBidAskPrice(), Is.EqualTo("1.60200/1.83430"));
        }

        [Test]
        public void WhenConvertingBidAskSpotPriceBackToSpotPriceObjectItShouldCaptureAllKeyElements()
        {
            var actualPrice = "1.60200/1.83430".ToSpotPrice("N/A");

            Assert.That(actualPrice.BidPrice, Is.EqualTo(1.60200));
            Assert.That(actualPrice.AskPrice, Is.EqualTo(1.83430));
            Assert.That(actualPrice.CurrencyPair, Is.EqualTo("N/A"));
        }

        [Test]
        public  void WhenGettingMovingAverageForMarketPrices()
        {
            var prices = new[] 
            {
                new MarketPrice() { Close = 1 },
                new MarketPrice() { Close = 2 },
                new MarketPrice() { Close = 3 },
                new MarketPrice() { Close = 1 },
                new MarketPrice() { Close = 2 },
                new MarketPrice() { Close = 3 },
                new MarketPrice() { Close = 1 },
                new MarketPrice() { Close = 2 },
                new MarketPrice() { Close = 3 },
            };
            IEnumerable<MarketPrice> inputSignals = prices;
            var result = inputSignals.MovingAverage(3).ToList();
            Assert.That(result, Has.Count.EqualTo(7));

            foreach(var r in result)
            {
                Console.WriteLine(r.ToString());
                // Price Predicted is the average
                Assert.That(r.PricePredicted, Is.EqualTo(2));               

                // Signal is number of stdevs away from mean, so when price == mean it should be zero
                if (r.Price == r.PricePredicted)
                    Assert.That(r.Signal, Is.EqualTo(0));                
            }
        }
    }
}
