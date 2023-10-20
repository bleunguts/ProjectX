using ProjectX.Core.Strategy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectX.Core.Services
{
    public static class MarketPriceExtensions
    {
        public static IEnumerable<PriceSignal> MovingAverage(this IEnumerable<MarketPrice> r, int movingWindow)
        {
            var rawSignals = r.ToList();
            var computedSignals = new List<PriceSignal>();

            for (int i = movingWindow - 1; i < rawSignals.Count; i++)
            {
                var temp = new List<MarketPrice>();
                for (int j = i - movingWindow + 1; j <= i; j++)
                {
                    temp.Add(rawSignals[j]);
                }
                decimal avg = temp.Average(x => x.Close);
                decimal std = temp.StdDev(x => x.Close);
                decimal price = rawSignals[i].Close;
                decimal zscore = std == 0 ? 0 :(price - avg) / std;
                computedSignals.Add(new PriceSignal
                {
                    Ticker = rawSignals[i].Ticker,
                    Date = rawSignals[i].Date,
                    Price = rawSignals[i].Close,
                    PricePredicted = avg,
                    UpperBand = avg + 2.0M * std,
                    LowerBand = avg - 2.0M * std,
                    Signal = zscore
                });
            }
            return computedSignals;
        }
    }
}
