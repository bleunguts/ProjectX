using Microsoft.AspNetCore.SignalR;
using ProjectX.Core.Requests;

public interface IStreamHub
{
    Task PricingResults(OptionsPricingByMaturityResults results);
    Task PlotResults(PlotOptionsPricingResult results);
    Task PushFxRate(SpotPriceResult result);
    Task StopFxRate(string ccyPair);
}

public class StreamHub : Hub<IStreamHub>
{
    public void FxRateStopped(string ccyPair)
    {
        Clients.All.StopFxRate(ccyPair);    
    }
    public void FxRateReceived(SpotPriceResult result)
    {
        Clients.All.PushFxRate(result);
    }
    public void OptionsPricingResultsReceived(OptionsPricingByMaturityResults results)
    {
        Clients.All.PricingResults(results);
    }

    public void OptionsPricingPlotResultsReceived(PlotOptionsPricingResult results)
    {
        Clients.All.PlotResults(results);
    }
}