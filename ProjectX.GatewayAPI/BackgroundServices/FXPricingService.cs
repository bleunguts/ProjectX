using Microsoft.Extensions.Hosting;
using ProjectX.Core.Requests;
using ProjectX.GatewayAPI.Processors;
using System.Threading.Channels;

namespace ProjectX.GatewayAPI.BackgroundServices
{
    public class FXPricingService : BackgroundService
    {
        private readonly ILogger<FXPricingService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IHostApplicationLifetime _hostApplicationLifetime;
        private readonly RequestTasksChannel _requestTaskChannel;

        public FXPricingService(ILogger<FXPricingService> logger,
            IServiceProvider serviceProvider,
            IHostApplicationLifetime hostApplicationLifetime)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _hostApplicationLifetime = hostApplicationLifetime;
            _requestTaskChannel = new RequestTasksChannel();
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Yield();

            try
            {
                await foreach (var request in _requestTaskChannel.ReadAllAsync(stoppingToken))
                {
                    _logger.LogInformation($"Read message {request} to process from channel. Request={request}");

                    // Establish channel pipe with FXRatesController/Subscribe endpoint

                    // Generate Prices for currency pair and expose Observable stream
                    // Create a processor which will invoke WebApi to send prices to SignalR channel to Client                    
                    //public void Subscribe()
                    //{
                    //    if (_currentDisposableStream != null)
                    //    {
                    //        _currentDisposableStream.Dispose();
                    //    }
                    //    var selectedCurrency = SelectedCurrency ?? throw new ArgumentNullException(nameof(SelectedCurrency));
                    //    _currentDisposableStream = _fXMarketService.StreamSpotPricesFor(new ProjectX.Core.SpotPriceRequest(selectedCurrency, ClientName))
                    //        .Subscribe(priceResponse => UpdatePrice(priceResponse));
                    //    AppendStatus($"subscribed to price stream {selectedCurrency} successfully.");
                    //}

                    // Support Unsubscribe from currency pair
                    // Establish channel pipe with FXRatesController/Unsubscribe
                    //public void Unsubscribe()
                    //{
                    //    _fXMarketService.UnStream(SelectedCurrency);
                    //    if (_currentDisposableStream != null)
                    //    {
                    //        _currentDisposableStream.Dispose();
                    //        _currentDisposableStream = null;
                    //    }
                    //    AppendStatus($"Stream unsubscribed to {SelectedCurrency}.");
                    //}

                    // all this above code will happen in server side now 
                    // can remove from client
                    // all client does is starts subscribing to SignalR On<string>() to receive the FX rates

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
    }

    internal class RequestTasksChannel 
    {
        private Channel<string> _channel;
        public IAsyncEnumerable<string> ReadAllAsync(CancellationToken ct = default) => _channel.Reader.ReadAllAsync(ct);
    }
}
