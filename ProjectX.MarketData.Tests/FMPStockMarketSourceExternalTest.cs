using MatthiWare.FinancialModelingPrep;
using MatthiWare.FinancialModelingPrep.Model;
using MatthiWare.FinancialModelingPrep.Model.CompanyValuation;
using MatthiWare.FinancialModelingPrep.Model.StockTimeSeries;
using NuGet.Frameworks;
using ProjectX.Core;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ProjectX.MarketData.Tests
{
    public class FMPStockMarketSourceExternalTest
    {      
        [Test]
        public async Task ExternalFoo()
        {
            var api = FinancialModelingPrepApiClientFactory.CreateClient(new FinancialModelingPrepOptions()
            {
                ApiKey = "35fdfe7c1a0d49e6ca2283bb073fea3a"
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
        public async Task WhenCallingGetHistoricalPricesShouldReturnValidPricesAsync()
        {
            var marketDataService = new FMPStockMarketSource();
            var marketPrices = await marketDataService.GetPrices("ACAQ", new DateTime(2023, 1, 1), new DateTime(2023, 10, 1));
            Assert.That(marketPrices.Count(), Is.GreaterThan(0));
            foreach (var data in marketPrices)
            {
                Console.WriteLine($"Date: {data.Date}, Close:{data.Close}, Volume:{data.Volume}");
                //Assert.That(data.Date.ToDateTime().IsBetween(new DateTime(2020, 1, 1), new DateTime(2020, 2, 1)), Is.True);
                Assert.That(data.Close, Is.GreaterThan(0));
            }
        }
    }
   
}