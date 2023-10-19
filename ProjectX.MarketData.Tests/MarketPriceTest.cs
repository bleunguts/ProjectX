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
                Volume= 10_000
            };
                
            var actualPrice = item.ToMarketPrice();
            Assert.That(actualPrice.Close, Is.EqualTo(someValue));
            Assert.That(actualPrice.Open, Is.EqualTo(someValue));
            Assert.That(actualPrice.High, Is.EqualTo(someValue));
            Assert.That(actualPrice.Low, Is.EqualTo(someValue));
            Assert.That(actualPrice.Volume, Is.EqualTo(10_000));            
        }            
    }
}
