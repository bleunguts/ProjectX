using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectX.Core.Strategy
{
    public class SignalEntity
    {
        public required string Ticker { get; set; }
        public DateTime Date { get; set; }
        public double Price { get; set; }        
        public double UpperBand { get; set; }
        public double PricePredicted { get; set; }
        public double LowerBand { get; set; }
        public double Signal { get; set; }

        public override string ToString()
        {
            return $"Date={Date},Price={Price},UpperBand={UpperBand},PricePredicted={PricePredicted},LowerBand={LowerBand},Signal={Signal},Ticker={Ticker}";
        }
    }
}
