﻿using Castle.Components.DictionaryAdapter.Xml;
using Moq;
using ProjectX.Core.Services;
using ProjectX.Core.Strategy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectX.Core.Tests.Services
{
    public class StockSignalServiceTest
    {
        private readonly string Ticker = "IBM";
        private readonly DateTime _startDate = new DateTime(2023, 12, 1);
        private readonly DateTime _endDate = new DateTime(2023, 12, 31);
        private readonly int _movingWindow = 3;        
        private Mock<IStockMarketSource> _marketSource;
        private MarketPrice[] _marketPrices = new[]
        {
            new MarketPrice{ Date = new DateTime(2023, 12, 1), Close = 123, High=124, Low=122, Open=123, Volume=10000},
            new MarketPrice{ Date = new DateTime(2023, 12, 2), Close = 245, High=246, Low=244, Open=245, Volume=10000},
            new MarketPrice{ Date = new DateTime(2023, 12, 3), Close = 567, High=568, Low=566, Open=567, Volume=10000},
            new MarketPrice{ Date = new DateTime(2023, 12, 4), Close = 567, High=568, Low=566, Open=567, Volume=10000},
            new MarketPrice{ Date = new DateTime(2023, 12, 5), Close = 245, High=246, Low=244, Open=245, Volume=10000},
            new MarketPrice{ Date = new DateTime(2023, 12, 6), Close = 567, High=568, Low=566, Open=567, Volume=10000},
            new MarketPrice{ Date = new DateTime(2023, 12, 7), Close = 123, High=124, Low=122, Open=123, Volume=10000},
            new MarketPrice{ Date = new DateTime(2023, 12, 8), Close = 567, High=568, Low=566, Open=567, Volume=10000},
            new MarketPrice{ Date = new DateTime(2023, 12, 9), Close = 123, High=124, Low=122, Open=123, Volume=10000},
            new MarketPrice{ Date = new DateTime(2023, 12, 10), Close = 567, High=568, Low=566, Open=567, Volume=10000},
            new MarketPrice{ Date = new DateTime(2023, 12, 11), Close = 567, High=568, Low=566, Open=567, Volume=10000},
            new MarketPrice{ Date = new DateTime(2023, 12, 12), Close = 567, High=568, Low=566, Open=567, Volume=10000},
            new MarketPrice{ Date = new DateTime(2023, 12, 13), Close = 567, High=568, Low=566, Open=567, Volume=10000},
            new MarketPrice{ Date = new DateTime(2023, 12, 14), Close = 567, High=568, Low=566, Open=567, Volume=10000},
            new MarketPrice{ Date = new DateTime(2023, 12, 15), Close = 567, High=568, Low=566, Open=567, Volume=10000},
            new MarketPrice{ Date = new DateTime(2023, 12, 16), Close = 567, High=568, Low=566, Open=567, Volume=10000},
            new MarketPrice{ Date = new DateTime(2023, 12, 17), Close = 567, High=568, Low=566, Open=567, Volume=10000},
            new MarketPrice{ Date = new DateTime(2023, 12, 18), Close = 567, High=568, Low=566, Open=567, Volume=10000},
            new MarketPrice{ Date = new DateTime(2023, 12, 19), Close = 567, High=568, Low=566, Open=567, Volume=10000},
            new MarketPrice{ Date = new DateTime(2023, 12, 20), Close = 567, High=568, Low=566, Open=567, Volume=10000},
            new MarketPrice{ Date = new DateTime(2023, 12, 21), Close = 567, High=568, Low=566, Open=567, Volume=10000},
            new MarketPrice{ Date = new DateTime(2023, 12, 22), Close = 567, High=568, Low=566, Open=567, Volume=10000},
            new MarketPrice{ Date = new DateTime(2023, 12, 23), Close = 567, High=568, Low=566, Open=567, Volume=10000},
            new MarketPrice{ Date = new DateTime(2023, 12, 24), Close = 567, High=568, Low=566, Open=567, Volume=10000},
            new MarketPrice{ Date = new DateTime(2023, 12, 25), Close = 567, High=568, Low=566, Open=567, Volume=10000},
            new MarketPrice{ Date = new DateTime(2023, 12, 26), Close = 567, High=568, Low=566, Open=567, Volume=10000},
            new MarketPrice{ Date = new DateTime(2023, 12, 27), Close = 567, High=568, Low=566, Open=567, Volume=10000},
            new MarketPrice{ Date = new DateTime(2023, 12, 28), Close = 567, High=568, Low=566, Open=567, Volume=10000},
            new MarketPrice{ Date = new DateTime(2023, 12, 29), Close = 567, High=568, Low=566, Open=567, Volume=10000},
            new MarketPrice{ Date = new DateTime(2023, 12, 30), Close = 567, High=568, Low=566, Open=567, Volume=10000},
            new MarketPrice{ Date = new DateTime(2023, 12, 31), Close = 567, High=568, Low=566, Open=567, Volume=10000},
        }; 

        [SetUp]
        public void SetUp()
        {
            _marketSource = new Mock<IStockMarketSource>();
            _marketSource.Setup(_ => _.GetPrices(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                        .ReturnsAsync(_marketPrices);
        }
       
        [Test]
        public async Task WhenGettingStockSignalsShouldReturnMovingAveragedSmoothingOverRawValues()
        {
            var service = new StockSignalService(Ticker, _marketSource.Object);
            var actual = await service.GetSignalUsingMovingAverageByDefault(_startDate, _endDate, _movingWindow);            
            foreach(var p in actual)
            {
                Console.Out.WriteLine($"{p.Ticker}: {p.Date} price={p.Price} predicted={p.PricePredicted} upper={p.UpperBand} lower={p.LowerBand} signal={p.Signal}");
            }
            Assert.That(actual, Is.Not.Empty);
            Assert.That(actual, Has.Count.EqualTo(30));
            _marketSource.Verify(x => x.GetPrices(Ticker, _startDate, _endDate), Times.Once);

            //var data = await SignalHelper.GetStockData(Ticker, StartDate, EndDate, SelectedPriceType);
            //var signal = SignalHelper.GetSignal(data, MovingWindow, SelectedSignalType);

            // signal is used as input to compute long short pnl model
            //var pnls = BacktestHelper.ComputeLongShortPnl(signal, 10_000.0, signalIn, signalOut, SelectedStrategyType, IsReinvest).ToList();
        }
    }
}
