using ProjectX.Core.Requests;
using System.Collections;
using System.Linq;

namespace ProjectX.Core.Services
{
    public record OptionGreeksPerMaturityResult(double Maturity, OptionGreeksResult OptionGreeks);

    public class OptionsPricingResults
    {
        private readonly List<OptionGreeksPerMaturityResult> _results;
        public List<OptionGreeksPerMaturityResult> Results => _results;
        public Guid RequestId { get; }

        public int ResultsCount 
        {
            get
            {
                if (_results == null) return 0;
                return _results.Count;
            }
        }
        public OptionGreeksPerMaturityResult this[int index] => _results[index];
        public OptionsPricingResults(Guid requestId, List<OptionGreeksPerMaturityResult> results)
        {
            RequestId = requestId;
            _results = results;
        }
        public string Maturities() => string.Join(',', _results.Select((Func<OptionGreeksPerMaturityResult, double>)(x => x.Maturity)));
        public string Prices() => string.Join(',', _results.Select((Func<OptionGreeksPerMaturityResult, double>)(x => x.OptionGreeks.price)));
        public override string ToString() => $"{ResultsCount} results, maturties: {Maturities()}, prices: {Prices()}";
    } 
}
