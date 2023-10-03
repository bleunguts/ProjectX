namespace ProjectX.Core.Requests
{
    public class PlotOptionsPricingResult : OptionsPricingResultsBase
    {
        public PlotResults PlotResults { get; }
        public PlotOptionsPricingResult(Guid RequestId, PlotResults plotResults) : base(RequestId)
        {
            PlotResults = plotResults;
        }
    }
}
