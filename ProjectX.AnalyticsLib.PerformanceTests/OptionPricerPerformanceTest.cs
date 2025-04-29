using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Running;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using ProjectX.AnalyticsLib.OptionsCalculators;
using ProjectX.AnalyticsLibNativeShim;
using Xunit;
using Xunit.Abstractions;

namespace ProjectX.AnalyticsLib.PerformanceTests;
public class OptionPricerPerformanceTest
{
    private readonly ITestOutputHelper output;

    public OptionPricerPerformanceTest(ITestOutputHelper output)
    {
        this.output = output;
    }      

    [Fact]
    public void PerformanceTest()
    {
        var logger = new AccumulationLogger();

        var config = ManualConfig.Create(DefaultConfig.Instance)
            .AddLogger(logger)
            .WithOptions(ConfigOptions.DisableOptimizationsValidator);        
                       
        BenchmarkRunner.Run<ManagedVsNativeBenchmarks>(config);

        // write benchmark summary
        output.WriteLine(logger.GetLog());
    }    
}
