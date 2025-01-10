using ProjectX.Core.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Subjects;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ProjectX.Core;

public interface IFXSpotPriceStream
{
    IConnectableObservable<SpotPrice> SpotPriceEventsFor(string currencyPair);
}

public interface IFXSpotPricer
{
    SpotPrice Price(string ccyPair, SpotPrice spotPrice, int spreadInPips);
}

public enum FXProductType
{
    Spot
    // Outright
    // Swap
    // Options
}
public record SpreadedSpotPriceResponse(SpotPrice SpotPrice, string ClientName, int SpreadInPips) : SpotPriceResponse(SpotPrice, ClientName);
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
