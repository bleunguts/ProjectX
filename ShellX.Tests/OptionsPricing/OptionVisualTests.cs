using System.Globalization;
using ProjectX.Core;
using ProjectX.Core.Requests;
using ShellX.OptionsPricing;

namespace ShellX.Tests.OptionsPricing;

public class OptionFigureTests
{
    private CultureInfo _original = CultureInfo.CurrentCulture;

    [SetUp]
    public void SetUp()
    {
        _original = CultureInfo.CurrentCulture;
        CultureInfo.CurrentCulture = new CultureInfo("en-US");
    }

    [TearDown]
    public void TearDown() => CultureInfo.CurrentCulture = _original;

    [Test]
    public void Whole_part_and_two_decimals_are_big_the_rest_small()
    {
        var figure = OptionFigure.Split(12.345678);

        Assert.That((figure.Big, figure.Small), Is.EqualTo(("12.34", "57")));
    }

    [Test]
    public void Small_digits_keep_their_leading_and_trailing_zeros()
    {
        var figure = OptionFigure.Split(7.1);

        Assert.That((figure.Big, figure.Small), Is.EqualTo(("7.10", "00")));
    }

    [Test]
    public void Negative_values_keep_the_sign_on_the_big_part()
    {
        var figure = OptionFigure.Split(-3.14159);

        Assert.That((figure.Big, figure.Small), Is.EqualTo(("-3.14", "16")));
    }

    [Test]
    public void Rounding_can_carry_into_the_big_part()
    {
        var figure = OptionFigure.Split(9.99996);

        Assert.That((figure.Big, figure.Small), Is.EqualTo(("10.00", "00")));
    }

    [Test]
    public void Not_a_number_shows_dashes()
    {
        var figure = OptionFigure.Split(double.NaN);

        Assert.That((figure.Big, figure.Small), Is.EqualTo(("--", string.Empty)));
    }

    [Test]
    public void Uses_the_current_culture_decimal_separator()
    {
        CultureInfo.CurrentCulture = new CultureInfo("de-DE");

        Assert.That(OptionFigure.Split(1.5).Big, Is.EqualTo("1,50"));
    }
}

public class CalculatorDescriptionTests
{
    [TestCase(OptionsPricingCalculatorType.OptionsPricer, "Vanilla BlackScholes Options Pricer C#")]
    [TestCase(OptionsPricingCalculatorType.OptionsPricerCpp, "BlackScholes Options Pricer native C++")]
    [TestCase(OptionsPricingCalculatorType.MonteCarloCppPricer, "MonteCarlo Pricing native C++ (1e8 MC Paths)")]
    public void Each_calculator_has_a_description(OptionsPricingCalculatorType type, string expected)
    {
        Assert.That(CalculatorCycle.Describe(type), Is.EqualTo(expected));
    }

    [Test]
    public void Describe_agrees_with_what_the_toggle_reports_when_it_lands_on_a_calculator()
    {
        foreach (var type in Enum.GetValues<OptionsPricingCalculatorType>())
        {
            var (next, description) = CalculatorCycle.Next(type);
            Assert.That(CalculatorCycle.Describe(next), Is.EqualTo(description));
        }
    }
}

public class SelectedMaturityTests
{
    private static MaturityAndOptionGreeksResultPair Pair(double maturity) =>
        new(maturity, new OptionGreeksResult(maturity * 10, maturity, maturity / 10, -maturity, maturity * 2, maturity * 3));

    [Test]
    public void The_selected_index_picks_that_maturity()
    {
        var results = new[] { Pair(0.5), Pair(1), Pair(1.5) };

        Assert.That(SelectedMaturity.Pick(results, 1)!.Maturity, Is.EqualTo(1));
    }

    [Test]
    public void An_index_outside_the_results_is_clamped_to_the_nearest_row()
    {
        var results = new[] { Pair(0.5), Pair(1), Pair(1.5) };

        Assert.That(SelectedMaturity.Pick(results, 99)!.Maturity, Is.EqualTo(1.5));
        Assert.That(SelectedMaturity.Pick(results, -4)!.Maturity, Is.EqualTo(0.5));
    }

    [Test]
    public void No_results_means_nothing_selected()
    {
        Assert.That(SelectedMaturity.Pick(Array.Empty<MaturityAndOptionGreeksResultPair>(), 0), Is.Null);
    }
}
