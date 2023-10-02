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
        public void Deconstruct(out int timeSlices, out OptionType optionType, out double spot, out double strike, out double rate, out double carry, out double vol)
        {
            timeSlices = TimeSlices;
            optionType = OptionType;
            spot = Spot;
            strike = Strike;
            rate = Rate;
            carry = Carry;
            vol = Vol;
        }
    }
}
