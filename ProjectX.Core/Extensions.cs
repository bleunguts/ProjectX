using ProjectX.Core.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectX.Core
{
    public static class Extensions
    {
        public static string Dump(this OptionsPricingByMaturityResults o) => $"{o.ResultsCount} results, maturties: {o.Maturities()}, prices: {o.Prices()}";
        public static string Maturities(this OptionsPricingByMaturityResults o) => o.Results == null ? string.Empty : string.Join(',', o.Results.Select(x => x.Maturity));
        public static string Prices(this OptionsPricingByMaturityResults o) => o.Results == null ? string.Empty : string.Join(',', o.Results.Select(x => x.OptionGreeks.price));
        public static decimal Spread(this SpotPrice s) => Decimal.Truncate((Math.Abs(s.BidPrice - s.AskPrice)) * 1000);
        public static DateTime ToDateTime(this string s) => DateTime.Parse(s);
        public static bool IsBetween(this DateTime dateTime, DateTime from, DateTime to) => dateTime > from && dateTime < to;
        public static string ToPrettifiedBidAskPrice(this SpotPrice price) => $"{price.BidPrice.ToString("#.00000")}/{price.AskPrice.ToString("#.00000")}";
        public static SpotPrice ToSpotPrice(this string spotPrice, string selectedCurrency)
        {
            var parts = spotPrice.Split('/');
            var bidPrice = Convert.ToDecimal(parts[0].Trim());
            var askPrice = Convert.ToDecimal(parts[1].Trim());

            return new SpotPrice(selectedCurrency, bidPrice, askPrice);
        }
        public static decimal StdDev<T>(this IEnumerable<T> list, Func<T, decimal> values)
        {
            var mean = 0.0M;
            var stdDev = 0.0M;
            var n = 0;

            n = 0;
            foreach (var value in list.Select(values))
            {
                n++;
                mean += value;
            }
            mean /= n;

            foreach (var value in list.Select(values))
            {
                stdDev += (value - mean) * (value - mean);
            }            
            stdDev = (decimal)Math.Sqrt((double)stdDev / (n - 1));
            return stdDev;
        }
    }
}
