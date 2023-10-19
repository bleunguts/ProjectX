﻿using ProjectX.Core.Strategy;
using Skender.Stock.Indicators;
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
        internal async Task<List<MarketPrice>> FetchMarketPricesAsync(DateTime startDate, DateTime endDate)
        {            
            var prices = await _marketSource.GetPrices(_ticker, startDate, endDate);
            return prices.ToList();
        }        
        public async Task<List<SignalEntity>> GetSignalUsingMovingAverageByDefault(DateTime startDate, DateTime endDate, int movingWindow)
        {
            List<MarketPrice> marketPrices = await FetchMarketPricesAsync(startDate, endDate);
            
            var processed = marketPrices                            
                            .GetMaEnvelopes(movingWindow)         
                            .ToList();

            List<SignalEntity> signalsProcessed = new List<SignalEntity>();
            for (int i = 0; i < processed.Count - 1; i++)
            {                
                var signal = processed[i];                
                if(signal == null)
                {
                    throw new ArgumentNullException(nameof(signal), "Signal should not be null");
                }                                                
                signalsProcessed.Add(new SignalEntity
                {
                    Ticker = _ticker,
                    Date = signal.Date,
                    Price = Convert.ToDouble(marketPrices[i].Close),
                    PricePredicted = signal.Centerline ?? Double.MaxValue,
                    LowerBand = signal.LowerEnvelope ?? Double.MaxValue,
                    UpperBand = signal.UpperEnvelope ?? Double.MaxValue,
                    Signal = signal.Centerline ?? Double.MaxValue,
                });
            }
            return signalsProcessed;
        }
    }
}
