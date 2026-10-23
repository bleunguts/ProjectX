using ProjectX.Core;

namespace ShellX.OptionsPricing;

/// <summary>A cell of the heatmap: grid indexes (not t / spot values) and the surface value.</summary>
public record HeatMapPoint(int I, int J, double Z);

/// <summary>
/// The option price / Greeks surface flattened for a heatmap. X is maturity t, Y is spot, the value is the colour.
/// Cells are addressed by grid index; <see cref="XValue"/> and <see cref="YValue"/> map an index back to t and spot.
/// </summary>
public class HeatMapData
{
    public required IReadOnlyList<HeatMapPoint> Points { get; init; }
    public double ZMin { get; init; }
    public double ZMax { get; init; }
    public double ZTick { get; init; }
    public double XMin { get; init; }
    public double XSpacing { get; init; }
    public double YMin { get; init; }
    public double YSpacing { get; init; }

    public double XValue(double index) => XMin + index * XSpacing;
    public double YValue(double index) => YMin + index * YSpacing;

    public static HeatMapData FromPlot(PlotResults plot, int zDecimalPlaces, int zTickDecimalPlaces)
    {
        var points = new List<HeatMapPoint>();
        for (int i = 0; i < plot.PointArray.GetLength(0); i++)
            for (int j = 0; j < plot.PointArray.GetLength(1); j++)
                points.Add(new HeatMapPoint(i, j, plot.PointArray[i, j].z));

        return new HeatMapData
        {
            Points = points,
            ZMin = Math.Round(plot.zmin, zDecimalPlaces),
            ZMax = Math.Round(plot.zmax, zDecimalPlaces),
            ZTick = Math.Round((plot.zmax - plot.zmin) / 5.0, zTickDecimalPlaces),
            XMin = plot.XLimitMin,
            XSpacing = plot.XSpacing,
            YMin = plot.YLimitMin,
            YSpacing = plot.YSpacing,
        };
    }

    /// <summary>The "peaks" demo surface behind Plot Experimental.</summary>
    public static HeatMapData Peak()
    {
        const double min = -3, max = 3, spacing = 0.2;
        int number = (int)Math.Round((max - min) / spacing) + 1;

        var points = new List<HeatMapPoint>();
        for (int i = 0; i < number; i++)
        {
            for (int j = 0; j < number; j++)
            {
                double x = min + i * spacing;
                double y = min + j * spacing;
                double z = 3 * Math.Pow(1 - x, 2) * Math.Exp(-x * x - (y + 1) * (y + 1)) - 10 *
                    (0.2 * x - Math.Pow(x, 3) - Math.Pow(y, 5)) * Math.Exp(-x * x - y * y) - 1 / 3 *
                    Math.Exp(-(x + 1) * (x + 1) - y * y);
                points.Add(new HeatMapPoint(i, j, z));
            }
        }

        return new HeatMapData { Points = points, ZMin = -8, ZMax = 8, ZTick = 4, XMin = min, XSpacing = spacing, YMin = min, YSpacing = spacing };
    }
}
