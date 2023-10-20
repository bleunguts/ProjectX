using ProjectX.Core.Strategy;
using Skender.Stock.Indicators;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectX.Core.Services
{
    public interface IStockSignalService
    {
        Task<List<PriceSignal>> GetSignalUsingMovingAverageByDefault(string ticker, DateTime startDate, DateTime endDate, int movingWindow);
    }

    [Export(typeof(IStockSignalService)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class StockSignalService : IStockSignalService
    {
        private readonly IStockMarketSource _marketSource;

        [ImportingConstructor]
        public StockSignalService(IStockMarketSource marketSource)
        {
            this._marketSource = marketSource;
        }
               
        /// <summary>
        /// As a general rule of thumb, you will be safe if you provide 750 points of historical quote data (e.g. 3 years of daily data).
        /// </summary>
        public async Task<List<PriceSignal>> GetSignalUsingMovingAverageByDefault(string ticker, DateTime startDate, DateTime endDate, int movingWindow)
        {
            var marketPrices = await _marketSource.GetPrices(ticker, startDate, endDate);            

            return marketPrices.MovingAverage(movingWindow).ToList();           
        }
    }
}
