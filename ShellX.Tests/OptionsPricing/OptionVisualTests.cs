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

public class GaugeScaleTests
{
    [Test]
    public void A_value_maps_to_its_position_within_the_range()
    {
        Assert.That(GaugeScale.Fraction(5, 0, 10), Is.EqualTo(0.5).Within(1e-9));
        Assert.That(GaugeScale.Fraction(2.5, 0, 10), Is.EqualTo(0.25).Within(1e-9));
    }

    [Test]
    public void Values_outside_the_range_are_clamped()
    {
        Assert.That(GaugeScale.Fraction(-3, 0, 10), Is.EqualTo(0));
        Assert.That(GaugeScale.Fraction(99, 0, 10), Is.EqualTo(1));
    }

    [Test]
    public void A_flat_range_sits_in_the_middle()
    {
        Assert.That(GaugeScale.Fraction(7, 7, 7), Is.EqualTo(0.5));
    }

    [Test]
    public void Negative_ranges_work_like_theta()
    {
        Assert.That(GaugeScale.Fraction(-15, -20, -10), Is.EqualTo(0.5).Within(1e-9));
    }

    [Test]
    public void The_range_is_padded_by_ten_percent_so_the_extremes_are_not_pegged()
    {
        var (min, max) = GaugeScale.Range(new[] { 0.0, 4.0, 10.0 });

        Assert.That(min, Is.EqualTo(-1).Within(1e-9));
        Assert.That(max, Is.EqualTo(11).Within(1e-9));
    }

    [Test]
    public void A_flat_series_still_gets_a_band_and_no_values_a_default()
    {
        var flat = GaugeScale.Range(new[] { 3.0, 3.0 });
        var none = GaugeScale.Range(Array.Empty<double>());

        Assert.That((flat.Min, flat.Max), Is.EqualTo((2d, 4d)));
        Assert.That((none.Min, none.Max), Is.EqualTo((0d, 1d)));
    }

    [Test]
    public void The_dial_sweeps_270_degrees()
    {
        Assert.That(GaugeScale.SweepDegrees(0), Is.EqualTo(0));
        Assert.That(GaugeScale.SweepDegrees(0.5), Is.EqualTo(135).Within(1e-9));
        Assert.That(GaugeScale.SweepDegrees(1), Is.EqualTo(270).Within(1e-9));
        Assert.That(GaugeScale.SweepDegrees(2), Is.EqualTo(270).Within(1e-9));
    }
}

public class GreekGaugeSetTests
{
    private static MaturityAndOptionGreeksResultPair Pair(double maturity, double delta, double theta) =>
        new(maturity, new OptionGreeksResult(1, delta, 0.01, theta, 1, 10));

    [Test]
    public void Each_greek_is_placed_within_its_own_range_across_all_maturities()
    {
        var results = new[] { Pair(0.5, 0.2, -10), Pair(1, 0.8, -20) };

        var gauges = GreekGaugeSet.From(results, selectedIndex: 0);

        var delta = gauges.Single(g => g.Name == "Delta");
        var theta = gauges.Single(g => g.Name == "Theta");
        Assert.That(delta.Value, Is.EqualTo(0.2));
        Assert.That(delta.Fraction, Is.LessThan(0.5), "0.2 is the low end of 0.2..0.8");
        Assert.That(theta.Value, Is.EqualTo(-10));
        Assert.That(theta.Fraction, Is.GreaterThan(0.5), "-10 is the high end of -20..-10");
    }

    [Test]
    public void The_set_always_lists_the_five_greeks_in_order()
    {
        var gauges = GreekGaugeSet.From(new[] { Pair(1, 0.5, -1) }, 0);

        Assert.That(gauges.Select(g => g.Name), Is.EqualTo(new[] { "Delta", "Gamma", "Theta", "Rho", "Vega" }));
    }

    [Test]
    public void No_results_gives_five_empty_mid_dials()
    {
        var gauges = GreekGaugeSet.From(Array.Empty<MaturityAndOptionGreeksResultPair>(), 0);

        Assert.That(gauges, Has.Count.EqualTo(5));
        Assert.That(gauges.All(g => g.Value == 0 && g.Fraction == 0.5), Is.True);
    }
}
