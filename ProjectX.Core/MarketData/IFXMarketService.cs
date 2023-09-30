using System.Reactive;

namespace ProjectX.Core.MarketData
{
    public interface IFXMarketService
    {
        IObservable<Timestamped<SpotPriceResponse>> StreamSpotPricesFor(SpotPriceRequest request);
        void UnStream(string currencyPair);
    }
}