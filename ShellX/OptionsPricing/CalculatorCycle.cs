using ProjectX.Core.Requests;

namespace ShellX.OptionsPricing;

public static class CalculatorCycle
{
    public static (OptionsPricingCalculatorType Type, string Description) Next(OptionsPricingCalculatorType current) => current switch
    {
        OptionsPricingCalculatorType.OptionsPricer => (OptionsPricingCalculatorType.OptionsPricerCpp, "BlackScholes Options Pricer native C++"),
        OptionsPricingCalculatorType.OptionsPricerCpp => (OptionsPricingCalculatorType.MonteCarloCppPricer, "MonteCarlo Pricing native C++ (1e8 MC Paths)"),
        OptionsPricingCalculatorType.MonteCarloCppPricer => (OptionsPricingCalculatorType.OptionsPricer, "Vanilla BlackScholes Options Pricer C#"),
        _ => throw new NotImplementedException(),
    };
}
