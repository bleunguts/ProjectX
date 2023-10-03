namespace ProjectX.Core.Requests
{
    public class PlotOptionsPricingResult : OptionsPricingResultsBase
    {
        public PlotOptionsPricingRequest Request { get; }
        public PlotResults PlotResults { get; }     
        public PlotOptionsPricingResult(PlotOptionsPricingRequest request, PlotResults plotResults) : base(request.Id)
        {
            Request = request;
            PlotResults = plotResults;
        }
    }
}
