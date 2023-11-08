using ProjectXAnalyticsCppLib;
using System.Diagnostics;
using OptionType = ProjectX.Core.OptionType;

namespace ProjectX.AnalyticsLib.OptionsCalculators;

public class MonteCarloOptionsPricerCppWrapper : IMonteCarloOptionsPricerCpp
{
    private readonly MonteCarloOptionsPricer _pricer = new MonteCarloOptionsPricer();
    private readonly int _numberOfMcPaths;
    private long _timeTaken;

    public MonteCarloOptionsPricerCppWrapper(int numberOfMcPaths = 1000)
    {
        _numberOfMcPaths = numberOfMcPaths;
    }
    public long TimeTaken { get => _timeTaken; set => _timeTaken = value; }

    private void Execute(OptionType optionType, double spot, double strike, double rate, double carry, double maturity, double volatility)
    {
        var sw = Stopwatch.StartNew();
        _pricer.Execute(spot, strike, rate, maturity, volatility, _numberOfMcPaths);
        sw.Stop();

        _timeTaken = sw.ElapsedMilliseconds;
    }

    public double Delta(OptionType optionType, double spot, double strike, double rate, double carry, double maturity, double volatility)
    {
        Execute(optionType, spot, strike, rate, carry, maturity, volatility);
        return _pricer.Delta();
    }

    public double Gamma(OptionType optionType, double spot, double strike, double rate, double carry, double maturity, double volatility)
    {
        Execute(optionType, spot, strike, rate, carry, maturity, volatility);
        return _pricer.Gamma();
    }

    public double ImpliedVol(OptionType optionType, double spot, double strike, double rate, double carry, double maturity, double price)
    {
        return 0.0;
    }

    public double PV(OptionType optionType, double spot, double strike, double rate, double carry, double maturity, double volatility)
    {
        Execute(optionType, spot, strike, rate, carry, maturity, volatility);
        return _pricer.PV();
    }

    public double Rho(OptionType optionType, double spot, double strike, double rate, double carry, double maturity, double volatility)
    {
        Execute(optionType, spot, strike, rate, carry, maturity, volatility);
        return _pricer.Rho();
    }

    public double Theta(OptionType optionType, double spot, double strike, double rate, double carry, double maturity, double volatility)
    {
        Execute(optionType, spot, strike, rate, carry, maturity, volatility);
        return _pricer.Theta();
    }

    public double Vega(OptionType optionType, double spot, double strike, double rate, double carry, double maturity, double volatility)
    {
        Execute(optionType, spot, strike, rate, carry, maturity, volatility);
        return _pricer.Vega();
    }
}