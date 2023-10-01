using Microsoft.Extensions.Logging;
using ProjectX.Core;
using System;
using System.ComponentModel.Composition;
using System.Text;

namespace ProjectX.Core.Services;

public interface IFXTrader
{
    eFXTrader.TradeResponse ExecuteTrade(eFXTrader.TradeRequest request);
    Dictionary<string, (int netQuantity, int totalTrades, decimal PnL, string debug)> PositionsFor(string clientName);
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
        Guid priceId = request.Price.PriceId;
        string currencyPair = request.Price.CurrencyPair;
        _logger.LogInformation($"TradeExecuteRequest @ PriceId={priceId} ");

        // this could be a long operation
        (decimal purchasePrice, decimal totalPrice) = PriceTrade(request.ProductType, request.BuySell, request.Quantity, request.Price);

        var response = new TradeResponse(request.ClientName, request.BuySell, currencyPair, request.Quantity, purchasePrice, priceId, totalPrice, request.PriceTimestamp);
        _tradeStore.Add(new Trade(request.ClientName, response.Quantity, response.TotalPrice, request.BuySell, request.ProductType, currencyPair, response.TransactionPriceId, response.TransactionPrice));
        _logger.LogInformation("Trade executed and captured into trade store.");

        _logger.LogInformation($"TradeExecuteResponse @ Price={response.TransactionPrice} PriceId={response.TransactionPriceId} BuySell:{response.BuySell} Quantity:{response.Quantity} TotalPrice:{response.TotalPrice}");
        return response;
    }

    public Dictionary<string, (int netQuantity, int totalTrades, decimal PnL, string debug)> PositionsFor(string clientName)       
    {
        var byCurrencyPair = _tradeStore.Where(t => t.clientName == clientName)
                                        .GroupBy(t => t.currencyPair);

        Dictionary<string, (int, int, decimal, string)> positions = new Dictionary<string, (int, int, decimal, string)>();

        foreach(var group in byCurrencyPair)
        {
            var currencyPair = group.Key;
            int netQuantity = 0;
            int totalTrades = 0;
            decimal pnl = 0.0M;
            var debug = new StringBuilder();            
            foreach(var pair in group)
            {
                var quantity = pair.buySell == BuySell.Buy ? pair.quantity : -pair.quantity;
                netQuantity += quantity;
                totalTrades++;
                var totalPrice = pair.buySell == BuySell.Buy ? pair.totalPrice : -pair.totalPrice;
                pnl += totalPrice;
                debug.Append($"({totalTrades}):{pair.quantity},{pair.transactionPrice},{pair.totalPrice};");
                debug.AppendLine();
            }
            positions[currencyPair] = (netQuantity, totalTrades, pnl, debug.ToString());
        }

        return positions;
    }

    public (decimal purchasePrice, decimal totalPrice) PriceTrade(FXProductType productType, BuySell buySell, int quantity, SpotPrice price)
    {
        var purchasePrice = buySell == BuySell.Buy ? price.AskPrice : price.BidPrice;
        var totalPrice = purchasePrice * quantity;
        return (purchasePrice, totalPrice);
    }   
    public record Trade(string clientName, int quantity, decimal totalPrice, BuySell buySell, FXProductType productType, string currencyPair, Guid transactionPriceId, decimal transactionPrice);
    public record TradeRequest(FXProductType ProductType, SpotPrice Price, int Quantity, BuySell BuySell, string ClientName, DateTimeOffset PriceTimestamp);
    public record TradeResponse(string ClientName, BuySell BuySell, string currencyPair, int Quantity, decimal TransactionPrice, Guid TransactionPriceId, decimal TotalPrice, DateTimeOffset PriceTimestamp);
}
