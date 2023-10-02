using ProjectX.Core.Requests;
using ProjectX.Core.Services;

namespace ProjectX.GatewayAPI.Processors
{
    public class PricingTasksProcessor : IPricingTasksProcessor
    {
        private readonly ILogger<PricingTasksProcessor> _logger;
        private readonly IBlackScholesOptionsPricingModel _pricingModel;

        public PricingTasksProcessor(ILogger<PricingTasksProcessor> logger, IBlackScholesOptionsPricingModel pricingModel)
        {
            this._logger = logger;
            this._pricingModel = pricingModel;
        }
        public async Task Process(MultipleTimeslicesOptionsPricingRequest request)
        {
            _logger.LogInformation("Invoking quant libraries to price request");
            
            var result = _pricingModel.Price(request);

            // what do I do here with the result
            // 
        }
    }
}
