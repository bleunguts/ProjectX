using ProjectX.Core;

namespace ProjectX.GatewayAPI.Processors
{
    public interface IPricingTasksProcessor
    {        
        Task Process(IRequest request, CancellationToken cancellationToken);
    }
}