using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectX.Core.Requests
{
    public class SpotPriceResult
    {
        public string CcyPair { get; }
        public DateTimeOffset Timestamp { get; }
        public SpotPriceResponse SpotPriceResponse { get; }

        public SpotPriceResult(string ccyPair, DateTimeOffset timestamp, SpotPriceResponse spotPriceResponse)
        {
            this.CcyPair = ccyPair;
            this.Timestamp = timestamp;
            this.SpotPriceResponse = spotPriceResponse;
        }
    }
}
