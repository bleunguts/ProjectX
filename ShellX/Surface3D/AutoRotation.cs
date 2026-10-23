namespace ShellX.Surface3D;

public static class AutoRotation
{
    public const double DefaultDegreesPerSecond = 20;

    /// <summary>The azimuth after turning for <paramref name="elapsedSeconds"/>, wrapped into -180 (inclusive) to 180 (exclusive).</summary>
    public static double Step(double azimuthDegrees, double elapsedSeconds, double degreesPerSecond = DefaultDegreesPerSecond)
    {
        double turned = azimuthDegrees + elapsedSeconds * degreesPerSecond;
        return ((turned + 180) % 360 + 360) % 360 - 180;
    }
}
