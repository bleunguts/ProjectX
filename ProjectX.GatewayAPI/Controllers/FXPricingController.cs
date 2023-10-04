

using Microsoft.AspNetCore.Mvc;
using ProjectX.Core.Requests;
using ProjectX.GatewayAPI.BackgroundServices;

namespace ProjectX.GatewayAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FXPricingController
    {
        private readonly ILogger<FXPricingController> _logger;
        private readonly FXTasksChannel _fXTasksChannel;

        public FXPricingController(ILogger<FXPricingController> logger, FXTasksChannel fXTasksChannel)
        {
            this._logger = logger;
            this._fXTasksChannel = fXTasksChannel;
        }

        [HttpGet]
        public string Get() 
        {
            return "Hello from FXRatesController";
        }

        [HttpPost]
        public async Task Invoke(SpotPriceRequest request)
        {
            _logger.LogInformation($"FX subscription Request recieved: {request}");
            try
            {
                _logger.LogInformation($"Sending request down the FX channel for the background service to pick up and process...");
                if (await _fXTasksChannel.SendRequestAsync(request))
                {
                    _logger.LogInformation("Request for price subscription sent successfully.");
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to send FX subscription request with error: {ex}");
            }
        }
    }
}
