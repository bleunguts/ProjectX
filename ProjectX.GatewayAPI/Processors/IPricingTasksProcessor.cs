namespace ProjectX.GatewayAPI.Processors
{
    public interface IPricingTasksProcessor
    {
        Task Process(string message);
    }
}