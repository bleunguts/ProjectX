using ProjectX.AnalyticsLibNativeShim;
using ProjectX.AnalyticsLibNativeShim.Interop;

namespace ProjectX.AnalyticsLibNative.Tests;

public class ApiTests
{     
    #region setup

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
        _api = API.Instance;
    }
    #endregion

    private API _api;

    double Spot = 100;
    double Vol = 0.3;
    double r = 0.1;
    double epsilon = 1e-6;    
    VanillaOptionParameters callOption = new VanillaOptionParameters()
    {
        OptionType = OptionType.Call,
        Strike = 110,
        Expiry = 0.5
    };

    [Test]
    public void WhenCallingBSM_PV()
    {               
        var value = _api.BlackScholes_PV(callOption, Spot, Vol, r);
        Console.WriteLine($"PV: {value}");
    }

    [Test]
    public void WhenCallingBSM_Delta()
    {
        var value = _api.BlackScholes_Delta(callOption, Spot, Vol, r);
        Console.WriteLine($"Delta: {value}");
    }

    [Test]
    public void WhenCallingBSM_Gamma()
    {
        var value = _api.BlackScholes_Gamma(callOption, Spot, Vol, r, epsilon);
        Console.WriteLine($"Gamma: {value}");
    }

    [Test]
    public void WhenCallingBSM_Rho()
    {
        var value = _api.BlackScholes_Rho(callOption, Spot, Vol, r);
        Console.WriteLine($"Rho: {value}");
    }

    [Test]
    public void WhenCallingBSM_Vega()
    {
        var value = _api.BlackScholes_Vega(callOption, Spot, Vol, r);
        Console.WriteLine($"Vega: {value}");
    }

    [Test]
    public void WhenCallingBSM_Theta()
    {
        var value = _api.BlackScholes_Theta(callOption, Spot, Vol, r);
        Console.WriteLine($"Theta: {value}");
    }

    [Test]
    public void WhenCallingBSM_ImpliedVol()
    {
        double optionPrice = 2.5;
        var value = _api.BlackScholes_ImpliedVolatility(callOption, Spot, r, optionPrice);
        Console.WriteLine($"ImpliedVol: {value}");
    }

    [Test]
    public void WhenCallingMonteCarlo()
    {
        var value = _api.MonteCarlo_PV(callOption, Spot, Vol, r, 1000);
        Console.WriteLine($"PV: {value.PV}");
        Console.WriteLine($"PVPut: {value.PVPut}");

        // doesnt output rhos in debug log
        Console.WriteLine($"Rhos:");
        Console.WriteLine(value.Debug.rhos);
        Console.WriteLine($"Spots:");
        Console.WriteLine(value.Debug.spotGraph);
    }

    [Test]
    public void WhenCallingMonteCarlo_ImpliedVol()
    {
        double optionPrice = 2.5;
        var value = _api.MonteCarlo_ImpliedVolatility(callOption, Spot, r, 1000, optionPrice);
        Console.WriteLine($"ImpliedVol: {value}");       
    }
}