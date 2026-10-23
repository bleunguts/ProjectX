namespace ShellX.Services;

public class MarketDataOptions
{
    /// <summary>Cache file for market data. A relative path is resolved against the app data directory.</summary>
    public string CacheFile { get; set; } = "cache.json";
}
