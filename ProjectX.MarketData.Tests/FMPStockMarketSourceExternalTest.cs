using MatthiWare.FinancialModelingPrep;
using MatthiWare.FinancialModelingPrep.Model;
using MatthiWare.FinancialModelingPrep.Model.CompanyValuation;
using MatthiWare.FinancialModelingPrep.Model.StockTimeSeries;
using Microsoft.Extensions.Options;
using NuGet.Frameworks;
using ProjectX.Core;
using ProjectX.Core.Strategy;
using Skender.Stock.Indicators;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ProjectX.MarketData.Tests
{
    public class FMPStockMarketSourceExternalTest
    {
        private static readonly EnvironmentVariableLoader _environmentVariableLoader = new();
        private static readonly string _apiKey = _environmentVariableLoader.FromEnvironmentVariable("fmpapikey");

        [Test]
        [Ignore("Tool")]
        public async Task ExternalFoo()
        {
            var api = FinancialModelingPrepApiClientFactory.CreateClient(new FinancialModelingPrepOptions()
            {
                ApiKey = _apiKey
            });            

            var result = await api.StockTimeSeries.GetHistoricalDailyPricesAsync("GOOGL", "2020-01-01", "2020-02-01");
            Console.WriteLine($"Getting price for stock, Result: {result.Data.ToString()}");            
            Assert.That(result.Data.Historical, Has.Count.GreaterThan(1));
                       
            foreach(var data in result.Data.Historical)
            {
                Console.WriteLine($"Date: {data.Date.ToDateTime()}, Symbol: {data.Date}, Label:{data.Label}, Close:{data.Close}, Volume:{data.Volume}");
                Assert.That(data.Date.ToDateTime().IsBetween(new DateTime(2020, 1, 1), new DateTime(2020, 2, 1)), Is.True);
                Assert.That(data.Close, Is.GreaterThan(0));
            }
        }

        [Test]
        [Ignore("Tool")]
        public async Task WhenGettingHistoricalPricesFromFMP()
        {
            var marketDataService = new FMPStockMarketSource(Options.Create<FMPStockMarketSourceOptions>(new FMPStockMarketSourceOptions { ApiKey = _apiKey }));
            var marketPrices = await marketDataService.GetPrices("ACAQ", new DateTime(2023, 1, 1), new DateTime(2023, 10, 1));
            Assert.That(marketPrices.Count(), Is.GreaterThan(0));
            foreach (var data in marketPrices)
            {
                Console.WriteLine($"Date: {data.Date}, Close:{data.Close}, Volume:{data.Volume}");
                //Assert.That(data.Date.ToDateTime().IsBetween(new DateTime(2020, 1, 1), new DateTime(2020, 2, 1)), Is.True);
                Assert.That(data.Close, Is.GreaterThan(0));
            }
        }

        [Test]
        [Ignore("External Tool for testing only")]
        public async Task WhenGettingStockMarketMostActiveFromFMP() 
        {
            var marketDataService = new FMPStockMarketSource(Options.Create<FMPStockMarketSourceOptions>(new FMPStockMarketSourceOptions { ApiKey = _apiKey }));
            var mostActiveStocks = await marketDataService.GetMostActiveStocks();
            Assert.That(mostActiveStocks.Count(), Is.GreaterThan(0));
            foreach(var data in mostActiveStocks) 
            {
                Console.WriteLine($"{data.CompanyName} {data.Ticker} {data.Price} {data.Changes} {data.ChangesPercentage}");
            }
        }

        [Test]
        [Ignore("Use to try out StockIndicator Api features")]
        public async Task WhenGettingBollingerBandsFromFMQ()
        {
            var config = Options.Create<FMPStockMarketSourceOptions>(new FMPStockMarketSourceOptions { ApiKey = _apiKey });
            var marketDataService = new FMPStockMarketSource(config);
            var quotes = await marketDataService.GetQuote("ACAQ", new DateTime(2023, 1, 1), new DateTime(2023, 10, 1));
            Assert.That(quotes.Count(), Is.GreaterThan(0));

            var results = quotes.GetBollingerBands(5);
            foreach(var result in results)
            {
                var signal = new PriceSignal() 
                { 
                    Ticker = "ACAQ", 
                    Date = result.Date, 
                    Price = Convert.ToDecimal(result.Sma), 
                    PricePredicted = Convert.ToDecimal(result.Sma),
                    Signal = Convert.ToDecimal(result.ZScore), 
                    UpperBand = Convert.ToDecimal(result.UpperBand), 
                    LowerBand = Convert.ToDecimal(result.LowerBand) 
                };
                Console.WriteLine(signal);
            }
        }
    }
   
}