using BenchmarkDotNet.Attributes;
using ProjectX.AnalyticsLib.OptionsCalculators;
using ProjectX.AnalyticsLibNativeShim;

namespace ProjectX.AnalyticsLib.PerformanceTests;

[MemoryDiagnoser]
//[MinInvokeCount(3), InvocationCount(16)]
//[MinWarmupCount(3), MaxWarmupCount(5)]
//[MinIterationCount(3), MaxIterationCount(5)]
public class ManagedVsNativeBenchmarks
{    
    private const Core.OptionType _optionType = Core.OptionType.Call;
    private const double _spot = 100.0;
    private const double _strike = 110.0;
    private const double _rate = 0.10;
    private const double _carry = 0.0;
    private const double _maturity = 1.0;
    private const double _volatility = 0.3;

    private readonly BlackScholesCppOptionsPricerWrapper _native = new(new API());
    private readonly BlackScholesOptionsPricer _managed = new();

    [Benchmark(Description = "NativeCode")]
    public void NativeTest() => Console.WriteLine(_native.PV(_optionType, _spot, _strike, _rate, _carry, _maturity, _volatility));

    [Benchmark(Description = "Managed")]
    public void ManagedTest() => Console.WriteLine(_managed.PV(Core.OptionType.Call, _spot, _strike, _rate, _carry, _maturity, _volatility));    
}