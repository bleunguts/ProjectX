using Microsoft.Extensions.Options;
using ProjectX.Core;
using System.ComponentModel.Composition;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace ProjectX.MarketData
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
            _rawSpreadInPips = rawSpreadInPips;
            _intervalBetweenSends = intervalBetweenSends;
        }

        public IConnectableObservable<SpotPrice> SpotPriceEventsFor(string currencyPair) =>
                                    Observable.Interval(TimeSpan.FromMilliseconds(_intervalBetweenSends))
                                                .Select(l => GenerateSpotPrice(currencyPair))
                                                .Publish();

        internal SpotPrice GenerateSpotPrice(string currencyPair)
        {
            //random spot price 1.6420 +- 10 pips
            // get a random midpoint - fluctuate by 10 pips
            double mid = currencyPair switch {
                "EURUSD" => 1.0510,
                "GBPUSD" => 1.2550,
                "USDJPY" => 149.94,
                "USDCHF" => 0.8953,
                "AUDUSD" => 0.6330,
                "USDCAD" => 1.3776,
                "NZDUSD" => 0.5821,
                "BTCUSD" => 34223,
                _ => 1.6420
            }; 
            var rawMid = (decimal)(mid + _random.NextDouble() / 100);

            // apply spread
            var difference = _rawSpreadInPips / 10000M / 2M;
            var bidPrice = rawMid - difference;
            var askPrice = rawMid + difference;
            return new SpotPrice(currencyPair, bidPrice, askPrice);
        }
    }
}


