using ProjectX.Core.Requests;
using System.Collections;
using System.Linq;

namespace ProjectX.Core.Services
{
    public record OptionGreeksPerMaturityResult(double Maturity, OptionGreeksResult OptionGreeks);

    public class OptionsPricingResults
    {        
        public List<OptionGreeksPerMaturityResult> Results { get; }
        public Guid RequestId { get; }

        public int ResultsCount 
        {
            get
            {
                if (Results == null) return 0;
                return Results.Count;
            }
        }
        public OptionGreeksPerMaturityResult this[int index] => Results[index];
        public OptionsPricingResults(Guid requestId, List<OptionGreeksPerMaturityResult> results)
        {
            RequestId = requestId;
            Results = results;
        }
        public string Maturities() => string.Join(',', Results.Select((Func<OptionGreeksPerMaturityResult, double>)(x => x.Maturity)));
        public string Prices() => string.Join(',', Results.Select((Func<OptionGreeksPerMaturityResult, double>)(x => x.OptionGreeks.price)));
        public override string ToString() => $"{ResultsCount} results, maturties: {Maturities()}, prices: {Prices()}";
    } 
}
