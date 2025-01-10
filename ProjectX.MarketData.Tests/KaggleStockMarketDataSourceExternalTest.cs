using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectX.MarketData.Tests;

public class KaggleStockMarketDataSourceExternalTest
{
    [Test]
    public async Task GetPricesShouldOnlyReturnPricesWithinDateRange()
    {
        var marketDataSource = new KaggleFileBasedStockMarketSource();
        var startDate = new DateTime(2015, 5, 27);
        var nextDay = new DateTime(2015, 5, 28);
        var marketPrices = (await marketDataSource.GetPrices("AAPL", startDate, nextDay)).ToList();

        CollectionAssert.AreEquivalent(new[] { startDate, nextDay }, marketPrices.Select(p => p.Date));
    }

    [Test]
    public async Task Tool()
    {
        var marketData = new KaggleFileBasedStockMarketSource();
        var someData = await marketData.GetPrices("AAPL", new DateTime(2020, 1, 1), new DateTime(2020, 1, 15));

        foreach (var price in someData)
        {
            Console.WriteLine($"{price.Date} {price.Ticker} {price.Close}");
        }       
    }

}
