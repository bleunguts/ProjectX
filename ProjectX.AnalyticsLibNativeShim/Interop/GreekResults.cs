namespace ProjectX.AnalyticsLibNativeShim.Interop;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
public struct GreekResults
{
    public double PV;
    public double PVPut;
    public double Delta;
    public double DeltaPut;
    public double Gamma;
    public double Vega;
    public double Rho;
    public double RhoPut;
    public double Theta;
    public double ThetaPut;
    public Debug Debug;
}
