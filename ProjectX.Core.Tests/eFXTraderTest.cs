﻿using Microsoft.Extensions.Logging.Abstractions;
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
          
            // assert            
            Assert.That(_sut.PositionsFor(_clientName).TryGetValue("EURUSD", out (int netQuantity, int totalTrades, string debug) v1), Is.True);            
            Assert.That(v1.netQuantity, Is.EqualTo(100));
            Assert.That(v1.totalTrades, Is.EqualTo(1));            

            _sut.ExecuteTrade(TradeRequestFor(BuySell.Buy, "EURUSD", 200, 2.5M, 0.2M));
            Assert.That(_sut.PositionsFor(_clientName).TryGetValue("EURUSD", out (int netQuantity, int totalTrades, string debug) v2), Is.True);
            Assert.That(v2.netQuantity, Is.EqualTo(300));
            Assert.That(v2.totalTrades, Is.EqualTo(2));

            _sut.ExecuteTrade(TradeRequestFor(BuySell.Sell, "EURUSD", 125, 9.5M, 5.2M));
            Assert.That(_sut.PositionsFor(_clientName).TryGetValue("EURUSD", out (int netQuantity, int totalTrades, string debug) v3), Is.True);
            Assert.That(v3.netQuantity, Is.EqualTo(175));
            Assert.That(v3.totalTrades, Is.EqualTo(3));            
        }

        private static TradeRequest TradeRequestFor(BuySell buySell, string currencyPair, int quantity, decimal bidPrice, decimal askPrice) => new TradeRequest(FXProductType.Spot, new SpotPrice(currencyPair, bidPrice, askPrice), quantity, buySell, _clientName, new DateTimeOffset(DateTime.Now));
    }
}
