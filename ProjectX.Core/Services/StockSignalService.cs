﻿using Microsoft.Extensions.Options;
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
        Task<IEnumerable<PriceSignal>> GetSignalUsingMovingAverageByDefault(string ticker, DateTime startDate, DateTime endDate, int movingWindow);        
    }

    public enum MovingAverageImpl { MyImpl, BollingerBandsImpl }

    [Export(typeof(IStockSignalService)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class StockSignalService : IStockSignalService
    {        
        private readonly IStockMarketSource _marketSource;
        private readonly MovingAverageImpl _moveringAverageImpl;

        [ImportingConstructor]
        public StockSignalService(IStockMarketSource marketSource, IOptions<StockSignalServiceOptions> options)
        {
            this._marketSource = marketSource;            
            this._moveringAverageImpl = options?.Value?.MoveringAverageImpl ?? MovingAverageImpl.MyImpl;
        }

        /// <summary>
        /// As a general rule of thumb, you will be safe if you provide 750 points of historical quote data (e.g. 3 years of daily data).
        /// </summary>
        public async Task<IEnumerable<PriceSignal>> GetSignalUsingMovingAverageByDefault(string ticker, DateTime startDate, DateTime endDate, int movingWindow)
        {
            switch(_moveringAverageImpl)
            {
                case MovingAverageImpl.MyImpl:
                    return await MovingAverageMyImpl(ticker, startDate, endDate, movingWindow);
                case MovingAverageImpl.BollingerBandsImpl:
                    return await MovingAverageBollingerBandsImpl(ticker, startDate, endDate, movingWindow);
            }
            throw new NotSupportedException(nameof(_moveringAverageImpl));
        }

        public async Task<IEnumerable<PriceSignal>> MovingAverageMyImpl(string ticker, DateTime startDate, DateTime endDate, int movingWindow)
        {
            var marketPrices = await _marketSource.GetPrices(ticker, startDate, endDate);

            return marketPrices.MovingAverage(movingWindow).ToList();
        }

        public async Task<IEnumerable<PriceSignal>> MovingAverageBollingerBandsImpl(string ticker, DateTime startDate, DateTime endDate, int movingWindow)
        {
            var quotes = await _marketSource.GetQuote(ticker, startDate, endDate);
            var bollingerBands = quotes
                                    .Use(CandlePart.Close)
                                    .GetBollingerBands(5)
                                    .RemoveWarmupPeriods();
            
            var computedSignals = new List<PriceSignal>();
            foreach (var b in bollingerBands)
            {                
                var quoteForThisDate = quotes.FirstOrDefault(q => q.Date == b.Date);
                var signal = new PriceSignal()
                {
                    Ticker = ticker,
                    Date = b.Date,
                    Price = quoteForThisDate != null ? quoteForThisDate.Close : Convert.ToDecimal(b.Sma),
                    PricePredicted = Convert.ToDecimal(b.Sma),
                    Signal = Convert.ToDecimal(b.ZScore),
                    UpperBand = Convert.ToDecimal(b.UpperBand),
                    LowerBand = Convert.ToDecimal(b.LowerBand),                                        
                };
                computedSignals.Add(signal);
            }
            return computedSignals;
        }      
    }
}
