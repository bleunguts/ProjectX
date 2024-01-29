using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectX.Core
{
    public static class Endpoints
    {
        public const string RequestsPriceOptionBS = "PricingTasks/bsPrice";
        public const string RequestsPlotOptionBS = "PricingTasks/bsPlot";
        public const string ResultsPriceOptionBS = "PricingResults/bsPrice";
        public const string ResultsPlotOptionBS = "PricingResults/bsPlot";
        public const string FXSpotPriceStart = "FXPricing";
        public const string FXSpotPriceEnd = "FXPricing";
        public const string BacktestLongShortPnlMatrix = "BacktestService/ComputeLongShortPnlMatrix";
        public const string BacktestLongShortPnl = "BacktestService/ComputeLongShortPnl";
        public const string StockSignalMovingAverage = "StockSignal/MovingAverageSignals";
        public const string StockSignalHursts = "StockSignal/Hursts";
    }
}
