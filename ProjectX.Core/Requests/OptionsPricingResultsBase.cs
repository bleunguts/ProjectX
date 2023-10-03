namespace ProjectX.Core.Requests
{
    public abstract class OptionsPricingResultsBase
    {
        public Guid RequestId { get; }

        public OptionsPricingResultsBase(Guid RequestId)
        {
            this.RequestId = RequestId;
        }
    }
}
