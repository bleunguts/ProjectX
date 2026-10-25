using System.Globalization;
using ProjectX.Core;
using ShellX.Fx;

namespace ShellX.Tests.Fx;

public class FxTickDirectionTests
{
    private static SpotPrice Price(decimal bid, decimal ask) => new("EURUSD", bid, ask);

    [Test]
    public void Mid_rising_is_up()
    {
        Assert.That(FxTickDirections.Between(Price(1.1000m, 1.1002m), Price(1.1001m, 1.1003m)), Is.EqualTo(FxTickDirection.Up));
    }

    [Test]
    public void Mid_falling_is_down()
    {
        Assert.That(FxTickDirections.Between(Price(1.1001m, 1.1003m), Price(1.1000m, 1.1002m)), Is.EqualTo(FxTickDirection.Down));
    }

    [Test]
    public void Unchanged_mid_is_flat_even_if_the_spread_moved()
    {
        Assert.That(FxTickDirections.Between(Price(1.1000m, 1.1002m), Price(1.0999m, 1.1003m)), Is.EqualTo(FxTickDirection.Flat));
    }

    [Test]
    public void First_tick_has_no_direction()
    {
        Assert.That(FxTickDirections.Between(null, Price(1.1m, 1.2m)), Is.EqualTo(FxTickDirection.Flat));
    }

    [TestCase(FxTickDirection.Up, "▲")]
    [TestCase(FxTickDirection.Down, "▼")]
    [TestCase(FxTickDirection.Flat, "")]
    public void Arrow_for_each_direction(FxTickDirection direction, string arrow)
    {
        Assert.That(FxTickDirections.Arrow(direction), Is.EqualTo(arrow));
    }
}

public class FxPriceDisplayTests
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
    public void Buy_and_sell_labels_carry_the_live_price()
    {
        var price = new SpotPrice("EURUSD", 1.08432m, 1.08441m);

        Assert.That(FxPriceFormatter.SellLabel(price), Is.EqualTo("Sell 1.08432"));
        Assert.That(FxPriceFormatter.BuyLabel(price), Is.EqualTo("Buy 1.08441"));
    }

    [Test]
    public void Labels_before_the_first_price_have_no_number()
    {
        Assert.That(FxPriceFormatter.SellLabel(null), Is.EqualTo("Sell"));
        Assert.That(FxPriceFormatter.BuyLabel(null), Is.EqualTo("Buy"));
    }

    [Test]
    public void Price_splits_into_small_prefix_big_figure_and_fraction()
    {
        var parts = FxPriceFormatter.Split(1.08432m);

        Assert.That((parts.Prefix, parts.Big, parts.Fraction), Is.EqualTo(("1.08", "43", "2")));
    }

    [Test]
    public void Split_keeps_trailing_zeros_to_five_decimals()
    {
        var parts = FxPriceFormatter.Split(1.2m);

        Assert.That((parts.Prefix, parts.Big, parts.Fraction), Is.EqualTo(("1.20", "00", "0")));
    }

    [Test]
    public void Split_uses_the_current_culture_decimal_separator()
    {
        CultureInfo.CurrentCulture = new CultureInfo("de-DE");

        Assert.That(FxPriceFormatter.Split(1.08432m).Prefix, Is.EqualTo("1,08"));
    }

    [Test]
    public void Empty_parts_for_no_price()
    {
        var parts = FxPriceFormatter.Split(null);

        Assert.That((parts.Prefix, parts.Big, parts.Fraction), Is.EqualTo(("--", "--", string.Empty)));
    }
}

public class FxQuantityTests
{
    [Test]
    public void Chips_are_one_ten_hundred_thousand_and_a_million()
    {
        Assert.That(FxQuantity.Chips.Select(c => (c.Label, c.Value)), Is.EqualTo(new[]
        {
            ("1k", 1_000), ("10k", 10_000), ("100k", 100_000), ("1m", 1_000_000),
        }));
    }
}

public class FxLivenessTests
{
    private static readonly DateTimeOffset Now = new(2026, 10, 25, 12, 0, 10, TimeSpan.Zero);

    [Test]
    public void A_recent_tick_is_live()
    {
        Assert.That(FxLiveness.IsLive(Now.AddSeconds(-2), Now), Is.True);
    }

    [Test]
    public void An_old_tick_is_not_live()
    {
        Assert.That(FxLiveness.IsLive(Now.AddSeconds(-4), Now), Is.False);
    }

    [Test]
    public void No_tick_yet_is_not_live()
    {
        Assert.That(FxLiveness.IsLive(null, Now), Is.False);
    }

    [Test]
    public void The_window_can_be_changed()
    {
        Assert.That(FxLiveness.IsLive(Now.AddSeconds(-4), Now, TimeSpan.FromSeconds(5)), Is.True);
    }
}

