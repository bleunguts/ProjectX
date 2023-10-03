using System.Text.Json;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Builder.Extensions;
using ProjectX.Core.Requests;
using ProjectX.Core;

namespace ProjectX.GatewayAPI.ExternalServices;

public class PricingResultsApiClient : IPricingResultsApiClient
{    
    private readonly HttpClient _httpClient;
    private readonly ILogger<PricingResultsApiClient> _logger;

    public PricingResultsApiClient(HttpClient httpClient, 
        ILogger<PricingResultsApiClient> logger, 
        IOptions<ApiClientOptions> options)
    {
        this._httpClient = httpClient;
        this._logger = logger;
        var endpointAddress = options?.Value?.BaseAddress ?? string.Empty;

        if (string.IsNullOrEmpty(endpointAddress))
            throw new Exception("Invalid API base address");

        _httpClient.BaseAddress = new Uri(endpointAddress);
    }

    public async Task PostResultAsync(OptionsPricingByMaturityResults result, CancellationToken cancellationToken = default)
    {
        using HttpResponseMessage response = await PostResult<OptionsPricingByMaturityResults>(result, Endpoints.ResultsPriceOptionBS, cancellationToken);
    }
    public async Task PostResultAsync(PlotOptionsPricingResult result, CancellationToken cancellationToken = default)
    {
        using HttpResponseMessage response = await PostResult<PlotOptionsPricingResult>(result, Endpoints.ResultsPlotOptionBS, cancellationToken);
    }

    private async Task<HttpResponseMessage> PostResult<T>(T result, string endpoint, CancellationToken cancellationToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, endpoint)
        {
            Content = new StringContent(JsonSerializer.Serialize<T>(result), Encoding.UTF8, "application/json")
        };
        var response = await _httpClient.SendAsync(request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("Failed to send match result.");
        }

        return response;
    }

    
}
