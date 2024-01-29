using Microsoft.AspNetCore.Mvc;
using ProjectX.Core;
using ProjectX.Core.Services;
using ProjectX.Core.Strategy;

namespace ProjectX.GatewayAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class StockSignalController : ControllerBase
{
    private readonly ILogger<StockSignalController> _logger;
    private readonly IStockSignalService _stockSignalService;
    private readonly IStockMarketSource _stockMarketSource;

    public StockSignalController(ILogger<StockSignalController> logger, IStockSignalService stockSignalService, IStockMarketSource stockMarketSource)
    {
        _logger = logger;
        _stockSignalService = stockSignalService;
        _stockMarketSource = stockMarketSource;
    }

    [HttpGet("MovingAverageSignals")]
    public async Task<ActionResult<IEnumerable<PriceSignal>>> MovingAverageSignalAsync(string ticker, DateTime fromDate, DateTime toDate, int movingWindow, MovingAverageImpl movingAverageImpl)
    {
        var signals = await _stockSignalService.GetSignalUsingMovingAverageByDefault(ticker, fromDate, toDate, movingWindow, movingAverageImpl);

        return Ok(signals);
    }

    [HttpGet("Hursts")]
    public async Task<ActionResult<IEnumerable<double?>>> Hursts(string ticker, DateTime fromDate, DateTime toDate)
    {
        var signals = await _stockMarketSource.GetHurst(ticker, fromDate, toDate);

        return Ok(signals); 
    }
}
