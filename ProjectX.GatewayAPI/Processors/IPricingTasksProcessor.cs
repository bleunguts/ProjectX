using ProjectX.Core.Requests;

namespace ProjectX.GatewayAPI.Processors
{
    public interface IPricingTasksProcessor
    {        
        Task Process(MultipleTimeslicesOptionsPricingRequest multipleTimeslicesOptionsPricingRequest, CancellationToken cancellationToken);
    }
}