using ProjectXAnalyticsCppLib;
using OptionType = ProjectX.Core.OptionType;

namespace ProjectX.AnalyticsLib.OptionsCalculators
{
    public class MonteCarloSimulationCache
    {
        private IMonteCarloCppPricer _calculator { get; }
       
        private readonly Dictionary<ExecutionKey, GreekResults> _cache = new();

        public MonteCarloSimulationCache(IMonteCarloCppPricer pricer)
        {
            _calculator = pricer;
        }

        public GreekResults RunSimulation(ExecutionKey key, OptionType optionType, double spot, double strike, double rate, double maturity, double volatility, ulong numOfMcPaths)
        {
            if (!_cache.TryGetValue(key, out GreekResults? greekResult))
            {
                var param = new VanillaOptionParameters(optionType.ToNativeOptionType(), strike, maturity);
                var result = _calculator.MCValue(ref param, spot, volatility, rate, numOfMcPaths);

                _cache.Add(key, result);
                greekResult = result;
            }
            return greekResult;
        }
        public static ExecutionKey Key(double spot, double strike, double rate, double carry, double maturity, double volatility)
        {
            return new ExecutionKey(spot, strike, rate, carry, maturity, volatility);
        }
        public class ExecutionKey
        {
            public ExecutionKey(double spot, double strike, double rate, double carry, double maturity, double volatility)
            {
                Spot = spot;
                Strike = strike;
                Rate = rate;
                Carry = carry;
                Maturity = maturity;
                Volatility = volatility;
            }
            public double Spot { get; }
            public double Strike { get; }
            public double Rate { get; }
            public double Carry { get; }
            public double Maturity { get; }
            public double Volatility { get; }

            public override bool Equals(object? obj)
            {
                return obj is ExecutionKey key &&
                       Spot == key.Spot &&
                       Strike == key.Strike &&
                       Rate == key.Rate &&
                       Carry == key.Carry &&
                       Maturity == key.Maturity &&
                       Volatility == key.Volatility;
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(Spot, Strike, Rate, Carry, Maturity, Volatility);
            }
        }
    }
}