using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectX.Core.Services
{
    public record Trade(string clientName, int quantity, decimal totalPrice, BuySell buySell, FXProductType productType, string currencyPair, Guid transactionPriceId, decimal transactionPrice);
    public record TradeRequest(FXProductType ProductType, SpotPrice Price, int Quantity, BuySell BuySell, string ClientName, DateTimeOffset PriceTimestamp);
    public record TradeResponse(string ClientName, BuySell BuySell, string currencyPair, int Quantity, decimal TransactionPrice, Guid TransactionPriceId, decimal TotalPrice, DateTimeOffset PriceTimestamp);
}
