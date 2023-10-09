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
    }
}
