using ProjectX.Core.Requests;

namespace ProjectX.GatewayAPI.Processors
{
    public interface IPricingTasksProcessor
    {        
        Task Process(IRequest request, CancellationToken cancellationToken);
    }
}