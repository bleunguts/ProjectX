namespace ShellX.Surface3D;

public readonly record struct Rgb(byte R, byte G, byte B);

/// <summary>The "Jet" colormap the WPF Chart3D used: dark blue, blue, cyan, yellow, red, dark red.</summary>
public static class JetColorMap
{
    public static Rgb Evaluate(double t)
    {
        t = Math.Clamp(t, 0, 1);
        return new Rgb(Channel(1.5 - Math.Abs(4 * t - 3)), Channel(1.5 - Math.Abs(4 * t - 2)), Channel(1.5 - Math.Abs(4 * t - 1)));
    }

    private static byte Channel(double value) => (byte)Math.Round(Math.Clamp(value, 0, 1) * 255, MidpointRounding.AwayFromZero);
}
