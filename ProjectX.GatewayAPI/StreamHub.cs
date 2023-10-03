﻿using Microsoft.AspNetCore.SignalR;
using ProjectX.Core.Requests;

public interface IStreamHub
{
    Task PricingResults(OptionsPricingByMaturityResults results);
    Task PlotResults(PlotOptionsPricingResult results);        
}

public class StreamHub : Hub<IStreamHub>
{
    public void OptionsPricingResultsReceived(OptionsPricingByMaturityResults results)
    {
        Clients.All.PricingResults(results);
    }

    public void OptionsPricingPlotResultsReceived(PlotOptionsPricingResult results)
    {
        Clients.All.PlotResults(results);
    }
}