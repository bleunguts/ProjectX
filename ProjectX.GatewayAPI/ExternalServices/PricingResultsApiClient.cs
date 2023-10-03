using System.Text.Json;
using System.Text;
using ProjectX.Core.Services;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Builder.Extensions;

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

    public async Task PostResultAsync(OptionsPricingResults result, CancellationToken cancellationToken = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "PricingResults")
        {
            Content = new StringContent(JsonSerializer.Serialize(result), Encoding.UTF8, "application/json")
        };

        using var response = await _httpClient.SendAsync(request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("Failed to send match result.");
        }
    }
}
