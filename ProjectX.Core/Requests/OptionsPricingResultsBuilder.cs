using ProjectX.Core.Requests;

namespace ProjectX.Core.Services
{
    public class OptionsPricingResultsBuilder
    {
        private readonly IRequest _request;
        private readonly List<(double maturity, OptionGreeksResult greeks)> _results;

        public OptionsPricingResultsBuilder(IRequest request)
        {
            _request = request;
            _results = new List<(double maturity, OptionGreeksResult greeks)>();
        }

        public void AddResult(double maturity, OptionGreeksResult greeks)  
        {
            _results.Add((maturity, greeks));
        } 

        public OptionsPricingResults Build()
        {
            List<(double Key, OptionGreeksResult Value)> x = _results.Select(x => (x.maturity, x.greeks)).ToList();
            return new OptionsPricingResults(_request.Id, x);
        }
    }
}
