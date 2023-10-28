using JsonFlatFileDataStore;
using ProjectX.Core;
using Skender.Stock.Indicators;

namespace ProjectX.MarketData.Cache
{
    public enum Category { Hurst, Prices, Quotes };

    public class FileBackedStockMarketDataSource : IStockMarketSource
    {        
        private readonly DataStore _store;
        private readonly string _filename;
        private readonly IStockMarketSource _marketDataSource;

        public FileBackedStockMarketDataSource(IStockMarketSource marketDataSource, string filename = "projectx.json")
        {
            // Open database (create new if file doesn't exist)
            _filename = filename;
            _store = new DataStore(filename);
            _marketDataSource = marketDataSource;
        }

        public async Task<IEnumerable<double?>> GetHurst(string ticker, DateTime from, DateTime to)
        {
            var key = Key(Category.Hurst, ticker, from, to);
            IEnumerable<double?> hurstValues = await GetFromCacheIfExistsElseFetch< IEnumerable<double?>>(ticker, from, to, key, async () =>
            {
                IEnumerable<double?> hurstsFromSource = await _marketDataSource.GetHurst(ticker, from, to);
                return hurstsFromSource;
            });
            return hurstValues;
        }

        private async Task<T> GetFromCacheIfExistsElseFetch<T>(string ticker, DateTime from, DateTime to, string key, Func<Task<T>> value)
        {
            var keys = _store.GetKeys(JsonFlatFileDataStore.ValueType.Item);
            if (!keys.TryGetValue(key, out _))
            {
                // fetch from real source
                T hurstsFromSource = await value();

                // store data into cache
                _store.InsertItem(key, hurstsFromSource);
            }
            T hurstValues = _store.GetItem<T>(key);
            return hurstValues;
        }

        public Task<IEnumerable<MarketPrice>> GetPrices(string ticker, DateTime from, DateTime to)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Quote>> GetQuote(string ticker, DateTime from, DateTime to)
        {
            throw new NotImplementedException();
        }

        static string Key(Category category, string ticker, DateTime from, DateTime to) => $"{category.ToString().ToLower()};{ticker};{from.ToString("yyyy-MM-dd")};{to.ToString("yyyy-MM-dd")}";

        public void CleanFile()
        {
            try
            {
                File.Delete(_filename);
            }
            catch (Exception ex) 
            {
                Console.WriteLine("Failed to clean up " + ex.Message);
            }
        }
    }
}