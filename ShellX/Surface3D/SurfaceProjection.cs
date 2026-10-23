namespace ShellX.Surface3D;

public readonly record struct Point2(double X, double Y);

public readonly record struct ProjectedPoint(double X, double Y, double Depth);

/// <summary>
/// Orthographic view of a z-up world. Azimuth turns the scene about the vertical axis, elevation tilts the
/// camera up from the horizon (90 looks straight down). Larger <see cref="ProjectedPoint.Depth"/> is further away;
/// screen Y grows upwards.
/// </summary>
public readonly record struct SurfaceProjection(double AzimuthDegrees, double ElevationDegrees)
{
    public ProjectedPoint Project(double x, double y, double z)
    {
        double azimuth = AzimuthDegrees * Math.PI / 180;
        double elevation = ElevationDegrees * Math.PI / 180;

        double across = x * Math.Cos(azimuth) - y * Math.Sin(azimuth);
        double away = x * Math.Sin(azimuth) + y * Math.Cos(azimuth);

        return new ProjectedPoint(
            across,
            z * Math.Cos(elevation) + away * Math.Sin(elevation),
            away * Math.Cos(elevation) - z * Math.Sin(elevation));
    }
}
