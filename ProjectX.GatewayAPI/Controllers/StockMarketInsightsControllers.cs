using Microsoft.AspNetCore.Mvc;
using ProjectX.Core;
using ProjectX.Core.Requests;
using ProjectX.GatewayAPI.BackgroundServices;

namespace ProjectX.GatewayAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StockMarketInsightsController : ControllerBase
    {        
        private readonly ILogger<StockMarketInsightsController> _logger;
        private readonly IStockMarketSource _stockMarketSource;

        public StockMarketInsightsController(ILogger<StockMarketInsightsController> logger, IStockMarketSource stockMarketSource)
        {
            _logger = logger;
            _stockMarketSource = stockMarketSource;
        }

        [HttpGet]
        public string Get()
        {
            return "Hello from the StockMarket insights module";
        }

        [HttpGet("HighestGainerStocks/{limit:int?}")]
        public async Task<IEnumerable<StockMarketSymbol>> HighestGainerStocks(int? limit = 50)
        {
            var stocks = await _stockMarketSource.GetHighestGainerStocks();
            _logger.LogInformation($"Request to fetch Highest Gainers Stock completed with {stocks.Count()} stocks");
            return stocks.Take(limit.Value);
        }

        [HttpGet("MostActiveStocks/{limit:int?}")]
        public async Task<IEnumerable<StockMarketSymbol>> MostActiveStocks(int? limit = 50)
        {
            var stocks = await _stockMarketSource.GetMostActiveStocks();
            _logger.LogInformation($"Request to fetch Highest Gainers Stock completed with {stocks.Count()} stocks");
            return stocks.Take(limit.Value);
        }
    }
}