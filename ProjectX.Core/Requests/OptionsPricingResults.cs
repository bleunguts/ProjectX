using ProjectX.Core.Requests;
using System.Collections;
using System.Linq;

namespace ProjectX.Core.Services
{
    public class OptionsPricingResults
    {
        private readonly List<(double maturities, OptionGreeksResult optionGreeks)> _results = new List<(double maturities, OptionGreeksResult optionGreeks)>();
        public List<(double maturities, OptionGreeksResult optionGreeks)> Results => _results;
        public Guid RequestId { get; }
        public int ResultsCount { get; }
        public (double maturities, OptionGreeksResult optionGreeks) this[int index] => (_results[index]);
        public OptionsPricingResults(Guid requestId, List<(double maturities, OptionGreeksResult optionGreeks)> results)
        {
            RequestId = requestId;
            ResultsCount = results.Count();
            _results = results;
        }
        public string Maturities() => string.Join(',', _results.Select(x => x.maturities));
        public string Prices() => string.Join(',', _results.Select(x => x.optionGreeks.price));
        public override string ToString() => $"{ResultsCount} results, maturties: {Maturities()}, prices: {Prices()}";
    } 
}
