using Microsoft.AspNetCore.Mvc;
using ProjectX.Core.Requests;

namespace ProjectX.GatewayAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PricingTasksController : ControllerBase
    {        
        private readonly ILogger<PricingTasksController> _logger;

        public PricingTasksController(ILogger<PricingTasksController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public string Get()
        {
            return "Hello World";
        }

        [HttpPost]
        public void BlackScholesPricingRequestAsync(MultipleTimeslicesOptionsPricingRequest request)
        {
            _logger.LogInformation($"Pricing Request recived: {request}");

            _logger.LogInformation($"Sending pricing request to the Background Service");

        }        
    }
}