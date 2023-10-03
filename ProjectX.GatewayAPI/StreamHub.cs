using Microsoft.AspNetCore.SignalR;
using ProjectX.Core.Requests;

public interface IStreamHub
{
    Task PricingResults(OptionsPricingByMaturityResults results);    
    Task PricingResults(PlotOptionsPricingResult results);    
}

public class StreamHub : Hub<IStreamHub>
{
    public void OptionsPricingResultsReceived(OptionsPricingByMaturityResults results)
    {
        Clients.All.PricingResults(results);
    }

    public void OptionsPricingResultsReceived2(PlotOptionsPricingResult results)
    {
        Clients.All.PricingResults(results);
    }
}