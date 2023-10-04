namespace ProjectX.Core.Requests
{
    public record SpotPriceRequest(string CurrencyPair, string ClientName, FXRateMode Mode)
    {
        public FXProductType ProductType { private set; get; } = FXProductType.Spot;
    }
}
