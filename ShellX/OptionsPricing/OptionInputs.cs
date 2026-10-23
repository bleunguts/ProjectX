using System.Globalization;
using ProjectX.Core;
using ProjectX.Core.Requests;

namespace ShellX.OptionsPricing;

/// <summary>The option parameters as typed on screen; parsed only when a request is built.</summary>
public record OptionInputs(string OptionType = "Call", string Spot = "100", string Strike = "100", string Rate = "0.1", string Carry = "0.04", string Vol = "0.3")
{
    public OptionsPricingByMaturitiesRequest ToPricingRequest(OptionsPricingCalculatorType calculatorType) =>
        new(10, ParseOptionType(), Number(nameof(Spot), Spot), Number(nameof(Strike), Strike), Number(nameof(Rate), Rate),
            Number(nameof(Carry), Carry), Number(nameof(Vol), Vol), calculatorType);

    public PlotOptionsPricingRequest ToPlotRequest(OptionGreeks greek, string zLabel, int zDecimalPlaces, int zTickDecimalPlaces, OptionsPricingCalculatorType calculatorType) =>
        new(greek, ParseOptionType(), Number(nameof(Strike), Strike), Number(nameof(Rate), Rate), Number(nameof(Carry), Carry), Number(nameof(Vol), Vol), calculatorType)
        {
            ZLabel = zLabel,
            ZDecimalPlaces = zDecimalPlaces,
            ZTickDecimalPlaces = zTickDecimalPlaces,
        };

    private ProjectX.Core.OptionType ParseOptionType() => OptionType.Trim().ToLowerInvariant() switch
    {
        "call" => ProjectX.Core.OptionType.Call,
        "put" => ProjectX.Core.OptionType.Put,
        _ => throw new NotSupportedException($"OptionType '{OptionType}' is not supported, use Call or Put."),
    };

    private static double Number(string parameter, string value) =>
        double.TryParse(value, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.CurrentCulture, out var number)
            ? number
            : throw new FormatException($"'{value}' is not a valid number for {parameter}.");
}
