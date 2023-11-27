namespace ProjectX.AnalyticsLibNativeShim.Interop;

public static class Extenions
{
    public static OptionType ToOptionType(this ProjectX.Core.OptionType t)
    {
        return t switch
        {
            ProjectX.Core.OptionType.Put => OptionType.Put,
            ProjectX.Core.OptionType.Call => OptionType.Call,
            _ => throw new NotImplementedException(),
        };
    }

    public static VanillaOptionParameters ToOption(this IAPI api, ProjectX.Core.OptionType optionType, double strike, double maturity)
    {
        return new VanillaOptionParameters()
        {
            OptionType = optionType.ToOptionType(),
            Expiry = maturity,
            Strike = strike,
        };
    }
}
