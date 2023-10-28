using JsonFlatFileDataStore;
using Microsoft.Extensions.Options;
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

        public FileBackedStockMarketDataSource(IStockMarketSource marketDataSource, IOptions<FileBackedStoreMarketDataSourceOptions> options)
        {
            // Open database (create new if file doesn't exist)
            _filename = options?.Value?.Filename ?? "projectx.json";
            _store = new DataStore(_filename);
            _marketDataSource = marketDataSource;
        }

        public async Task<IEnumerable<double?>> GetHurst(string ticker, DateTime from, DateTime to)
        {
            var key = Key(Category.Hurst, ticker, from, to);
            return await GetFromCacheIfExistsElseFetch<IEnumerable<double?>>(ticker, from, to, key, async () =>
            {
                return await _marketDataSource.GetHurst(ticker, from, to);
            });
        }

        public async Task<IEnumerable<MarketPrice>> GetPrices(string ticker, DateTime from, DateTime to)
        {
            var key = Key(Category.Prices, ticker, from, to);
            return await GetFromCacheIfExistsElseFetch<IEnumerable<MarketPrice>>(ticker, from, to, key, async () =>
            {
                return await _marketDataSource.GetPrices(ticker, from, to);
            });
        }

        public async Task<IEnumerable<Quote>> GetQuote(string ticker, DateTime from, DateTime to)
        {
            var key = Key(Category.Quotes, ticker, from, to);
            return await GetFromCacheIfExistsElseFetch<IEnumerable<Quote>>(ticker, from, to, key, async () =>
            {
                return await _marketDataSource.GetQuote(ticker, from, to);
            });
        }

        private async Task<T> GetFromCacheIfExistsElseFetch<T>(string ticker, DateTime from, DateTime to, string key, Func<Task<T>> fetchFromSource)
        {
            if (!_store.GetKeys().TryGetValue(key, out _))
            {                
                T fromSource = await fetchFromSource();                
                _store.InsertItem(key, fromSource);
            }
            T values = _store.GetItem<T>(key);
            return values;
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