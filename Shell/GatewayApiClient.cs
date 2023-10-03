using Caliburn.Micro;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ProjectX.Core.MarketData;
using ProjectX.Core.Requests;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Shell
{
    public interface IGatewayApiClient
    {
        HubConnection HubConnection { get; }
        Task SubmitPricingRequest(MultipleTimeslicesOptionsPricingRequest pricingRequest, CancellationToken cancellationToken);
        Task StartHubAsync();
        Task StopHubAsync();
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
        public async Task StartHubAsync() => await _hubConnector.Start();
        public async Task StopHubAsync() => await _hubConnector.Stop();

        public async Task SubmitPricingRequest(MultipleTimeslicesOptionsPricingRequest pricingRequest, CancellationToken cancellationToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "PricingTasks")
            {                
                Content = new StringContent(JsonSerializer.Serialize(pricingRequest), Encoding.UTF8, "application/json")
            };

            using var response = await _httpClient.SendAsync(request, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var content = await request.Content.ReadAsStringAsync();
                _logger.LogWarning($"Failed to send request. {request.Method} {request.RequestUri} failed with content: {content}");
                throw new ApplicationException($"{request.Method} {request.RequestUri} failed with content: {content}");
            }
        }        
    }
}
