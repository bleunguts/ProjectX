using ProjectX.Core.Requests;

namespace ProjectX.GatewayAPI.Processors
{
    public interface IPricingTasksProcessor
    {        
        Task Process(OptionsPricingByMaturitiesRequest multipleTimeslicesOptionsPricingRequest, CancellationToken cancellationToken);
    }
}