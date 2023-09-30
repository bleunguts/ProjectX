namespace ProjectX.Core.Services
{
    public record OptionPricerResult(double price, double delta, double gamma, double theta, double rho, double vega);
}
