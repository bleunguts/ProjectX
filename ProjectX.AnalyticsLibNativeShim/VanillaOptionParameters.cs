namespace ProjectX.AnalyticsLibNativeShim;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
public struct VanillaOptionParameters
{
    public OptionType OptionType;
    public double Strike;
    public double Expiry;
}
