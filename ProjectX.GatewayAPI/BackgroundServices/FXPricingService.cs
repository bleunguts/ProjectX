using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using ProjectX.Core.Requests;
using ProjectX.Core.Services;
using ProjectX.GatewayAPI.Processors;
using System.Reactive;

namespace ProjectX.GatewayAPI.BackgroundServices
{
    public class FXPricingService : BackgroundService
    {
        private readonly ILogger<FXPricingService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IHostApplicationLifetime _hostApplicationLifetime;
        private readonly FXTasksChannel _fxTaskChannel;
        private readonly IFXMarketService _fxMarketService;

        private Dictionary<string, IDisposable> _disposables = new Dictionary<string, IDisposable>(); 

        public FXPricingService(ILogger<FXPricingService> logger,
            FXTasksChannel fXTasksChannel,
            IFXMarketService fxMarketService,           
            IServiceProvider serviceProvider,
            IHostApplicationLifetime hostApplicationLifetime)
        {
            _logger = logger;
            _fxTaskChannel = fXTasksChannel;
            _fxMarketService = fxMarketService;
            _serviceProvider = serviceProvider;
            _hostApplicationLifetime = hostApplicationLifetime;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Yield();

            try
            {
                await foreach (var request in _fxTaskChannel.ReadAllAsync(stoppingToken))
                {
                    _logger.LogInformation($"Read message {request} to process from channel. Request={request}");                    

                    using var scope = _serviceProvider.CreateScope();
                    var hubContext= scope.ServiceProvider.GetRequiredService<IHubContext<StreamHub, IStreamHub>>();
                    
                    switch(request.Mode)
                    {
                        case SpotPriceSubscriptionMode.Subscribe:
                            if (_disposables.ContainsKey(request.CurrencyPair))
                            {
                                break;
                            }

                            var disposable = _fxMarketService.StreamSpotPricesFor(request)
                                            .Subscribe(priceResponse => hubContext.Clients.All.PushFxRate(new SpotPriceResult(request.ClientName, priceResponse.Timestamp, priceResponse.Value)));
                            if(!_disposables.TryAdd(request.CurrencyPair, disposable))
                            {
                                _logger.LogWarning($"Disposable stream already added {request.CurrencyPair}");
                            }
                            break;
                       case SpotPriceSubscriptionMode.Unsubscribe:
                            _fxMarketService.UnStream(request.CurrencyPair);
                            if(_disposables.TryGetValue(request.CurrencyPair, out IDisposable d))
                            {
                                d.Dispose();
                                _disposables.Remove(request.CurrencyPair);
                            }
                            await hubContext.Clients.All.StopFxRate(request.CurrencyPair);
                            break;
                        default:
                            throw new NotSupportedException($"SpotRequest type {request.Mode} is not supported");
                    }
                    
                    // Then can think about supporting TradeOrderControler BuyTrade and SellTrade endpoint
                    //  TradeOrderBackgroundService that listens to Order requests
                    //  TradeOrderController.PositionManagement summary
                    _logger.LogInformation("finished processing message {Id} from channel.", request);
                }
            }
            catch (OperationCanceledException)
            {
                _logger.Log(LogLevel.Information, "A task/operation cancelled exception was caught.");
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "An unhandled exception was thrown.  Triggering app shutdown.");
            }
            finally
            {
                _hostApplicationLifetime.StopApplication();
            }
        }

        private object PublishPrice(string ccyPair, Timestamped<SpotPriceResponse> priceResponse)
        {
            throw new NotImplementedException();
        }
    }
}
