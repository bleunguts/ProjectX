using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Any;
using Newtonsoft.Json;
using ProjectX.Core;
using ProjectX.Core.Services;
using ProjectX.Core.Strategy;
using System.Reflection.Metadata.Ecma335;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ProjectX.GatewayAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class BacktestServiceController : ControllerBase
{
    private readonly ILogger<BacktestServiceController> _logger;
    private readonly IStockSignalService _stockSignalService;
    private readonly IBacktestService _backtestService;
    private readonly IStockMarketSource _stockMarketSource;

    public BacktestServiceController(ILogger<BacktestServiceController> logger, IStockSignalService stockSignalService, IBacktestService backtestService, IStockMarketSource stockMarketSource)
    {
        this._logger = logger;
        this._stockSignalService = stockSignalService;
        this._backtestService = backtestService;
        this._stockMarketSource = stockMarketSource;
    }

    [HttpGet]
    public string Get()
    {
        return "Hello from BacktestServiceController";
    }

    [HttpGet("ComputeLongShortPnl")]
    public async Task<ActionResult<StrategyResults>> ComputeLongShortPnl(string ticker, DateTime fromDate, DateTime toDate, double notional, int movingWindow, double signalIn, double signalOut, MovingAverageImpl movingAverageImpl)
    {
        var strategy = new TradingStrategy(TradingStrategyType.MeanReversion, shouldReinvest: false);

        var smoothenedSignals = await _stockSignalService.GetSignalUsingMovingAverageByDefault(ticker, fromDate, toDate, movingWindow, movingAverageImpl);
        var strategyPnls = _backtestService.ComputeLongShortPnl(smoothenedSignals, notional, signalIn, signalOut, strategy);
        var yearlyPnls = _backtestService.GetYearlyPnl(strategyPnls);
        var drawdownPnls = _backtestService.CalculateDrawdown(strategyPnls, notional);
        return Ok(new StrategyResults(strategyPnls, yearlyPnls, drawdownPnls));
    }

    [HttpGet("ComputeLongShortPnlMatrix")]
    public async Task<ActionResult<IEnumerable<MatrixStrategyPnl>>> ComputeLongShortPnlMatrix(string ticker, DateTime fromDate, DateTime toDate, double notional)
    {
        var strategy = new TradingStrategy(TradingStrategyType.MeanReversion, shouldReinvest: false);

        var marketPrices = await _stockMarketSource.GetPrices(ticker, fromDate, toDate);
        var pnls = _backtestService.ComputeLongShortPnlFull(marketPrices, notional, strategy);
        return Ok(pnls);
    }

    [HttpGet("ComputeLongShortPnlStrategyChartData")]
    public async Task<ActionResult<IEnumerable<StrategyChartData>>> ComputeLongShortPnlStrategyChartData(string ticker, DateTime fromDate, DateTime toDate, double notional, MovingAverageImpl movingAverageImpl = MovingAverageImpl.BollingerBandsImpl)
    {
        var strategy = new TradingStrategy(TradingStrategyType.MeanReversion, shouldReinvest: false);

        try
        {
            var marketPrices = await _stockMarketSource.GetPrices(ticker, fromDate, toDate);
            var pnlRanking = _backtestService.ComputeLongShortPnlFull(marketPrices, notional, strategy);
            
            var maximumProfitStrategy = pnlRanking.OrderByDescending(p => p.pnlCum).First();
            int movingWindow = maximumProfitStrategy.movingWindow;
            double signalIn = maximumProfitStrategy.zin;
            double signalOut = maximumProfitStrategy.zout;

            var smoothenedSignals = await _stockSignalService.GetSignalUsingMovingAverageByDefault(ticker, fromDate, toDate, movingWindow, movingAverageImpl);
            List<StrategyPnl> pnlForMaximumProfit = _backtestService.ComputeLongShortPnl(smoothenedSignals, notional, signalIn, signalOut, strategy).ToList();
            var data = pnlForMaximumProfit.Select(p => new StrategyChartData(p.Date.ToString("ddMMyy"), p.PnLCum, p.PnLCumHold));
            return Ok(data);
        }
        catch(Exception e) when (e.Message.Contains("TooManyRequests"))
        {
            _logger.LogError($"Error executing LongShortStrategy for {ticker}, {fromDate} {toDate} {notional}, Reason: {e.Message}");
            return NoContent();
        }
        catch (Exception e)
        {
            _logger.LogError($"CannotError executing LongShortStrategy for {ticker}, {fromDate} {toDate} {notional}, Reason: {e.Message}");
            return NotFound();
        }
    }
}
