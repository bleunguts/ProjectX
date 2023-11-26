using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using ProjectX.Core.Requests;

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

        [HttpPost("bsPrice")]
        public void PricingResultsTaskCompletedAsync(OptionsPricingByMaturityResults results)
        {
            _logger.LogInformation($"OptionsPricingResults bsPrice TaskCompleted. RequestId:{results.RequestId}, There are {results.ResultsCount} option results and maturity pairs and it took {results.AuditTrail.ElapsedMilliseconds} ms to price.");
            _hubContext.Clients.All.PricingResults(results);
        }

        [HttpPost("bsPlot")]
        public void PricingResultsPlotTaskCompletedAsync(PlotOptionsPricingResult results)
        {
            _logger.LogInformation($"OptionsPricingResults bsPlot TaskCompleted. RequestId:{results.RequestId}");
            _hubContext.Clients.All.PlotResults(results);
        }

    }
}
