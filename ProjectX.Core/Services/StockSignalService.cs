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
        internal async Task<IEnumerable<MarketPrice>> FetchDataAsync(DateTime startDate, DateTime endDate)
        {
            // stores signal data locally 
            // calls a marketData facade to invoke getting the data
            var prices = await _marketSource.GetPrices(_ticker, startDate, endDate);
            return prices;
        }
        public IEnumerable<SignalEntity> GetSignal(IEnumerable<SignalEntity> signals, int movingWindow, string signalType)
        {
            // only implement for moving average
            // use moving average library
            throw new NotImplementedException();
        }
    }
}
