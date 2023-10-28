using Microsoft.Extensions.Options;
using Moq;
using ProjectX.Core;
using Skender.Stock.Indicators;

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
            new MarketPrice{ Close = 1.0M, Ticker="aTest"},
            new MarketPrice{ Close = 2.0M, Ticker="aTest"},
            new MarketPrice{ Close = 3.0M, Ticker="aTest"},
        };
        IEnumerable<Quote> quotes = new Quote[]
        {
            new Quote{ Close = 1.0M },
            new Quote{ Close = 2.0M },
            new Quote{ Close = 3.0M },
        };
        Comparer<MarketPrice> marketPriceComparer = Comparer<MarketPrice>.Create(new Comparison<MarketPrice>(compareMarketPrice));
        Comparer<Quote> quoteComparer = Comparer<Quote>.Create(new Comparison<Quote>(compareQuote));       

        [SetUp]
        public void Setup()
        {            
            _marketData.Setup(x => x.GetHurst(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>())).Returns(Task.FromResult(hurst));
            _marketData.Setup(x => x.GetPrices(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>())).Returns(Task.FromResult(prices));
            _marketData.Setup(x => x.GetQuote(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>())).Returns(Task.FromResult(quotes));
            var options = Options.Create<FileBackedStoreMarketDataSourceOptions>(new FileBackedStoreMarketDataSourceOptions { Filename = "tests.json" });
            _dataSource = new FileBackedStockMarketDataSource(_marketData.Object, options);
            _dataSource.CleanFile();
        }

        [TearDown]
        public void Clean()
        {
            _dataSource.CleanFile();
            _marketData.Invocations.Clear();
        }

        [Test]
        public async Task GetHurstInvokesUnderlyingMarketDataMethodWhenKeyDoesNotExist()
        {                        
            DateTime from = DateTime.Now.AddDays(-1);
            DateTime to = DateTime.Now;
            var initialResults = (await _dataSource.GetHurst("aTicker", from, to));
            CollectionAssert.AreEqual(hurst, initialResults);
            var cachedResults = (await _dataSource.GetHurst("aTicker", from, to));
            CollectionAssert.AreEqual(hurst, cachedResults);
            CollectionAssert.AreEqual(hurst, (await _dataSource.GetHurst("aTicker", from, to)));

            _marketData.Verify(x => x.GetHurst("aTicker", from, to), Times.Once);            
        }


        [Test]
        public async Task GetPricesInvokesUnderlyingMarketDataMethodWhenKeyDoesNotExist()
        {
            DateTime from = DateTime.Now.AddDays(-1);
            DateTime to = DateTime.Now;
            var initialResults = (await _dataSource.GetPrices("aTicker", from, to));            
            CollectionAssert.AreEqual(prices, initialResults, marketPriceComparer);
            
            var cachedResults = (await _dataSource.GetPrices("aTicker", from, to));
            CollectionAssert.AreEqual(prices, cachedResults, marketPriceComparer);
            CollectionAssert.AreEqual(prices, (await _dataSource.GetPrices("aTicker", from, to)), marketPriceComparer);

            _marketData.Verify(x => x.GetPrices("aTicker", from, to), Times.Once);
        }

        static int compareMarketPrice(MarketPrice x, MarketPrice y)
        {
            if (x.Open == y.Open &&
                x.Close == y.Close &&
                x.Ticker == y.Ticker &&
                x.High == y.High &&
                x.Low == y.Low &&   
                x.Date == y.Date &&
                x.Volume == y.Volume
            )
            return 0;

            if (x.Open > y.Open ||
               x.Close > y.Close ||
               x.High > y.High ||
               x.Low > y.Low ||
               x.Date > y.Date ||
               x.Volume > y.Volume
           )
                return 1;

            return -1;
        }

        [Test]
        public async Task GetQuotesInvokesUnderlyingMarketDataMethodWhenKeyDoesNotExist()
        {
            DateTime from = DateTime.Now.AddDays(-1);
            DateTime to = DateTime.Now;
            var initialResults = (await _dataSource.GetQuote("aTicker", from, to));
            CollectionAssert.AreEqual(quotes, initialResults, quoteComparer);
            var cachedResults = (await _dataSource.GetQuote("aTicker", from, to));
            CollectionAssert.AreEqual(quotes, cachedResults, quoteComparer);
            CollectionAssert.AreEqual(quotes, (await _dataSource.GetQuote("aTicker", from, to)), quoteComparer);

            _marketData.Verify(x => x.GetQuote("aTicker", from, to), Times.Once);
        }

        private static int compareQuote(Quote x, Quote y)
        {
            if (x.Open == y.Open &&
                x.Close == y.Close &&
                x.High == y.High &&
                x.Low == y.Low &&
                x.Date == y.Date &&
                x.Volume == y.Volume
            )
                return 0;

            if (x.Open > y.Open ||
               x.Close > y.Close ||
               x.High > y.High ||
               x.Low > y.Low ||
               x.Date > y.Date ||
               x.Volume > y.Volume
           )
                return 1;

            return -1;
        }

    }
}