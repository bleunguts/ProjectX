using System.Globalization;
using ProjectX.Core;
using ProjectX.Core.Requests;
using ShellX.OptionsPricing;

namespace ShellX.Tests.OptionsPricing;

public class OptionInputsTests
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
    public void Defaults_match_the_wpf_screen()
    {
        var inputs = new OptionInputs();

        Assert.That((inputs.OptionType, inputs.Spot, inputs.Strike, inputs.Rate, inputs.Carry, inputs.Vol),
            Is.EqualTo(("Call", "100", "100", "0.1", "0.04", "0.3")));
    }

    [Test]
    public void Pricing_request_carries_parsed_values_and_calculator()
    {
        var inputs = new OptionInputs("Put", "90", "95", "0.05", "0.02", "0.25");

        var request = inputs.ToPricingRequest(OptionsPricingCalculatorType.OptionsPricerCpp);

        Assert.That(request.TimeSlices, Is.EqualTo(10));
        Assert.That(request.OptionType, Is.EqualTo(OptionType.Put));
        Assert.That((request.Spot, request.Strike, request.Rate, request.Carry, request.Vol), Is.EqualTo((90d, 95d, 0.05, 0.02, 0.25)));
        Assert.That(request.CalculatorType, Is.EqualTo(OptionsPricingCalculatorType.OptionsPricerCpp));
    }

    [Test]
    public void Plot_request_carries_greek_labels_and_decimal_places()
    {
        var request = new OptionInputs().ToPlotRequest(OptionGreeks.Gamma, "Gamma", 2, 3, OptionsPricingCalculatorType.OptionsPricer);

        Assert.That(request.GreekType, Is.EqualTo(OptionGreeks.Gamma));
        Assert.That((request.ZLabel, request.ZDecimalPlaces, request.ZTickDecimalPlaces), Is.EqualTo(("Gamma", 2, 3)));
        Assert.That(request.Strike, Is.EqualTo(100d));
    }

    [Test]
    public void Option_type_is_case_insensitive()
    {
        Assert.That(new OptionInputs("cALL").ToPricingRequest(OptionsPricingCalculatorType.OptionsPricer).OptionType, Is.EqualTo(OptionType.Call));
    }

    [TestCase("Spot", "abc")]
    [TestCase("Vol", "")]
    public void Unparseable_number_names_the_offending_parameter(string parameter, string value)
    {
        var inputs = parameter == "Spot" ? new OptionInputs(Spot: value) : new OptionInputs(Vol: value);

        var ex = Assert.Throws<FormatException>(() => inputs.ToPricingRequest(OptionsPricingCalculatorType.OptionsPricer));

        Assert.That(ex!.Message, Does.Contain(parameter));
    }

    [Test]
    public void Unknown_option_type_is_rejected()
    {
        Assert.That(() => new OptionInputs("Straddle").ToPricingRequest(OptionsPricingCalculatorType.OptionsPricer), Throws.InstanceOf<NotSupportedException>());
    }
}

public class CalculatorCycleTests
{
    [Test]
    public void Cycles_through_the_three_calculators_and_back()
    {
        var (second, _) = CalculatorCycle.Next(OptionsPricingCalculatorType.OptionsPricer);
        var (third, _) = CalculatorCycle.Next(second);
        var (first, _) = CalculatorCycle.Next(third);

        Assert.That(new[] { second, third, first }, Is.EqualTo(new[]
        {
            OptionsPricingCalculatorType.OptionsPricerCpp,
            OptionsPricingCalculatorType.MonteCarloCppPricer,
            OptionsPricingCalculatorType.OptionsPricer,
        }));
    }

    [Test]
    public void Each_step_describes_the_calculator_it_moves_to()
    {
        Assert.That(CalculatorCycle.Next(OptionsPricingCalculatorType.OptionsPricer).Description, Is.EqualTo("BlackScholes Options Pricer native C++"));
        Assert.That(CalculatorCycle.Next(OptionsPricingCalculatorType.OptionsPricerCpp).Description, Is.EqualTo("MonteCarlo Pricing native C++ (1e8 MC Paths)"));
        Assert.That(CalculatorCycle.Next(OptionsPricingCalculatorType.MonteCarloCppPricer).Description, Is.EqualTo("Vanilla BlackScholes Options Pricer C#"));
    }
}

