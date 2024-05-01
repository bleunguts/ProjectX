using Moq;
using Newtonsoft.Json;
using ProjectX.AnalyticsLib.Shared;
using ProjectX.Core;
using ProjectX.Core.Requests;
using ProjectX.Core.Services;

namespace ProjectX.AnalyticsLib.Tests;

public class OptionsPricingModelTest
{
    private Mock<IBlackScholesCSharpPricer> _mockCSharpCalculator = new Mock<IBlackScholesCSharpPricer>();
    private OptionsPricingModel _sut;
    private Random _random = new Random();

    [SetUp]
    public void SetUp()
    {
        _sut = new OptionsPricingModel(_mockCSharpCalculator.Object, Mock.Of<IBlackScholesCppPricer>(), Mock.Of<IMonteCarloCppOptionsPricer>());
        _mockCSharpCalculator.Setup(m => m.PV(It.IsAny<OptionType>(), It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>()))
                             .Returns(() => RandomFloat(50, 60));
    }

    private double RandomFloat(int min, int max) => _random.Next(min, max) + _random.NextDouble();

    [Test]
    public async Task WhenPricingOptionsPricingByMaturitiesRequestItShouldReturnValidValues()
    {
        var request = new OptionsPricingByMaturitiesRequest(10, OptionType.Call, 100.0, 150.0, 1.0, 1.0, 0.3, OptionsPricingCalculatorType.OptionsPricer);

        Console.WriteLine($"JSON={JsonConvert.SerializeObject(request)}");
        var actual = _sut.Price(request);
        Assert.That(actual, Is.Not.Null);
        Assert.That(actual.ResultsCount, Is.EqualTo(10));
        Assert.That(actual[0].Maturity, Is.EqualTo(0.1));
        Assert.That(actual[0].OptionGreeks.price, Is.Not.EqualTo(0));
    }

    [Test]
    public void WhenPlottingGreeksZValuesAreValid()
    {
        var plotOptionsResult = _sut.PlotGreeks(new PlotOptionsPricingRequest(OptionGreeks.Price, OptionType.Call, 100, 0.1, 0.04, 0.3, OptionsPricingCalculatorType.OptionsPricer));
        var result = plotOptionsResult.PlotResults;
        AssertZValue(result.zmin, result.zmax, rounding: 1);
    }

    private static void AssertZValue(double zmin, double zmax, int rounding)
    {
        var theZmin = Math.Round(zmin, rounding);
        var theZmax = Math.Round(zmax, rounding);
        var theZTick = Math.Round((zmax - zmin) / 5.0, rounding);

        Assert.IsTrue(zmin < zmax);
        Assert.IsTrue(theZTick > 0);
    }
}
