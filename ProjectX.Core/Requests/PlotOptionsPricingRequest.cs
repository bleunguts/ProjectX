using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectX.Core.Requests
{
    public class PlotOptionsPricingRequest : IRequest
    {
        public Guid Id { get; }      
        public OptionGreeks GreekType { get; }
        public OptionType OptionType { get; }        
        public double Strike { get; }
        public double Rate { get; }
        public double Carry { get; }
        public double Vol { get; }
        public string ZLabel { get; set; }
        public int ZDecimalPlaces { get; set; }
        public int ZTickDecimalPlaces { get; set; }

        public PlotOptionsPricingRequest(OptionGreeks greekType, OptionType optionType, double strike, double rate, double carry, double vol) 
        {
            GreekType = greekType;
            OptionType = optionType;
            Strike = strike;
            Rate = rate;
            Carry = carry;
            Vol = vol;
            Id = Guid.NewGuid();
        }

        public void Deconstruct(out OptionGreeks greekType, out OptionType optionType, out double strike, out double rate, out double carry, out double vol)
        {
            greekType = GreekType;
            optionType = OptionType;
            strike = Strike;
            rate = Rate;
            carry = Carry;
            vol = Vol;
        }
    }
}
