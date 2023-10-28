using Moq;
using ProjectX.Core;

namespace ProjectX.MarketData.Cache.Tests
{
    public class FileBackedStockMarketDataSourceTest
    {
        private Mock<IStockMarketSource> _marketData = new Mock<IStockMarketSource>();
        private FileBackedStockMarketDataSource _dataSource;
        private IEnumerable<double?> hurst = new List<double?>
        {
            1.0,
            2.0,
            3.0
        };
        private IEnumerable<MarketPrice> prices = new List<MarketPrice>
        {
            new MarketPrice{ Close = 1.0M },
            new MarketPrice{ Close = 2.0M },
            new MarketPrice{ Close = 3.0M },
        };

        [SetUp]
        public void Setup()
        {            
            _marketData.Setup(x => x.GetHurst(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>())).Returns(Task.FromResult(hurst));
            _marketData.Setup(x => x.GetPrices(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>())).Returns(Task.FromResult(prices));
            _dataSource = new FileBackedStockMarketDataSource(_marketData.Object, "tests.json");
            _dataSource.CleanFile();
        }

        [TearDown]
        public void Clean()
        {
            _dataSource.CleanFile();
        }

        [Test]
        public async Task GetHurstInvokesUnderlyingMarketDataMethodWhenKeyDoesNotExist()
        {                        
            DateTime from = DateTime.Now.AddDays(-1);
            DateTime to = DateTime.Now;
            var initialResults = (await _dataSource.GetHurst("aTicker", from, to)).ToList();
            CollectionAssert.AreEqual(hurst, initialResults);
            var cachedResults = (await _dataSource.GetHurst("aTicker", from, to)).ToList();
            CollectionAssert.AreEqual(hurst, cachedResults);

            _marketData.Verify(x => x.GetHurst("aTicker", from, to), Times.Once);            
        }

        [Test]
        public async Task GetPricesInvokesUnderlyingMarketDataMethodWhenKeyDoesNotExist()
        {
            DateTime from = DateTime.Now.AddDays(-1);
            DateTime to = DateTime.Now;
            var initialResults = (await _dataSource.GetPrices("aTicker", from, to)).ToList();
            CollectionAssert.AreEqual(prices, initialResults);
            var cachedResults = (await _dataSource.GetPrices("aTicker", from, to)).ToList();
            CollectionAssert.AreEqual(prices, cachedResults);

            _marketData.Verify(x => x.GetPrices("aTicker", from, to), Times.Once);
        }

    }
}