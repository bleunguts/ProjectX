using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectX.Core.Requests
{
    public class MultipleTimeslicesOptionsPricingRequest : IRequest
    {
        public Guid Id { get; }
        public int TimeSlices { get; }
        public OptionType OptionType { get; }
        public double Spot { get; }
        public double Strike { get; }
        public double Rate { get; }
        public double Carry { get; }
        public double Vol { get; }

        public MultipleTimeslicesOptionsPricingRequest(int timeSlices, OptionType optionType, double spot, double strike, double rate, double carry, double vol)
        {
            TimeSlices = timeSlices;
            OptionType = optionType;
            Spot = spot;
            Strike = strike;
            Rate = rate;
            Carry = carry;
            Vol = vol;
            Id = Guid.NewGuid();
        }
    }
}