public class OptionResultsTableBuilderTests
{
    [Test]
    public void Rows_hold_maturity_then_price_and_greeks_in_header_order()
    {
        var builder = new OptionResultsTableBuilder();
        builder.SetRows(new[]
        {
            new MaturityAndOptionGreeksResultPair(0.5, new OptionGreeksResult(price: 10.25, delta: 0.5, gamma: 0.02, theta: -3, rho: 4, vega: 20)),
            new MaturityAndOptionGreeksResultPair(1, new OptionGreeksResult(12.5, 0.6, 0.01, -2.5, 5, 25)),
        });

        var table = builder.Build();

        Assert.That(table.Headers, Is.EqualTo(new[] { "Maturity", "Price", "Delta", "Gamma", "Theta", "Rho", "Vega" }));
        Assert.That(table.Rows[0], Is.EqualTo(new[] { "0.5", "10.25", "0.5", "0.02", "-3", "4", "20" }));
        Assert.That(table.Rows, Has.Count.EqualTo(2));
    }

    [Test]
    public void Empty_builder_has_headers_and_no_rows()
    {
        var table = new OptionResultsTableBuilder().Build();

        Assert.That(table.Headers, Has.Count.EqualTo(7));
        Assert.That(table.Rows, Is.Empty);
    }
}

public class HeatMapDataTests
{
    private static PlotResults Plot() => new()
    {
        PointArray = new[,]
        {
            { new MyPoint3D(0.1, 10, 1.234), new MyPoint3D(0.1, 20, 2.0) },
            { new MyPoint3D(0.6, 10, 3.0), new MyPoint3D(0.6, 20, 4.5678) },
        },
        zmin = 1.234,
        zmax = 4.5678,
        XLimitMin = 0.1,
        XSpacing = 0.5,
        XNumber = 2,
        YLimitMin = 10,
        YSpacing = 10,
        YNumber = 2,
    };

    [Test]
    public void Flattens_the_surface_to_cell_indexes_with_z_as_value()
    {
        var data = HeatMapData.FromPlot(Plot(), zDecimalPlaces: 1, zTickDecimalPlaces: 1);

        Assert.That(data.Points, Is.EquivalentTo(new[]
        {
            new HeatMapPoint(0, 0, 1.234), new HeatMapPoint(0, 1, 2.0),
            new HeatMapPoint(1, 0, 3.0), new HeatMapPoint(1, 1, 4.5678),
        }));
    }

    [Test]
    public void Z_range_and_tick_are_rounded_like_the_wpf_screen()
    {
        var data = HeatMapData.FromPlot(Plot(), zDecimalPlaces: 1, zTickDecimalPlaces: 2);

        Assert.That((data.ZMin, data.ZMax), Is.EqualTo((1.2, 4.6)));
        Assert.That(data.ZTick, Is.EqualTo(Math.Round((4.5678 - 1.234) / 5.0, 2)));
    }

    [Test]
    public void Axis_mapping_converts_cell_index_back_to_t_and_spot()
    {
        var data = HeatMapData.FromPlot(Plot(), 1, 1);

        Assert.That(data.XValue(1), Is.EqualTo(0.6).Within(1e-9));
        Assert.That(data.YValue(1), Is.EqualTo(20).Within(1e-9));
    }

    [Test]
    public void Experimental_peak_surface_spans_minus_three_to_three()
    {
        var data = HeatMapData.Peak();

        Assert.That(data.Points, Has.Count.EqualTo(31 * 31));
        Assert.That(data.XValue(0), Is.EqualTo(-3).Within(1e-9));
        Assert.That(data.YValue(30), Is.EqualTo(3).Within(1e-9));
        Assert.That((data.ZMin, data.ZMax, data.ZTick), Is.EqualTo((-8d, 8d, 4d)));
    }
}

public class OptionResultsFormattingTests
{
    [Test]
    public void Long_doubles_are_shown_to_six_significant_digits()
    {
        var builder = new OptionResultsTableBuilder();
        builder.SetRows(new[] { new MaturityAndOptionGreeksResultPair(0.123456789, new OptionGreeksResult(10.987654321, 0.5, 0.0123456789, -3.14159265, 4, 20)) });

        Assert.That(builder.Build().Rows.Single(), Is.EqualTo(new[] { "0.123457", "10.9877", "0.5", "0.0123457", "-3.14159", "4", "20" }));
    }
}
