using Microsoft.Extensions.Logging;
using ProjectX.Core;
using System;
using System.ComponentModel.Composition;

namespace ProjectX.Core.Services;

public interface IFXTrader
{
    eFXTrader.TradeResponse ExecuteTrade(eFXTrader.TradeRequest request);
    Dictionary<string, int> PositionsFor(string clientName);
    (decimal purchasePrice, decimal totalPrice) PriceTrade(FXProductType productType, BuySell buySell, int notional, SpotPrice price);
}

[Export(typeof(IFXTrader)), PartCreationPolicy(CreationPolicy.Shared)]

public class eFXTrader : IFXTrader
{
    private readonly ILogger<eFXTrader> _logger;
    private readonly List<Trade> _tradeStore = new List<Trade>();

    [ImportingConstructor]
    public eFXTrader(ILogger<eFXTrader> logger)
    {
        _logger = logger;
    }
    public TradeResponse ExecuteTrade(TradeRequest request)
    {
        _logger.LogInformation($"TradeExecuteRequest @ PriceId={request.Price.PriceId} ");

        // this could be a long operation
        (decimal purchasePrice, decimal totalPrice) = PriceTrade(request.ProductType, request.BuySell, request.Quantity, request.Price);        

        var response = new TradeResponse(request.ClientName, request.BuySell, request.Quantity, purchasePrice, request.Price.PriceId, totalPrice, request.PriceTimestamp);
        _tradeStore.Add(new Trade(request.ClientName, response.Quantity, response.TotalPrice, request.BuySell, request.ProductType, request.Price.CurrencyPair, response.TransactionPriceId, response.TransactionPrice));
        _logger.LogInformation("Trade executed and captured into trade store.");

        _logger.LogInformation($"TradeExecuteResponse @ Price={response.TransactionPrice} PriceId={response.TransactionPriceId} BuySell:{response.BuySell} Quantity:{response.Quantity} TotalPrice:{response.TotalPrice}");
        return response;
    }

    public Dictionary<string, int> PositionsFor(string clientName)
    {
        throw new NotImplementedException();
    }

    public (decimal purchasePrice, decimal totalPrice) PriceTrade(FXProductType productType, BuySell buySell, int quantity, SpotPrice price)
    {
        var purchasePrice = buySell == BuySell.Buy ? price.AskPrice : price.BidPrice;
        var totalPrice = purchasePrice * quantity;
        return (purchasePrice, totalPrice);
    }
    public record Trade(string clientName, int quantity, decimal totalPrice, BuySell buySell, FXProductType productType, string currencyPair, Guid transactionPriceId, decimal transactionPrice);
    public record TradeRequest(FXProductType ProductType, SpotPrice Price, int Quantity, BuySell BuySell, string ClientName, DateTimeOffset PriceTimestamp);
    public record TradeResponse(string ClientName, BuySell BuySell, int Quantity, decimal TransactionPrice, Guid TransactionPriceId, decimal TotalPrice, DateTimeOffset PriceTimestamp);
}
