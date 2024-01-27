using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Any;
using Newtonsoft.Json;
using ProjectX.Core;
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

    [HttpGet("LongShortStrategy")]
    public async Task<IEnumerable<StrategyChartData>> Get(string ticker, DateTime fromDate, DateTime toDate, double notional)
    { 
        bool isReinvest = false;
        MovingAverageImpl movingAverageImpl = MovingAverageImpl.BollingerBandsImpl;

        try
        {
            var marketPrices = await _stockMarketSource.GetPrices(ticker, fromDate, toDate);
            var pnlRanking = _backtestService.ComputeLongShortPnlFull(marketPrices, notional, new TradingStrategy(TradingStrategyType.MeanReversion, isReinvest));
            var maximumProfitStrategy = pnlRanking.OrderByDescending(p => p.pnlCum).First();

            int movingWindow = maximumProfitStrategy.movingWindow;
            double signalIn = maximumProfitStrategy.zin;
            double signalOut = maximumProfitStrategy.zout;

            var smoothenedSignals = await _stockSignalService.GetSignalUsingMovingAverageByDefault(ticker, fromDate, toDate, movingWindow, movingAverageImpl);
            List<StrategyPnl> pnlForMaximumProfit = _backtestService.ComputeLongShortPnl(smoothenedSignals, notional, signalIn, signalOut, new TradingStrategy(TradingStrategyType.MeanReversion, isReinvest)).ToList();
            var data = pnlForMaximumProfit.Select(p => new StrategyChartData(p.Date.ToString("ddMMyy"), p.PnLCum, p.PnLCumHold));
            return data;
        }
        catch(Exception exp)
        {
            _logger.LogError($"Cannot GetLongShort Strategy for {ticker}, {fromDate} {toDate} {notional}, Reason: {exp.Message}");
            throw;
        }
    }
}
