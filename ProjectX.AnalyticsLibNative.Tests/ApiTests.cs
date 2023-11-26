using ProjectX.AnalyticsLibNativeShim;

namespace ProjectX.AnalyticsLibNative.Tests;

public class ApiTests
{
    private API _api;

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        Utils.AddEnvironmentPaths(new[] { @"..\..\..\..\ProjectX.AnalyticsLibNative\out\build\x64-debug\bin" });
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        Utils.UnloadImportedDll("ProjectX.AnalyticsLibNative");
    }

    [SetUp]
    public void Setup()
    {
        _api = new API();
    }

    [Test]
    public void WhenCallingBSMPV()
    {
        var Spot = 100;
        var Vol = 0.3;
        var r = 0.1;
        var callOption = new VanillaOptionParameters()
        {
            OptionType = OptionType.Call,
            Strike = 110,
            Expiry = 0.5
        };
        
        var pv = _api.BlackScholes_PV(callOption, Spot, Vol, r);
        Console.WriteLine($"PV: {pv}");
    }
}