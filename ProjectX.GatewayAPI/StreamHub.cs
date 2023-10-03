using Microsoft.AspNetCore.SignalR;
using ProjectX.Core.Services;

public interface IStreamHub
{
    Task PricingResults(OptionsPricingResults results);    
}

public class StreamHub : Hub<IStreamHub>
{
    public void OptionsPricingResultsReceived(OptionsPricingResults results)
    {
        Clients.All.PricingResults(results);
    }
}