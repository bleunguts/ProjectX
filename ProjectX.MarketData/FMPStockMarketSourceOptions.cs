namespace ProjectX.MarketData;

public class FMPStockMarketSourceOptions
{
    public string ApiKey { get; set; }

    public static string GetFromEnvironment()
    {
        var provider = new Microsoft.Extensions.Configuration.EnvironmentVariables.EnvironmentVariablesConfigurationProvider();
        provider.Load();

        string? apiKey = null;
        if (!provider.TryGet("fmpapikey", out apiKey))
        {
            throw new Exception($"Blow up: cannot get compulsory market data api key from environment variable 'fmpapikey'");
        }
         
        return apiKey;
    }
}