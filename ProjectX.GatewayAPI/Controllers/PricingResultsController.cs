using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using ProjectX.Core.Services;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace ProjectX.GatewayAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PricingResultsController
    {
        private readonly ILogger<PricingResultsController> _logger;
        private readonly IHubContext<StreamHub> _hubContext;

        public PricingResultsController(ILogger<PricingResultsController> logger, IHubContext<StreamHub> hubContext)
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
        public async Task PricingResultsTaskCompletedAsync(OptionsPricingResults results)
        {
            _logger.LogInformation($"PricingResults TaskCompleted for RequestId:{results.RequestId}, {results.ResultsCount} sets of option results");
            var json = JsonConvert.SerializeObject(results);
            await _hubContext.Clients.All.SendAsync("PricingResults", json);                                
        }
    }
}
