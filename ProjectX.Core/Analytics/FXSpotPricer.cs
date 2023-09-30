﻿namespace ProjectX.Core.Analytics
{
    public interface IFXPricer
    {
        SpotPrice Price(string ccyPair, SpotPrice spotPrice, int spreadInPips);
    }
    public class FXSpotPricer : IFXPricer
    {
        // potential complex calculations go here... 
        // this may be a long running operation and may need to talk to external services, run on grids, or use lots of threads.
        public SpotPrice Price(string ccyPair, SpotPrice spotPrice, int spreadInPips)
        {
            var spreadInDecimal = spreadInPips / 10000M;

            var rawSpread = spotPrice.AskPrice - spotPrice.BidPrice;
            var rawdifference = rawSpread / 2;
            var rawMid = spotPrice.BidPrice + rawdifference;
            var finalSpread = rawSpread + spreadInDecimal;
            var finaldifference = finalSpread / 2;

            var bidPrice = rawMid - finaldifference;
            var askPrice = rawMid + finaldifference;

            return new SpotPrice(ccyPair, bidPrice, askPrice);
        }
    }
}

