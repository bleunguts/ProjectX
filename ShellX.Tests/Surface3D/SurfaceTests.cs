using ShellX.OptionsPricing;
using ShellX.Surface3D;

namespace ShellX.Tests.Surface3D;

public class JetColorMapTests
{
    [Test]
    public void Runs_from_dark_blue_to_dark_red()
    {
        Assert.That(JetColorMap.Evaluate(0), Is.EqualTo(new Rgb(0, 0, 128)));
        Assert.That(JetColorMap.Evaluate(0.5), Is.EqualTo(new Rgb(128, 255, 128)));
        Assert.That(JetColorMap.Evaluate(1), Is.EqualTo(new Rgb(128, 0, 0)));
    }

    [Test]
    public void Passes_through_cyan_and_yellow()
    {
        Assert.That(JetColorMap.Evaluate(0.375), Is.EqualTo(new Rgb(0, 255, 255)));
        Assert.That(JetColorMap.Evaluate(0.625), Is.EqualTo(new Rgb(255, 255, 0)));
    }

    [Test]
    public void Out_of_range_values_are_clamped()
    {
        Assert.That(JetColorMap.Evaluate(-3), Is.EqualTo(JetColorMap.Evaluate(0)));
        Assert.That(JetColorMap.Evaluate(7), Is.EqualTo(JetColorMap.Evaluate(1)));
    }
}

public class SurfaceProjectionTests
{
    [Test]
    public void Front_on_view_maps_x_to_screen_x_z_to_screen_y_and_y_to_depth()
    {
        var p = new SurfaceProjection(AzimuthDegrees: 0, ElevationDegrees: 0).Project(0.3, 0.7, 0.2);

        Assert.That((p.X, p.Y, p.Depth), Is.EqualTo((0.3, 0.2, 0.7)).Within(1e-9));
    }

    [Test]
    public void Azimuth_of_90_turns_the_x_axis_into_depth()
    {
        var p = new SurfaceProjection(90, 0).Project(1, 0, 0);

        Assert.That(p.X, Is.EqualTo(0).Within(1e-9));
        Assert.That(p.Depth, Is.EqualTo(1).Within(1e-9));
    }

    [Test]
    public void Looking_straight_down_puts_far_points_higher_on_screen_and_high_z_closer()
    {
        var projection = new SurfaceProjection(0, 90);

        Assert.That(projection.Project(0, 1, 0).Y, Is.EqualTo(1).Within(1e-9));
        Assert.That(projection.Project(0, 0, 1).Depth, Is.EqualTo(-1).Within(1e-9));
    }

    [Test]
    public void Rotation_preserves_distance_from_the_vertical_axis()
    {
        var p = new SurfaceProjection(-50, 0).Project(1, 1, 0);

        Assert.That(Math.Sqrt(p.X * p.X + p.Depth * p.Depth), Is.EqualTo(Math.Sqrt(2)).Within(1e-9));
    }
}

public class SurfaceMeshTests
{
    // 2 columns (t) by 3 rows (spot) of z values; z = j so it rises with spot.
    private static HeatMapData Ramp() => new()
    {
        Points = [new(0, 0, 0), new(0, 1, 1), new(0, 2, 2), new(1, 0, 0), new(1, 1, 1), new(1, 2, 2)],
        XCount = 2,
        YCount = 3,
    };

    [Test]
    public void Grid_of_n_by_m_points_gives_n_minus_1_by_m_minus_1_quads()
    {
        var quads = SurfaceMesh.Build(Ramp(), new SurfaceProjection(0, 0));

        Assert.That(quads, Has.Count.EqualTo(2));
        Assert.That(quads.All(q => q.Corners.Length == 4), Is.True);
    }

    [Test]
    public void Quads_are_ordered_back_to_front()
    {
        var quads = SurfaceMesh.Build(Ramp(), new SurfaceProjection(0, 0));

        Assert.That(quads[0].Depth, Is.GreaterThan(quads[1].Depth));
    }

    [Test]
    public void Colour_follows_the_height_of_each_quad()
    {
        var quads = SurfaceMesh.Build(Ramp(), new SurfaceProjection(0, 0));

        // quads[0] is the far (high z) one: z averages 1.5 of 0..2 -> 0.75; the near one averages 0.5 -> 0.25.
        Assert.That(quads[0].Color, Is.EqualTo(JetColorMap.Evaluate(0.75)));
        Assert.That(quads[1].Color, Is.EqualTo(JetColorMap.Evaluate(0.25)));
    }

    [Test]
    public void Flat_surface_does_not_divide_by_zero()
    {
        var flat = new HeatMapData { Points = [new(0, 0, 5), new(0, 1, 5), new(1, 0, 5), new(1, 1, 5)], XCount = 2, YCount = 2 };

        var quads = SurfaceMesh.Build(flat, new SurfaceProjection(-50, 40));

        Assert.That(quads, Has.Count.EqualTo(1));
        Assert.That(double.IsFinite(quads[0].Corners[0].X), Is.True);
    }

    [Test]
    public void Grid_is_normalised_into_a_unit_box_so_it_fits_any_view()
    {
        var quads = SurfaceMesh.Build(Ramp(), new SurfaceProjection(0, 0));

        var xs = quads.SelectMany(q => q.Corners).Select(c => c.X);
        Assert.That((xs.Min(), xs.Max()), Is.EqualTo((-1d, 1d)));
    }

    [Test]
    public void Empty_or_single_row_data_yields_no_quads()
    {
        var single = new HeatMapData { Points = [new(0, 0, 1), new(0, 1, 2)], XCount = 1, YCount = 2 };

        Assert.That(SurfaceMesh.Build(single, new SurfaceProjection(0, 0)), Is.Empty);
    }

    [Test]
    public void Heat_map_data_reports_its_grid_size()
    {
        Assert.That((HeatMapData.Peak().XCount, HeatMapData.Peak().YCount), Is.EqualTo((31, 31)));
    }
}
