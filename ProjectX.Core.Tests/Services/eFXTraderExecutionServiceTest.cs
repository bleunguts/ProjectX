using Microsoft.Extensions.Logging.Abstractions;
using ProjectX.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ProjectX.Core.Services.eFXTradeExecutionService;

namespace ProjectX.Core.Tests.Services
{
    public class eFXTraderExecutionServiceTest
    {
        private const string _clientName = "Joe";
        private IFXTradeExecutionService _sut = new eFXTradeExecutionService(new NullLogger<eFXTradeExecutionService>());

        [Test]
        public void WhenFetchingPositionsFromTradeManagerShouldReturnValidTradeCountFieldsAndCalculatedFields()
        {
            // act
            _sut.ExecuteTrade(TradeRequestFor(BuySell.Buy, "EURUSD", 100, 1.5M, 1.2M));

            // assert            
            Assert.That(_sut.PositionsFor(_clientName).TryGetValue("EURUSD", out (int netQuantity, int totalTrades, decimal pnl, string debug) v1), Is.True);
            Assert.That(v1.netQuantity, Is.EqualTo(100));
            Assert.That(v1.totalTrades, Is.EqualTo(1));
            Assert.That(v1.pnl, Is.EqualTo(120m));

            _sut.ExecuteTrade(TradeRequestFor(BuySell.Buy, "EURUSD", 200, 2.5M, 0.2M));
            Assert.That(_sut.PositionsFor(_clientName).TryGetValue("EURUSD", out (int netQuantity, int totalTrades, decimal pnl, string debug) v2), Is.True);
            Assert.That(v2.netQuantity, Is.EqualTo(300));
            Assert.That(v2.pnl, Is.EqualTo(160m));

            _sut.ExecuteTrade(TradeRequestFor(BuySell.Sell, "EURUSD", 125, 9.5M, 5.2M));
            Assert.That(_sut.PositionsFor(_clientName).TryGetValue("EURUSD", out (int netQuantity, int totalTrades, decimal pnl, string debug) v3), Is.True);
            Assert.That(v3.netQuantity, Is.EqualTo(175));
            Assert.That(v3.pnl, Is.EqualTo(-1027.5m));
        }

        private static TradeRequest TradeRequestFor(BuySell buySell, string currencyPair, int quantity, decimal bidPrice, decimal askPrice) => new TradeRequest(FXProductType.Spot, new SpotPrice(currencyPair, bidPrice, askPrice), quantity, buySell, _clientName, new DateTimeOffset(DateTime.Now));
    }
}
