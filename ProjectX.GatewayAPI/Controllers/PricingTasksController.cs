using Microsoft.AspNetCore.Mvc;
using ProjectX.Core.Requests;
using ProjectX.GatewayAPI.BackgroundServices;

namespace ProjectX.GatewayAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PricingTasksController : ControllerBase
    {        
        private readonly ILogger<PricingTasksController> _logger;
        private readonly PricingTasksChannel _pricingTasksChannel;

        public PricingTasksController(ILogger<PricingTasksController> logger, PricingTasksChannel pricingTasksChannel)
        {
            _logger = logger;
            _pricingTasksChannel = pricingTasksChannel;
        }

        [HttpGet]
        public string Get()
        {
            return "Hello from the Pricing module";
        }

        [HttpPost("bsPrice")]        
        public async Task BlackScholesPricingRequestAsync(OptionsPricingByMaturitiesRequest request)
        {
            _logger.LogInformation($"Pricing Request recieved: {request}");            
            try
            {
                _logger.LogInformation($"Sending pricing request down the Pricing channel for the background service to pick up and process...");
                if (await _pricingTasksChannel.SendRequestAsync(request))
                {
                    _logger.LogInformation("Request sent successfully.");
                };                
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to send pricing request with error: {ex}");
            }
        }

        [HttpPost("bsPlot")]        
        public async Task BlackScholesPlotRequestAsync(PlotOptionsPricingRequest request)
        {
            _logger.LogInformation($"Pricing Request recieved: {request}");
            try
            {
                _logger.LogInformation($"Sending pricing request down the Pricing channel for the background service to pick up and process...");
                if (await _pricingTasksChannel.SendRequestAsync(request))
                {
                    _logger.LogInformation("Request sent successfully.");
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to send pricing request with error: {ex}");
            }
        }
    }
}