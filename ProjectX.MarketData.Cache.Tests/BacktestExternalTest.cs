using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using ProjectX.Core;
using ProjectX.Core.Services;
using ProjectX.Core.Strategy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectX.MarketData.Cache.Tests
{
    public class BacktestExternalTest
    {
        const string ticker = "IBM";

        private static readonly EnvironmentVariableLoader _environmentVariableLoader = new();
        private static readonly string _apiKey = _environmentVariableLoader.FromEnvironmentVariable("fmpapikey");
        private readonly BacktestService _backtestService = new();
        private readonly FileBackedStockMarketDataSource _marketSource = new FileBackedStockMarketDataSource(
            new FMPStockMarketSource(Options.Create<FMPStockMarketSourceOptions>(new FMPStockMarketSourceOptions { ApiKey = _apiKey })), 
            Options.Create<FileBackedStoreMarketDataSourceOptions>(new FileBackedStoreMarketDataSourceOptions { Filename = "externalTests.json" }));
        public record ChartData(string time, double amount, double amountHold);

        [Test]
        [Ignore("Once off tool to generate chart data for webui, dont use this to get lots of data")]
        public async Task foo()
        {            
            var smoothenedSignals = await new StockSignalService(_marketSource).GetSignalUsingMovingAverageByDefault(ticker, new DateTime(2023, 5, 1), new DateTime(2023, 9, 25), 3, MovingAverageImpl.MyImpl);
            List<StrategyPnl> pnls = _backtestService.ComputeLongShortPnl(smoothenedSignals, 10_000, 0.8, 0.2, new TradingStrategy(TradingStrategyType.MeanReversion, false)).ToList();
            List<ChartData> result = new();
            foreach (StrategyPnl pn in pnls)
            {
                result.Add(new ChartData(pn.Date.ToString("yyMMdd"), pn.PnLCum, pn.PnLCumHold));
            }
            var json = JsonConvert.SerializeObject(result);
            Console.WriteLine(json);
            File.WriteAllText($"c:\\temp\\chart.json", json);
        }
    }
}
