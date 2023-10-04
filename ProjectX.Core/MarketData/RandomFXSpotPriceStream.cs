using Microsoft.Extensions.Options;
using System.ComponentModel.Composition;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace ProjectX.Core.MarketData
{    
    public class RandomFXSpotPriceStreamOptions
    {
        public decimal RawSpreadInPips { get; set; } = 5;
        public int IntervalBetweenSends { get; set; } = 1000;
    }
    public class RandomFXSpotPriceStream : IFXSpotPriceStream
    {
        private readonly decimal _rawSpreadInPips = 2;
        private readonly int _intervalBetweenSends = 2000;
        private Random _random = new Random();

        public RandomFXSpotPriceStream(IOptions<RandomFXSpotPriceStreamOptions> options)
        {
            _rawSpreadInPips = options.Value.RawSpreadInPips;
            _intervalBetweenSends = options.Value.IntervalBetweenSends;
        }

        public RandomFXSpotPriceStream(decimal rawSpreadInPips, int intervalBetweenSends)
        {
            this._rawSpreadInPips = rawSpreadInPips;
            this._intervalBetweenSends = intervalBetweenSends;
        }

        public IConnectableObservable<SpotPrice> SpotPriceEventsFor(string currencyPair) =>
                                    Observable.Interval(TimeSpan.FromMilliseconds(_intervalBetweenSends))
                                                .Select<long, SpotPrice>(l => GenerateSpotPrice(currencyPair))
                                                .Publish();

        internal SpotPrice GenerateSpotPrice(string currencyPair)
        {
            //random spot price 1.6420 +- 10 pips
            // get a random midpoint - fluctuate by 10 pips
            var rawMid = (decimal)(1.6420 + _random.NextDouble() / 100);

            // apply spread
            var difference = (_rawSpreadInPips / 10000M) / 2M;
            var bidPrice = rawMid - difference;
            var askPrice = rawMid + difference;
            return new SpotPrice(currencyPair, bidPrice, askPrice);
        }
    }
}


