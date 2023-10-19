using Castle.Components.DictionaryAdapter.Xml;
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
    public class SignalServiceTest
    {
        private readonly string Ticker = "IBM";
        private readonly DateTime _startDate = new DateTime(2023, 10, 1);
        private readonly DateTime _endDate = new DateTime(2023, 10, 14);
        private readonly int _movingWindow = 5;        
        private Mock<IStockMarketSource> _marketSource;
        private MarketPrice[] _marketPrices = new[]
        {
            new MarketPrice{ Close = 123 },
            new MarketPrice{ Close = 245 },
            new MarketPrice{ Close = 567 },
        }; 

        [SetUp]
        public void SetUp()
        {
            _marketSource = new Mock<IStockMarketSource>();
            _marketSource.Setup(_ => _.GetPrices(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                        .ReturnsAsync(_marketPrices);
        }
       
        [Test]
        public async Task GettingSignalWorksAsync()
        {
            var service = new StockSignalService(Ticker, _marketSource.Object);
            var actual = await service.GetSignalUsingMovingAverageByDefault(_startDate, _endDate, _movingWindow);
            Assert.That(actual, Is.Not.Empty, "TODO");

            _marketSource.Verify(x => x.GetPrices(Ticker, _startDate, _endDate), Times.Once);

            //var data = await SignalHelper.GetStockData(Ticker, StartDate, EndDate, SelectedPriceType);
            //var signal = SignalHelper.GetSignal(data, MovingWindow, SelectedSignalType);

            // signal is used as input to compute long short pnl model
            //var pnls = BacktestHelper.ComputeLongShortPnl(signal, 10_000.0, signalIn, signalOut, SelectedStrategyType, IsReinvest).ToList();
        }
    }
}
