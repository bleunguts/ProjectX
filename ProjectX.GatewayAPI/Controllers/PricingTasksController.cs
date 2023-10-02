using Microsoft.AspNetCore.Mvc;

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
    }
}