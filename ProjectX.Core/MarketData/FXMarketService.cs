using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Collections.Concurrent;
using System.Reactive.Linq;
using System.Diagnostics;
using System.Reactive;

namespace ProjectX.Core.MarketData
{
    public class FXMarketService: IDisposable
    {
        private readonly int RawSpreadInPips = 2;
        private readonly ILogger<FXMarketService> _logger;
        private readonly IFXSpotPriceGenerator _spotPriceGenerator;        
        private readonly IFXPricer _fxPricer;
        private readonly IDictionary<string, IDisposable> _spotPriceStreams = new ConcurrentDictionary<string, IDisposable>();        

        public FXMarketService(ILogger<FXMarketService> logger, IFXSpotPriceGenerator spotPriceGenerator, IFXPricer fXPricer) 
        {
            _logger = logger;
            _spotPriceGenerator = spotPriceGenerator;            
            _fxPricer = fXPricer;
        }
        public IObservable<Timestamped<SpotPriceResponse>> StreamSpotPricesFor(SpotPriceRequest request)
        {            
            var spotPriceResponseStream = _spotPriceGenerator.SpotPriceEventsFor(request.CurrencyPair)
                                    //.ObserveOn(NewThreadScheduler.Default)
                                    .AutoConnect(1)
                                    .Select(spotPrice => Price(request, spotPrice, RawSpreadInPips))
                                    .Timestamp();                                           

            var disposable = spotPriceResponseStream                             
                                    .Subscribe(priceResponse => PriceUpdated(priceResponse),
                                               exception => _logger.LogWarning($"PriceResponse stream error, Reason:'{exception.Message}'"));
                      
            if (!_spotPriceStreams.TryAdd(request.CurrencyPair, disposable))
            {
                _logger.LogWarning($"Tried to add {request.CurrencyPair} observable stream to internal dictionary but failed");
            };
            
            return spotPriceResponseStream;
        }

        public void Unsubscribe(string currencyPair)
        {
            if (_spotPriceStreams.TryGetValue(currencyPair, out var disposable))
            {
                disposable.Dispose();
            }            
        }

        private void PriceUpdated(Timestamped<SpotPriceResponse> priceResponse)
        {
            _logger.LogInformation("PriceReceived [{0}]: {1}", priceResponse.Timestamp, PrettyPrint(priceResponse.Value.SpotPrice));

            static string PrettyPrint(SpotPrice price) => $"ccyPair={price.CurrencyPair}, Price={price.BidPrice:#.00000}/{price.AskPrice.ToString("#.00000")}, PriceId={price.PriceId}";
        }        
        private SpotPriceResponse Price(SpotPriceRequest request, SpotPrice spotPrice, int spreadInPips)
        {
            _logger.LogInformation($"Pricing Request {request.CurrencyPair}, {spotPrice}, {spreadInPips}");
            
            var price = _fxPricer.Price(request.CurrencyPair, spotPrice, spreadInPips);
            return new SpreadedPriceResponse(price, request.ClientName, spreadInPips);
        }

        public void Dispose()
        {          
            foreach(var stream in _spotPriceStreams.Values)
            {
                stream.Dispose();                
            }
            _spotPriceStreams.Clear();
        }
    }
}


