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
    public void foo()
    {
        _api.Execute();
    }
}