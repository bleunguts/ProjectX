namespace ProjectX.AnalyticsLibNativeShim.Interop;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
public struct Debug
{
    public int callsCount;
    public int putsCount;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 10000)] public string rhos;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 10000)] public string spotGraph;
    public int totalSimulations;
}
