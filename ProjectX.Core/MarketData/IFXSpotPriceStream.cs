using System.Reactive.Subjects;

namespace ProjectX.Core.MarketData
{
    public interface IFXSpotPriceStream
    {
        IConnectableObservable<SpotPrice> SpotPriceEventsFor(string currencyPair);
    }
}