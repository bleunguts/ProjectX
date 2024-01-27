using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Any;
using Newtonsoft.Json;
using ProjectX.Core.Services;
using ProjectX.Core.Strategy;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ProjectX.GatewayAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class BacktestServiceController
{
    private readonly ILogger<BacktestServiceController> _logger;
    private readonly IStockSignalService _stockSignalService;
    private readonly IBacktestService _backtestService;

    public BacktestServiceController(ILogger<BacktestServiceController> logger, IStockSignalService stockSignalService, IBacktestService backtestService)
    {
        this._logger = logger;
        this._stockSignalService = stockSignalService;
        this._backtestService = backtestService;
    }

    [HttpGet]
    public string Get()
    {
        return "Hello from BacktestServiceController";
    }

    [HttpGet("LongShortStrategy")]
    public async Task<IEnumerable<StrategyChartData>> Get(string ticker, DateTime fromDate, DateTime toDate, double notional)
    {
        int movingWindow = 10;
        double signalIn = 0.5;
        double signalOut = 0.4;

        bool isReinvest = false;
        MovingAverageImpl movingAverageImpl = MovingAverageImpl.BollingerBandsImpl;

        var smoothenedSignals = await _stockSignalService.GetSignalUsingMovingAverageByDefault(ticker, fromDate, toDate, movingWindow, movingAverageImpl);
        List<StrategyPnl> pnls = _backtestService.ComputeLongShortPnl(smoothenedSignals, notional, signalIn, signalOut, new TradingStrategy(TradingStrategyType.MeanReversion, isReinvest)).ToList();
        var data = pnls.Select(p => new StrategyChartData(p.Date.ToString("ddMMyy"), p.PnLCum, p.PnLCumHold));
        return data;
    }
}
