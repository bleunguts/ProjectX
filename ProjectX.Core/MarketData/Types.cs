using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ProjectX.Core.MarketData
{
    public enum ProductType
    {
        Spot
        // Outright
        // Swap
        // Options
    }
    public enum BuySell
    {     
        Buy,     
        Sell
    }

    public record SpotPriceRequest(string CurrencyPair, string ClientName)
    {
        public ProductType ProductType { private set; get; } = ProductType.Spot;
    }
    public record SpotPriceResponse(SpotPrice SpotPrice, string ClientName);

    public record struct SpotPrice(decimal BidPrice, decimal AskPrice, string CurrencyPair, Guid PriceId)
    {
        public SpotPrice(string currencyPair, decimal bidPrice, decimal askPrice) : this(bidPrice, askPrice, currencyPair, Guid.NewGuid())
        {
            CurrencyPair = currencyPair;
            BidPrice = bidPrice;
            AskPrice = askPrice;
        }

        public override string ToString()
        {
            return string.Format("ccyPair={0}, Price={1}/{2}, PriceId={3}", CurrencyPair, BidPrice.ToString("#.00000"), AskPrice.ToString("#.00000"), PriceId);
        }
    }

    public record SpreadedPriceResponse(SpotPrice SpotPrice, string ClientName, int SpreadInPips) : SpotPriceResponse(SpotPrice, ClientName);
}
