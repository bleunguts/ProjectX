using ProjectX.Core.Strategy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectX.Core.Services
{
    public class StockSignalService
    {
        private readonly string _ticker;
        private readonly IStockMarketSource _marketSource;

        public StockSignalService(string ticker, IStockMarketSource marketSource)
        {
            this._ticker = ticker;
            this._marketSource = marketSource;
        }

        /// <summary>
        /// As a general rule of thumb, you will be safe if you provide 750 points of historical quote data (e.g. 3 years of daily data).
        /// </summary>
        internal async Task<IEnumerable<MarketPrice>> FetchMarketPricesAsync(DateTime startDate, DateTime endDate)
        {            
            var prices = await _marketSource.GetPrices(_ticker, startDate, endDate);
            return prices;
        }        
        public async Task<IEnumerable<SignalEntity>> GetSignalUsingMovingAverageByDefault(DateTime startDate, DateTime endDate, int movingWindow)
        {     
            IEnumerable<MarketPrice> marketPrices = await FetchMarketPricesAsync(startDate, endDate);

            // TODO: parse marketPrices into Moving Average library that results in signals             

            return new List<SignalEntity>();
        }
    }
}
