namespace ProjectX.Core.Requests
{
    public class PlotOptionsPricingResult : OptionsPricingResultsBase
    {
        public PlotOptionsPricingRequest Request { get; }
        public PlotResults PlotResults { get; }     
        public AuditTrail AuditTrail { get; }
        public PlotOptionsPricingResult(PlotOptionsPricingRequest request, PlotResults plotResults, AuditTrail auditTrail) : base(request.Id)
        {
            Request = request;
            PlotResults = plotResults;
            AuditTrail = auditTrail;
        }
    }
}
