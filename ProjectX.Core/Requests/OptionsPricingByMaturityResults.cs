using System.Collections;
using System.Linq;

namespace ProjectX.Core.Requests
{
    public record MaturityAndOptionGreeksResultPair(double Maturity, OptionGreeksResult OptionGreeks);

    public class OptionsPricingByMaturityResults : OptionsPricingResultsBase
    {
        public List<MaturityAndOptionGreeksResultPair> Results { get; }

        public int ResultsCount
        {
            get
            {
                if (Results == null) return 0;
                return Results.Count;
            }
        }
        public MaturityAndOptionGreeksResultPair this[int index] => Results[index];        
    
        public OptionsPricingByMaturityResults(Guid RequestId, List<MaturityAndOptionGreeksResultPair> results) : base(RequestId)
        {
            Results = results;
        }

        public override string ToString() => $"{ResultsCount} results";       
    }
}
