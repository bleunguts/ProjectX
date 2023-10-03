using ProjectX.Core.Requests;

namespace ProjectX.Core.Services
{
    public class OptionsPricingResultsBuilder
    {
        private readonly IRequest _request;
        private readonly List<MaturityAndOptionGreeksResultPair> _results;

        public OptionsPricingResultsBuilder(IRequest request)
        {
            _request = request;
            _results = new List<MaturityAndOptionGreeksResultPair>();
        }

        public void AddResult(double maturity, OptionGreeksResult greeks)  
        {
            _results.Add(new MaturityAndOptionGreeksResultPair(maturity, greeks));
        } 

        public OptionsPricingResults Build()
        {            
            return new OptionsPricingResults(_request.Id, _results);
        }
    }
}