public class PnlSignTests
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

    [TestCase("1,234", 1)]
    [TestCase("-2,001", -1)]
    [TestCase("0", 0)]
    [TestCase("", 0)]
    [TestCase("total", 0)]
    public void Sign_of_a_formatted_pnl_cell(string text, int expected)
    {
        Assert.That(PnlSign.Of(text), Is.EqualTo(expected));
    }
}

public class FxChartRangeTests
{
    [Test]
    public void Range_is_padded_by_ten_percent_each_side()
    {
        var range = FxChartRange.Padded(new[] { 1.00, 1.05, 1.10 });

        Assert.That(range!.Value.Min, Is.EqualTo(0.99).Within(1e-9));
        Assert.That(range.Value.Max, Is.EqualTo(1.11).Within(1e-9));
    }

    [Test]
    public void A_flat_series_still_gets_a_visible_band()
    {
        var range = FxChartRange.Padded(new[] { 1.1, 1.1 });

        Assert.That(range!.Value.Max - range.Value.Min, Is.GreaterThan(0));
        Assert.That(range.Value.Min, Is.LessThan(1.1));
    }

    [Test]
    public void No_values_means_no_range()
    {
        Assert.That(FxChartRange.Padded(Array.Empty<double>()), Is.Null);
    }
}

public class FxPositionsTotalTests
{
    [Test]
    public void Total_is_available_as_a_number_for_colour_and_count_up()
    {
        var builder = new FxPositionsTableBuilder();
        builder.SetRows(new Dictionary<string, (int netQuantity, int totalTrades, decimal PnL, string debug)>
        {
            ["EURUSD"] = (1, 1, 0.6m, string.Empty),
            ["GBPUSD"] = (1, 1, -3.4m, string.Empty),
        });

        // 0.6 rounds to 1, -3.4 rounds to -3: the screen adds what it shows.
        Assert.That(builder.Total, Is.EqualTo(-2m));
    }

    [Test]
    public void Total_starts_at_zero()
    {
        Assert.That(new FxPositionsTableBuilder().Total, Is.Zero);
    }
}

public class SpreadLadderScaleTests
{
    [Test]
    public void The_centre_price_sits_in_the_middle_of_the_ladder()
    {
        var scale = new SpreadLadderScale(Center: 1.1000, Span: 0.0006);

        Assert.That(scale.YFraction(1.1000), Is.EqualTo(0.5).Within(1e-9));
    }

    [Test]
    public void Higher_prices_are_nearer_the_top()
    {
        var scale = new SpreadLadderScale(1.1000, 0.0006);

        Assert.That(scale.YFraction(1.1003), Is.EqualTo(0).Within(1e-9));
        Assert.That(scale.YFraction(1.0997), Is.EqualTo(1).Within(1e-9));
    }

    [Test]
    public void Rungs_are_evenly_spaced_around_the_centre_and_listed_top_down()
    {
        var scale = new SpreadLadderScale(1.1000, 0.0006);

        var rungs = scale.Rungs(step: 0.0001);

        Assert.That(rungs, Is.EqualTo(new[] { 1.1003, 1.1002, 1.1001, 1.1000, 1.0999, 1.0998, 1.0997 }).Within(1e-9));
    }

    [Test]
    public void Rungs_stay_on_round_steps_when_the_centre_drifts()
    {
        var scale = new SpreadLadderScale(1.100037, 0.0006);

        var rungs = scale.Rungs(0.0001);

        Assert.That(rungs.All(r => Math.Abs(r / 0.0001 - Math.Round(r / 0.0001)) < 1e-6), Is.True);
        Assert.That(rungs.All(r => scale.YFraction(r) is >= 0 and <= 1), Is.True);
    }
}

public class CountUpTests
{
    [Test]
    public void Starts_and_ends_exactly_on_the_values()
    {
        Assert.That(CountUp.Value(100, 500, 0), Is.EqualTo(100));
        Assert.That(CountUp.Value(100, 500, 1), Is.EqualTo(500));
    }

    [Test]
    public void Moves_monotonically_and_eases_out()
    {
        var quarter = CountUp.Value(0, 100, 0.25);
        var half = CountUp.Value(0, 100, 0.5);

        Assert.That(quarter, Is.GreaterThan(0));
        Assert.That(half, Is.GreaterThan(quarter));
        Assert.That(half, Is.GreaterThan(50), "ease-out is past the halfway value at half time");
    }

    [Test]
    public void Works_downwards_and_clamps_time()
    {
        Assert.That(CountUp.Value(500, -100, 1), Is.EqualTo(-100));
        Assert.That(CountUp.Value(0, 10, 3), Is.EqualTo(10));
        Assert.That(CountUp.Value(0, 10, -1), Is.EqualTo(0));
    }
}
