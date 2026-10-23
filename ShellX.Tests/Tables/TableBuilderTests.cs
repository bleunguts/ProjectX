using System.Globalization;
using ProjectX.Core;
using ProjectX.Core.Strategy;
using ProjectX.MachineLearning;
using ShellX.Tables;

namespace ShellX.Tests.Tables;

public class TableBuilderTests
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
    public void PnlRanking_sorts_by_pnl_descending_and_formats_values()
    {
        var builder = new PnlRankingTableBuilder();
        builder.SetRows(new[]
        {
            new MatrixStrategyPnl("AAPL", 10, 1.5, 0.5, 4, 1200, 0.5),
            new MatrixStrategyPnl("AAPL", 20, 1, 0, 8, 12000, 1.23456),
            new MatrixStrategyPnl("AAPL", 30, 2, 1, 2, -300, -0.1),
        });

        var table = builder.Build();

        Assert.That(table.Headers, Is.EqualTo(new[] { "Ticker", "Window", "SignalIn", "SignalOut", "Num", "PnL", "Sharpe" }));
        Assert.That(table.Rows.Select(r => r[5]), Is.EqualTo(new[] { "$12,000", "$1,200", "$-300" }));
        Assert.That(table.Rows[0], Is.EqualTo(new[] { "AAPL", "20", "1", "0", "8", "$12,000", "1.235" }));
    }

    [Test]
    public void YearlyPnL_formats_pnl_and_sharpe()
    {
        var builder = new YearlyPnLTableBuilder();
        builder.SetRows(new[] { new YearlyStrategyPnl("MSFT", "2023", 5, 1234.6, 0.98765, -50, 0.1) });

        var table = builder.Build();

        Assert.That(table.Headers, Is.EqualTo(new[] { "Ticker", "Period", "Num", "PnL", "Sharpe", "PnLHold", "SharpeHold" }));
        Assert.That(table.Rows.Single(), Is.EqualTo(new[] { "MSFT", "2023", "5", "$1,235", "0.988", "$-50", "0.100" }));
    }

    [Test]
    public void PnL_maps_position_state_and_blank_entry_fields()
    {
        var builder = new PnLTableBuilder();
        builder.SetRows(new[]
        {
            new StrategyPnl(new DateTime(2023, 3, 1), "AAPL", 150.5, 1.5, 100, 10, 5, 10, 100, PositionStatus.POSITION_LONG, new DateTime(2023, 2, 20), 148.25, 3),
            new StrategyPnl(new DateTime(2023, 3, 2), "AAPL", 151, 0, 100, 0, 0, 0, 100, PositionStatus.POSITION_NONE, null, null, 3),
        });

        var table = builder.Build();

        Assert.That(table.Headers, Has.Count.EqualTo(13));
        Assert.That(table.Rows[0], Is.EqualTo(new[] { "010323", "AAPL", "150.5", "1.500", "$100", "$10", "$5", "$10", "$100", "LONG", "200223", "148.25", "3" }));
        Assert.That(table.Rows[1][9], Is.EqualTo(string.Empty));
        Assert.That(table.Rows[1][10], Is.EqualTo(string.Empty));
        Assert.That(table.Rows[1][11], Is.EqualTo(string.Empty));
    }

    [Test]
    public void Stock_rows_start_with_unset_expected_and_predicted()
    {
        var builder = new StockTableBuilder();
        builder.SetRows(new[] { new MarketPrice { Date = new DateTime(2023, 3, 1), Open = 1.5m, High = 2m, Low = 1m, Close = 1.75m } });

        var table = builder.Build();

        Assert.That(table.Headers, Is.EqualTo(new[] { "Date", "Open", "High", "Low", "Close", "Expected", "Predicted" }));
        Assert.That(table.Rows.Single(), Is.EqualTo(new[] { "010323", "1.5", "2", "1", "1.75", nameof(StockPriceTrendDirection.Unset), nameof(StockPriceTrendDirection.Unset) }));
    }

    [Test]
    public void Empty_builder_yields_headers_and_no_rows()
    {
        var table = new PnlRankingTableBuilder().Build();

        Assert.That(table.Headers, Is.Not.Empty);
        Assert.That(table.Rows, Is.Empty);
    }
}
