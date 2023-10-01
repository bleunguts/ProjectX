using Microsoft.Extensions.Logging.Abstractions;
using ProjectX.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ProjectX.Core.Services.eFXTrader;

namespace ProjectX.Core.Tests
{
    public class eFXTraderTest
    {
        private const string _clientName = "Joe";
        private IFXTrader _sut = new eFXTrader(new NullLogger<eFXTrader>());

        [Test]
        public void WhenFetchingPositionsFromTradeManagerShouldReturnCorrectPositionsPerCurrencyPairTraded()
        {           
            // act
            _sut.ExecuteTrade(TradeRequestFor(BuySell.Buy, "EURUSD", 100, 1.5M, 1.2M));            
            Dictionary<string, int> positions = _sut.PositionsFor(_clientName);

            // assert
            Assert.That(positions.ContainsKey("EURUSD"), Is.True);
            Assert.That(positions["EURUSD"], Is.EqualTo(100));
        }

        private static TradeRequest TradeRequestFor(BuySell buySell, string currencyPair, int quantity, decimal bidPrice, decimal askPrice) => new TradeRequest(FXProductType.Spot, new SpotPrice(currencyPair, bidPrice, askPrice), quantity, buySell, _clientName, new DateTimeOffset(DateTime.Now));
    }
}
