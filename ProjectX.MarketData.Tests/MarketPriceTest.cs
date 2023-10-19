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
            var item = new HistoricalPriceItem()
            {
                Close = 123
            };
                
            var actualPrice = item.ToMarketPrice();
            Assert.That(actualPrice.Close, Is.EqualTo(123));
        }            
    }
}
