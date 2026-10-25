using System.Globalization;
using ProjectX.Core;
using ShellX.Fx;

namespace ShellX.Tests.Fx;

public class FxTickBufferTests
{
    private static FxTick Tick(int n) => new(new DateTimeOffset(2026, 10, 25, 9, 0, n, TimeSpan.Zero), new SpotPrice("EURUSD", 1.1m + n / 1000m, 1.2m));

    [Test]
    public void Newest_first_for_the_list_and_oldest_first_for_the_chart()
    {
        var buffer = new FxTickBuffer();
        buffer.Add(Tick(1));
        buffer.Add(Tick(2));
        buffer.Add(Tick(3));

        Assert.That(buffer.NewestFirst.Select(t => t.Timestamp.Second), Is.EqualTo(new[] { 3, 2, 1 }));
        Assert.That(buffer.OldestFirst.Select(t => t.Timestamp.Second), Is.EqualTo(new[] { 1, 2, 3 }));
    }

    [Test]
    public void Drops_the_oldest_once_over_capacity()
    {
        var buffer = new FxTickBuffer(capacity: 3);
        for (int i = 1; i <= 5; i++)
            buffer.Add(Tick(i));

        Assert.That(buffer.Count, Is.EqualTo(3));
        Assert.That(buffer.OldestFirst.Select(t => t.Timestamp.Second), Is.EqualTo(new[] { 3, 4, 5 }));
    }

    [Test]
    public void Default_capacity_is_200()
    {
        var buffer = new FxTickBuffer();
        for (int i = 0; i < 250; i++)
            buffer.Add(Tick(i % 60));

        Assert.That(buffer.Count, Is.EqualTo(200));
    }

    [Test]
    public void Clear_empties_it()
    {
        var buffer = new FxTickBuffer();
        buffer.Add(Tick(1));

        buffer.Clear();

        Assert.That(buffer.Count, Is.Zero);
        Assert.That(buffer.NewestFirst, Is.Empty);
    }
}

public class FxPriceFormatterTests
{
    [Test]
    public void Latest_price_matches_the_wpf_bid_ask_text()
    {
        var price = new SpotPrice("EURUSD", 1.08432m, 1.08441m);

        Assert.That(FxPriceFormatter.LatestPrice(price), Is.EqualTo(price.ToPrettifiedBidAskPrice()));
        Assert.That(FxPriceFormatter.LatestPrice(price), Is.EqualTo("1.08432/1.08441"));
    }

    [Test]
    public void Spread_text_wraps_the_core_spread_calculation()
    {
        var price = new SpotPrice("EURUSD", 1.08432m, 1.08441m);

        Assert.That(FxPriceFormatter.SpreadText(price), Is.EqualTo($"({price.Spread()} pips spread)"));
    }

    [Test]
    public void Selected_pair_filter_ignores_other_pairs()
    {
        var eurusd = new SpotPrice("EURUSD", 1m, 1m);

        Assert.That(FxPriceFormatter.IsForPair(eurusd, "EURUSD"), Is.True);
        Assert.That(FxPriceFormatter.IsForPair(eurusd, "GBPUSD"), Is.False);
    }
}

public class FxPositionsTableBuilderTests
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

    private static Dictionary<string, (int netQuantity, int totalTrades, decimal PnL, string debug)> Positions(params (string pair, int qty, int trades, decimal pnl)[] rows) =>
        rows.ToDictionary(r => r.pair, r => (r.qty, r.trades, r.pnl, $"{r.trades} trades"));

    [Test]
    public void Rows_hold_pair_net_quantity_rounded_pnl_and_breakdown_then_a_total_row()
    {
        var builder = new FxPositionsTableBuilder();
        builder.SetRows(Positions(("EURUSD", 1000, 2, 1234.4m), ("GBPUSD", -500, 1, -2000.6m)));

        var table = builder.Build();

        Assert.That(table.Headers, Is.EqualTo(new[] { "CcyPair", "Notional", "PnL", "Breakdown" }));
        Assert.That(table.Rows[0], Is.EqualTo(new[] { "EURUSD", "1000", "1,234", "2 trades" }));
        Assert.That(table.Rows[1], Is.EqualTo(new[] { "GBPUSD", "-500", "-2,001", "1 trades" }));
        Assert.That(table.Rows[2][0], Is.EqualTo("total"));
        Assert.That(table.Rows[2][2], Is.EqualTo("-767"));
    }

    [Test]
    public void Total_adds_the_rounded_figures_shown_on_screen_like_wpf()
    {
        var builder = new FxPositionsTableBuilder();
        builder.SetRows(Positions(("EURUSD", 1, 1, 0.6m), ("GBPUSD", 1, 1, 0.6m)));

        // 0.6 + 0.6 = 1.2 would round to 1; the screen shows 1 + 1, so WPF totals 2.
        Assert.That(builder.Build().Rows.Last()[2], Is.EqualTo("2"));
    }

    [Test]
    public void No_positions_still_gives_a_zero_total_row()
    {
        var table = new FxPositionsTableBuilder().Build();

        Assert.That(table.Rows, Has.Count.EqualTo(1));
        Assert.That(table.Rows[0][0], Is.EqualTo("total"));
        Assert.That(table.Rows[0][2], Is.EqualTo("0"));
    }

    [Test]
    public void Thousands_separators_are_formatted_not_parsed()
    {
        var builder = new FxPositionsTableBuilder();
        builder.SetRows(Positions(("EURUSD", 1, 1, 1234567m)));

        var table = builder.Build();

        Assert.That(table.Rows[0][2], Is.EqualTo("1,234,567"));
        Assert.That(table.Rows[1][2], Is.EqualTo("1,234,567"));
    }
}
