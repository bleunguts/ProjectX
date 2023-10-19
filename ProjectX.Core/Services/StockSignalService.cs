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
        Task<List<PriceSignalEntity>> GetSignalUsingMovingAverageByDefault(string ticker, DateTime startDate, DateTime endDate, int movingWindow);
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
        internal async Task<List<MarketPrice>> FetchMarketPricesAsync(string ticker, DateTime startDate, DateTime endDate)
        {
            var prices = await _marketSource.GetPrices(ticker, startDate, endDate);
            return prices.ToList();
        }
        public async Task<List<PriceSignalEntity>> GetSignalUsingMovingAverageByDefault(string ticker, DateTime startDate, DateTime endDate, int movingWindow)
        {
            List<MarketPrice> marketPrices = await FetchMarketPricesAsync(ticker, startDate, endDate);

            var processed = marketPrices
                            .GetMaEnvelopes(movingWindow)
                            .ToList();

            List<PriceSignalEntity> signalsProcessed = new();
            for (int i = 0; i < processed.Count; i++)
            {
                var signal = processed[i];
                if (signal == null)
                {
                    throw new ArgumentNullException(nameof(signal), "Signal should not be null");
                }
                if (signal.Centerline == null) continue;
                if (signal.LowerEnvelope == null) continue;
                if (signal.UpperEnvelope == null) continue;                

                signalsProcessed.Add(new PriceSignalEntity
                {
                    Ticker = ticker,
                    Date = signal.Date,
                    Price = marketPrices[i].Close,
                    PricePredicted = (decimal)signal.Centerline,
                    LowerBand = (decimal)signal.LowerEnvelope,
                    UpperBand = (decimal)signal.UpperEnvelope,
                    Signal = (decimal)signal.Centerline,
                });
            }
            return signalsProcessed;
        }
    }
}
