using Caliburn.Micro;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using ProjectX.Core;
using ProjectX.Core.Requests;
using ProjectX.Core.Services;
using ProjectX.Core.Strategy;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Shell
{
    public interface IGatewayApiClient
    {
        HubConnection HubConnection { get; }
        Task SubmitPricingRequest(OptionsPricingByMaturitiesRequest pricingRequest, CancellationToken cancellationToken);
        Task StartHubAsync();
        Task StopHubAsync();
        Task SubmitPlotRequest(PlotOptionsPricingRequest request, CancellationToken token);
        Task SubmitFxRateSubscribeRequest(SpotPriceRequest spotPriceRequest, CancellationToken token);
        Task SubmitFxRateUnsubscribeRequest(SpotPriceRequest spotPriceRequest, CancellationToken token);
        Task<IEnumerable<MatrixStrategyPnl>> ComputeLongShortPnlMatrix(string ticker, DateTime fromDate, DateTime toDate, int notional);
        Task<StrategyResults> ComputeLongShortPnl(string ticker, DateTime fromDate, DateTime toDate, int movingWindow, int notional, double signalIn, double signalOut, MovingAverageImpl movingAverageImpl);
        Task<IEnumerable<double?>> GetHurst(string ticker, DateTime fromDate, DateTime toDate);
        Task<IEnumerable<PriceSignal>> GetMovingAverageSignals(string ticker, DateTime fromDate, DateTime toDate, int movingWindow, MovingAverageImpl movingAverageImpl);
    }

    [Export(typeof(IGatewayApiClient)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class GatewayApiClient : IGatewayApiClient
    {       
        private readonly HttpClient _httpClient;
        private readonly ILogger<GatewayApiClient> _logger;
        private readonly IHubConnector _hubConnector;

        [ImportingConstructor]
        public GatewayApiClient(ILogger<GatewayApiClient> logger,
            IHubConnector hubConnector,
            IOptions<GatewayApiClientOptions> options)
        {
            _logger = logger;
            _hubConnector = hubConnector;                    
            _httpClient = new HttpClient();

            var endpointAddress = options?.Value?.BaseUrl ?? string.Empty;

            if (string.IsNullOrEmpty(endpointAddress))
                throw new Exception("Invalid API base address");

            _httpClient.BaseAddress = new Uri(endpointAddress);
        }
        public HubConnection HubConnection => _hubConnector.Connection;
        public async Task<IEnumerable<double?>> GetHurst(string ticker, DateTime fromDate, DateTime toDate)
        {
            return await Get<IEnumerable<double?>>(Endpoints.StockSignalHursts, new Dictionary<string, string>()
            {
                {"ticker", ticker },
                {"fromDate", fromDate.ToString("yyyy-MM-dd") },
                {"toDate", toDate.ToString("yyyy-MM-dd") }
            }); ;
        }

        public async Task<IEnumerable<PriceSignal>> GetMovingAverageSignals(string ticker, DateTime fromDate, DateTime toDate, int movingWindow, MovingAverageImpl movingAverageImpl)
        {
            return await Get<IEnumerable<PriceSignal>>(Endpoints.StockSignalMovingAverage, new Dictionary<string, string>()
            {
                {"ticker", ticker },
                {"fromDate", fromDate.ToString("yyyy-MM-dd") },
                {"toDate", toDate.ToString("yyyy-MM-dd") },
                {"movingWindow", movingWindow.ToString() },
                {"movingAverageImpl", ((int)movingAverageImpl).ToString() }
            });
        }
        public async Task<StrategyResults> ComputeLongShortPnl(string ticker, DateTime fromDate, DateTime toDate, int movingWindow, int notional, double signalIn, double signalOut, MovingAverageImpl movingAverageImpl)
        {
            return await Get<StrategyResults>(Endpoints.BacktestLongShortPnl, new Dictionary<string, string>()
            {
                {"ticker", ticker },
                {"fromDate", fromDate.ToString("yyyy-MM-dd") },
                {"toDate", toDate.ToString("yyyy-MM-dd") },
                {"notional", notional.ToString() },
                {"movingWindow", movingWindow.ToString() },
                {"signalIn", signalIn.ToString() },
                {"signalOut", signalOut.ToString() },
                {"movingAverageImpl", ((int)movingAverageImpl).ToString() }
            }); 
        }
        public async Task<IEnumerable<MatrixStrategyPnl>> ComputeLongShortPnlMatrix(string ticker, DateTime fromDate, DateTime toDate, int notional)
        {
            return await Get<IEnumerable<MatrixStrategyPnl>>(Endpoints.BacktestLongShortPnlMatrix, new Dictionary<string, string>()
            {
                {"ticker", ticker },
                {"fromDate", fromDate.ToString("yyyy-MM-dd") },
                {"toDate", toDate.ToString("yyyy-MM-dd") },
                {"notional", notional.ToString() },
            }); 
        }

        public async Task StartHubAsync() => await _hubConnector.Start();
        public async Task StopHubAsync() => await _hubConnector.Stop();

        public async Task SubmitFxRateSubscribeRequest(SpotPriceRequest spotPriceRequest, CancellationToken token)
        {
            await Submit<SpotPriceRequest>(spotPriceRequest, Endpoints.FXSpotPriceStart, token);
        }

        public async Task SubmitFxRateUnsubscribeRequest(SpotPriceRequest spotPriceRequest, CancellationToken token)
        {
            await Submit<SpotPriceRequest>(spotPriceRequest, Endpoints.FXSpotPriceEnd, token);
        }

        public async Task SubmitPlotRequest(PlotOptionsPricingRequest pricingRequest, CancellationToken token)
        {            
            await Submit<PlotOptionsPricingRequest>(pricingRequest, Endpoints.RequestsPlotOptionBS, token);
        }
        public async Task SubmitPricingRequest(OptionsPricingByMaturitiesRequest optionsPricingByMaturitiesRequest, CancellationToken cancellationToken)
        {            
            await Submit<OptionsPricingByMaturitiesRequest>(optionsPricingByMaturitiesRequest, Endpoints.RequestsPriceOptionBS, cancellationToken);
        }
        private async Task<T> Get<T>(string endPoint, Dictionary<string, string> query)
        {
            var queryBuilder = new QueryBuilder(query);
            string? baseUri = endPoint + queryBuilder;

            var requestUri = new Uri(_httpClient.BaseAddress, baseUri);
            var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            { 
                _logger.LogWarning($"Failed to get request. {request} failed.");
                throw new ApplicationException($"GET {request} failed.");
            }

            var content =  response.Content;
            var json = await content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<T>(json);
            if (result == null)
            {
                _logger.LogWarning($"Failed to convert response content from json string to object. {json} parsing issue.");
                throw new ApplicationException($"GET json conversion failed {json}.");
            }
            return result;
        }
        private async Task<object> Submit<T>(T theRequest, string endPoint, CancellationToken cancellationToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, endPoint)
            {
                Content = new StringContent(System.Text.Json.JsonSerializer.Serialize<T>(theRequest), Encoding.UTF8, "application/json")
            };
            var response = await _httpClient.SendAsync(request, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var content = await request.Content.ReadAsStringAsync();
                _logger.LogWarning($"Failed to send request. {request.Method} {request.RequestUri} failed with content: {content}");
                throw new ApplicationException($"{request.Method} {request.RequestUri} failed with content: {content}");
            }

            return response;
        }

        public override string ToString()
        {            
            return $"BackendAddress={_httpClient.BaseAddress}";
        }
    }
}
