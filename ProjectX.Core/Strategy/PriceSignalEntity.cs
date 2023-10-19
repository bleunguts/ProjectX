using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectX.Core.Strategy
{
    public class PriceSignalEntity
    {
        public required string Ticker { get; set; }
        public DateTime Date { get; set; }
        public decimal Price { get; set; }        
        public decimal PricePredicted { get; set; }
        public decimal UpperBand { get; set; }
        public decimal LowerBand { get; set; }
        public decimal Signal { get; set; }

        public override string ToString()
        {
            return $"Date={Date},Price={Price},UpperBand={UpperBand},PricePredicted={PricePredicted},LowerBand={LowerBand},Signal={Signal},Ticker={Ticker}";
        }
    }
}
