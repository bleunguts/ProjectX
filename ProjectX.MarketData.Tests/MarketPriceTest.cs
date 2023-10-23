using MatthiWare.FinancialModelingPrep.Model.StockTimeSeries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectX.MarketData.Tests
{
    public class MarketPriceTest
    {
        [Test]
        public void CanConvertHistoricalPriceItemToInternalDomainObject()
        {
            const int someValue = 123;
            var item = new HistoricalPriceItem()
            {                
                Close = someValue,
                Open = someValue,
                High = someValue,
                Low = someValue,
                Volume= 10_000,
                Date = "2023-09-29",
                Label = "IBM"
            };
                
            var actualPrice = item.ToMarketPrice("IBM");
            Assert.That(actualPrice.Close, Is.EqualTo(someValue));
            Assert.That(actualPrice.Open, Is.EqualTo(someValue));
            Assert.That(actualPrice.High, Is.EqualTo(someValue));
            Assert.That(actualPrice.Low, Is.EqualTo(someValue));
            Assert.That(actualPrice.Volume, Is.EqualTo(10_000));
            Assert.That(actualPrice.Date, Is.EqualTo(new DateTime(2023, 9, 29)));
            Assert.That(actualPrice.Ticker, Is.EqualTo("IBM"));
        }            
    }
}
