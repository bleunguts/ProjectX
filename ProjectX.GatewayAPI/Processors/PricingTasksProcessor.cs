namespace ProjectX.GatewayAPI.Processors
{
    public class PricingTasksProcessor : IPricingTasksProcessor
    {

        public Task Process(string message)
        {
            return Task.CompletedTask;
        }
    }
}
