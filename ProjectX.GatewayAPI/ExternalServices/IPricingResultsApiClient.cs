using ProjectX.Core.Requests;

namespace ProjectX.GatewayAPI.ExternalServices
{
    public interface IPricingResultsApiClient
    {
        Task PostResultAsync(OptionsPricingByMaturityResults result, CancellationToken cancellationToken = default);
        Task PostResultAsync(PlotOptionsPricingResult pricingResult, CancellationToken cancellationToken = default);
    }
}