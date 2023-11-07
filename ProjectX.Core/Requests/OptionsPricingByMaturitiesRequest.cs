namespace ProjectX.Core.Requests
{
    public class OptionsPricingByMaturitiesRequest : IRequest
    {
        public Guid Id { get; }
        public int TimeSlices { get; }
        public OptionType OptionType { get; }
        public double Spot { get; }
        public double Strike { get; }
        public double Rate { get; }
        public double Carry { get; }
        public double Vol { get; }
        public OptionsPricingCalculatorType CalculatorType { get; }

        public OptionsPricingByMaturitiesRequest(int timeSlices, OptionType optionType, double spot, double strike, double rate, double carry, double vol, OptionsPricingCalculatorType calculatorType)
        {
            TimeSlices = timeSlices;
            OptionType = optionType;
            Spot = spot;
            Strike = strike;
            Rate = rate;
            Carry = carry;
            Vol = vol;
            CalculatorType = calculatorType;
            Id = Guid.NewGuid();
        }
        public void Deconstruct(out int timeSlices, out OptionType optionType, out double spot, out double strike, out double rate, out double carry, out double vol, out OptionsPricingCalculatorType calculatorType)
        {
            timeSlices = TimeSlices;
            optionType = OptionType;
            spot = Spot;
            strike = Strike;
            rate = Rate;
            carry = Carry;
            vol = Vol;
            calculatorType = CalculatorType;
        }
    }
}
