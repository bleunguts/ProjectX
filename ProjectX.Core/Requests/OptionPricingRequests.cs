using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectX.Core.Requests
{
    public record MultipleTimeslicesOptionsPricingRequest(int timeSlices, OptionType optionType, double spot, double strike, double rate, double carry, double vol)
    { 
        public Guid Id { get; } = Guid.NewGuid();   
    }
}
