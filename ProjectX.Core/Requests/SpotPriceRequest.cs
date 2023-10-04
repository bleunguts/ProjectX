namespace ProjectX.Core.Requests
{
    public record SpotPriceRequest(string CurrencyPair, string ClientName, SpotPriceSubscriptionMode Mode)
    {
        public FXProductType ProductType { private set; get; } = FXProductType.Spot;
    }
}
