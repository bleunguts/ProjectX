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
        public OptionType OptionType { get; }        
        public double Strike { get; }
        public double Rate { get; }
        public double Carry { get; }
        public double Vol { get; }

        public PlotOptionsPricingRequest(OptionType optionType, double strike, double rate, double carry, double vol) 
        {
            OptionType = optionType;
            Strike = strike;
            Rate = rate;
            Carry = carry;
            Vol = vol;
            Id = Guid.NewGuid();
        }
    }
}
