using System.Reactive.Subjects;

namespace ProjectX.Core.MarketData
{
    public interface IFXSpotPriceGenerator
    {
        IConnectableObservable<SpotPrice> SpotPriceEventsFor(string currencyPair);
    }
}