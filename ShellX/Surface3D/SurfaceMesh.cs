using ShellX.OptionsPricing;

namespace ShellX.Surface3D;

/// <summary>One cell of the surface: four projected corners, how far away it is, and its colour.</summary>
public record SurfaceQuad(Point2[] Corners, double Depth, Rgb Color);

public static class SurfaceMesh
{
    /// <summary>The world-space box the surface is normalised into: t and spot span -1..1, height spans -0.5..0.5.</summary>
    public const double HalfWidth = 1;
    public const double HalfHeight = 0.5;

    /// <summary>Quads for the surface, ordered back to front so they can be painted in sequence.</summary>
    public static IReadOnlyList<SurfaceQuad> Build(HeatMapData data, SurfaceProjection projection)
    {
        int columns = data.XCount, rows = data.YCount;
        if (columns < 2 || rows < 2 || data.Points.Count < columns * rows)
            return [];

        var z = new double[columns, rows];
        foreach (var point in data.Points)
            z[point.I, point.J] = point.Z;

        double min = data.Points.Min(p => p.Z), max = data.Points.Max(p => p.Z);
        double range = max - min;
        double Normalised(double value) => range > 0 ? (value - min) / range : 0.5;

        ProjectedPoint Corner(int i, int j) => projection.Project(
            -HalfWidth + 2 * HalfWidth * i / (columns - 1),
            -HalfWidth + 2 * HalfWidth * j / (rows - 1),
            (Normalised(z[i, j]) - 0.5) * 2 * HalfHeight);

        var quads = new List<SurfaceQuad>((columns - 1) * (rows - 1));
        for (int i = 0; i < columns - 1; i++)
        {
            for (int j = 0; j < rows - 1; j++)
            {
                var corners = new[] { Corner(i, j), Corner(i + 1, j), Corner(i + 1, j + 1), Corner(i, j + 1) };
                double height = (z[i, j] + z[i + 1, j] + z[i + 1, j + 1] + z[i, j + 1]) / 4;

                quads.Add(new SurfaceQuad(
                    corners.Select(c => new Point2(c.X, c.Y)).ToArray(),
                    corners.Average(c => c.Depth),
                    JetColorMap.Evaluate(Normalised(height))));
            }
        }

        return quads.OrderByDescending(q => q.Depth).ToList();
    }
}
