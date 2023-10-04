using System.Reactive;
using ProjectX.Core.Requests;

namespace ProjectX.Core.MarketData
{
    public interface IFXMarketService
    {
        IObservable<Timestamped<SpotPriceResponse>> StreamSpotPricesFor(SpotPriceRequest request);
        void UnStream(string currencyPair);
    }
}