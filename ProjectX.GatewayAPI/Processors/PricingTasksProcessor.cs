using ProjectX.Core;
using ProjectX.Core.Requests;
using ProjectX.Core.Services;
using ProjectX.GatewayAPI.ExternalServices;
using System.Collections.Concurrent;

namespace ProjectX.GatewayAPI.Processors
{
    public class PricingTasksProcessor : IPricingTasksProcessor
    {
        private readonly ILogger<PricingTasksProcessor> _logger;
        private readonly IBlackScholesOptionsPricingModel _pricingModel;
        private readonly IPricingResultsApiClient _pricingResultsApiClient;
        private readonly ConcurrentDictionary<Guid, OptionsPricingByMaturityResults> _responses = new ConcurrentDictionary<Guid, OptionsPricingByMaturityResults>();

        public PricingTasksProcessor(ILogger<PricingTasksProcessor> logger, 
            IBlackScholesOptionsPricingModel pricingModel,
            IPricingResultsApiClient pricingResultsApiClient)
        {
            this._logger = logger;
            this._pricingModel = pricingModel;
            this._pricingResultsApiClient = pricingResultsApiClient;
        }
        public Task Process(IRequest pricingRequest, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Spin off calculator to price RequestId=[{pricingRequest.Id}]");

            return Task.Run<object>(() =>
            {
                if (pricingRequest is OptionsPricingByMaturitiesRequest)
                {
                    return _pricingModel.Price((OptionsPricingByMaturitiesRequest) pricingRequest);
                }
                if (pricingRequest is PlotOptionsPricingRequest)
                {
                    return _pricingModel.PlotGreeks((PlotOptionsPricingRequest)pricingRequest);
                }                          
                
                throw new NotSupportedException($"type {pricingRequest.GetType()} is not supported.");
            }).ContinueWith(r =>
            {
                var pricingResult = r.Result;
                if(pricingResult is OptionsPricingByMaturityResults)
                {
                    return _pricingResultsApiClient.PostResultAsync((OptionsPricingByMaturityResults)pricingResult);
                }
                if(pricingResult is PlotOptionsPricingResult)
                {
                    return _pricingResultsApiClient.PostResultAsync((PlotOptionsPricingResult)pricingResult);
                }
                throw new NotSupportedException($"type {pricingResult.GetType()} is not supported.");
            }, cancellationToken);
        }
    }
}
