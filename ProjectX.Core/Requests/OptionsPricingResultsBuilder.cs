using ProjectX.Core.Requests;

namespace ProjectX.Core.Services
{
    public class OptionsPricingResultsBuilder
    {
        private readonly IRequest _request;
        private readonly List<OptionGreeksPerMaturityResult> _results;

        public OptionsPricingResultsBuilder(IRequest request)
        {
            _request = request;
            _results = new List<OptionGreeksPerMaturityResult>();
        }

        public void AddResult(double maturity, OptionGreeksResult greeks)  
        {
            _results.Add(new OptionGreeksPerMaturityResult(maturity, greeks));
        } 

        public OptionsPricingResults Build()
        {            
            return new OptionsPricingResults(_request.Id, _results);
        }
    }
}
