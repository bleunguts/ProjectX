using Microsoft.Extensions.Options;
using ProjectX.Core;
using ProjectX.Core.Services;
using ProjectX.Core.Strategy;
using ProjectX.MarketData.Cache;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectX.MarketData.Tests
{
    public  class StockSignalServiceExternalTest
    {
        private static readonly EnvironmentVariableLoader _environmentVariableLoader = new();
        private static readonly FileBackedStockMarketDataSource _marketSource = new FileBackedStockMarketDataSource(new FMPStockMarketSource(Options.Create<FMPStockMarketSourceOptions>(new FMPStockMarketSourceOptions { ApiKey = _environmentVariableLoader.FromEnvironmentVariable("fmpapikey")})), Options.Create<FileBackedStoreMarketDataSourceOptions>(new FileBackedStoreMarketDataSourceOptions { Filename = "externalTests.json" }));
        private readonly StockSignalService _signalService = new StockSignalService(_marketSource);        

        [Test]
        [Ignore("Tool to check result differences")]
        public async Task CompareMovingAverageImplementationFromBookVsFromStockIndicatorsLib()
        {
            var myImpl = await _signalService.GetSignalUsingMovingAverageByDefault("ACAQ", new DateTime(2023, 9, 1), new DateTime(2023, 10, 1), 5, MovingAverageImpl.MyImpl);
            Print(myImpl);

            Console.WriteLine("=======================================================================");
            var libImpl = await _signalService.GetSignalUsingMovingAverageByDefault("ACAQ", new DateTime(2023, 9, 1), new DateTime(2023, 10, 1), 5, MovingAverageImpl.BollingerBandsImpl);
            Print(libImpl);
        }

        private void Print(IEnumerable<PriceSignal> signals)
        {            
            foreach(var signal in signals.ToList().OrderBy(t => t.Date)) 
            { 
                Console.WriteLine(signal.ToString());
            }
        }
    }
}
