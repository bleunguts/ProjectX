using ProjectX.Core.Services;

namespace ProjectX.GatewayAPI.ExternalServices
{
    public interface IPricingResultsApiClient
    {
        Task PostResultAsync(OptionsPricingResults result, CancellationToken cancellationToken = default);
    }
}