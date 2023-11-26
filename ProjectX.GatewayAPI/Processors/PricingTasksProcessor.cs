using ProjectX.Core;
using ProjectX.Core.Requests;
using ProjectX.Core.Services;
using ProjectX.GatewayAPI.ExternalServices;
using System.Collections.Concurrent;
using System.Reflection.Metadata.Ecma335;

namespace ProjectX.GatewayAPI.Processors
{
    public class PricingTasksProcessor : IPricingTasksProcessor
    {
        private readonly ILogger<PricingTasksProcessor> _logger;
        private readonly IOptionsPricingModel _pricingModel;
        private readonly IPricingResultsApiClient _pricingResultsApiClient;
        private readonly ConcurrentDictionary<Guid, OptionsPricingByMaturityResults> _responses = new ConcurrentDictionary<Guid, OptionsPricingByMaturityResults>();

        public PricingTasksProcessor(ILogger<PricingTasksProcessor> logger, 
            IOptionsPricingModel pricingModel,
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
                ThrowIfTaskFailed(r.IsFaulted, r.Exception);

                var pricingResult = r.Result;
                ThrowIfResultIsInvalid(pricingResult);

                if (pricingResult is OptionsPricingByMaturityResults)
                {
                    return _pricingResultsApiClient.PostResultAsync((OptionsPricingByMaturityResults)pricingResult);
                }
                if (pricingResult is PlotOptionsPricingResult)
                {
                    return _pricingResultsApiClient.PostResultAsync((PlotOptionsPricingResult)pricingResult);
                }
                throw new NotSupportedException($"type {pricingResult.GetType()} is not supported.");
            }, cancellationToken);
        }

        private void ThrowIfResultIsInvalid(object? pricingResult)
        {
            if (pricingResult != null) return;
            
            _logger.LogError("PricingModel processing returned null.");
            throw new ApplicationException("PricingModel processing returned null.");
        }

        private void ThrowIfTaskFailed(bool isFaulted, Exception ex)
        {
            if (!isFaulted) return;
            if (ex == null) return;

            while (ex is AggregateException && ex.InnerException != null)
                ex = ex.InnerException;

            _logger.LogError(ex, "PricingModel processing processing threw an error.");
            throw new ApplicationException("PricingModel processing threw an error.", ex);
        }
    }
}
