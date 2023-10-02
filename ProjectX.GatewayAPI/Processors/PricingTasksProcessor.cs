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
        private readonly ConcurrentDictionary<Guid, OptionsPricingResults> _responses = new ConcurrentDictionary<Guid, OptionsPricingResults>();

        public PricingTasksProcessor(ILogger<PricingTasksProcessor> logger, 
            IBlackScholesOptionsPricingModel pricingModel,
            IPricingResultsApiClient pricingResultsApiClient)
        {
            this._logger = logger;
            this._pricingModel = pricingModel;
            this._pricingResultsApiClient = pricingResultsApiClient;
        }
        public Task Process(MultipleTimeslicesOptionsPricingRequest pricingRequest)
        {
            _logger.LogInformation($"Spin off calculator to price RequestId=[{pricingRequest.Id}]");

            var pricingTask = new Task<OptionsPricingResults>(() =>
            {
                var pricingResult = _pricingModel.Price(pricingRequest);
                _logger.LogInformation($"Priced successfully {pricingResult.ToString()}");
                return pricingResult;
            });

            pricingTask.Start();
            pricingTask.ContinueWith((results) =>
            {
                var pricingResult = results.Result;
                _logger.LogInformation($"Posting Pricing Results to Endpoint ... {pricingResult.ToString()}");
                _pricingResultsApiClient.PostResultAsync( pricingResult );  
            });
            return pricingTask;
        }
    }
}
