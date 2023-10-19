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
        public static IEnumerable<PriceSignalEntity> MovingAverage(this List<MarketPrice> rawSignals, int movingWindow)
        {
            string ticker = "foo";
            var computedSignals = new List<PriceSignalEntity>();

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
                decimal zscore = (price - avg) / std;
                computedSignals.Add(new PriceSignalEntity
                {
                    Ticker = ticker,
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
