using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using ProjectX.Core.Services;

namespace ProjectX.GatewayAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PricingResultsController
    {
        private readonly ILogger<PricingResultsController> _logger;
        private readonly IHubContext<StreamHub, IStreamHub> _hubContext;

        public PricingResultsController(ILogger<PricingResultsController> logger, IHubContext<StreamHub, IStreamHub> hubContext)
        {
            _logger = logger;
            _hubContext = hubContext;
        }

        [HttpGet]   
        public string Get()
        {
            return "Hello from Pricing Results";
        }

        [HttpPost]
        public void PricingResultsTaskCompletedAsync(OptionsPricingResults results)
        {
            _logger.LogInformation($"PricingResults TaskCompleted for RequestId:{results.RequestId}, {results.ResultsCount} sets of option results");
            _hubContext.Clients.All.PricingResults(results);
        }
    }
}
