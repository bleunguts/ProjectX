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
        private readonly DateTime StartDate = new DateTime(2023, 10, 1);
        private readonly DateTime EndDate = new DateTime(2023, 10, 14);
        private readonly object PriceType;
        private Mock<IStockMarketSource> _marketSource;

        [SetUp]
        public void SetUp()
        {
            _marketSource = new Mock<IStockMarketSource>();
        }

        [Test]
        public void GettingStockDataWorks()
        {

            var service = new StockSignalService(Ticker, _marketSource.Object);
            Assert.Fail("Not implemented");
            //var data = await SignalHelper.GetStockData(Ticker, StartDate, EndDate, SelectedPriceType);
            //var signal = SignalHelper.GetSignal(data, MovingWindow, SelectedSignalType);

            // signal is used as input to compute long short pnl model
            //var pnls = BacktestHelper.ComputeLongShortPnl(signal, 10_000.0, signalIn, signalOut, SelectedStrategyType, IsReinvest).ToList();
        }

        [Test]
        public void GettingSignalWorks()
        {
            var service = new StockSignalService(Ticker, _marketSource.Object);
            Assert.That(service.GetSignal(null, 5, "sdo"), Is.Empty);
        }      
    }
}
