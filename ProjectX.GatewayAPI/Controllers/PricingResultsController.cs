using Microsoft.AspNetCore.Mvc;
using ProjectX.Core.Services;

namespace ProjectX.GatewayAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PricingResultsController
    {
        private readonly ILogger<PricingResultsController> _logger;

        public PricingResultsController(ILogger<PricingResultsController> logger)
        {
            this._logger = logger;
        }

        [HttpGet]   
        public string Get()
        {
            return "Hello from Pricing Results";
        }

        [HttpPost]
        public void PricingResultsTaskCompleted(OptionsPricingResults results)
        {
            _logger.LogInformation($"PricingResults TaskCompleted for RequestId:{results.RequestId}, {results.ResultsCount} sets of option results");
        }
    }
}
