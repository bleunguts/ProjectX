using Microsoft.AspNetCore.Mvc;

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
        public void PricingResultsTaskCompleted()
        {
            _logger.LogInformation($"PricingResults Task completed.");
        }
    }
}
