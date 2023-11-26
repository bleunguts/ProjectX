namespace ProjectX.AnalyticsLibNativeShim;

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

public enum OptionType { Call, Put };

[StructLayout(LayoutKind.Sequential)]
public struct VanillaOptionParameters
{
    public OptionType OptionType;
    public double Strike;
    public double Expiry;
}
[StructLayout(LayoutKind.Sequential, CharSet=CharSet.Ansi)]
public struct Debug
{
    public int callsCount;
    public int putsCount;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 10000)] public string rhos;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 10000)] public string spotGraph;
    public int totalSimulations;
}

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

// https://mark-borg.github.io/blog/2017/interop/#:~:text=Platform%20Invocation%20(PInvoke%20for%20short,from%20within%20a%20C%23%20program.
// https://stackoverflow.com/questions/315051/using-a-class-defined-in-a-c-dll-in-c-sharp-code
public class API
{    
    [DllImport("ProjectX.AnalyticsLibNative")]
    static public extern IntPtr CreateAPI();

    [DllImport("ProjectX.AnalyticsLibNative")]
    static public extern void DisposeAPI(IntPtr pClassNameObject);

    [DllImport("ProjectX.AnalyticsLibNative")]
    static public extern double BlackScholes_PV(IntPtr pClassNameObject, ref VanillaOptionParameters TheOption, double Spot, double Vol, double r);
    [DllImport("ProjectX.AnalyticsLibNative")]
    static public extern double BlackScholes_Delta(IntPtr a_pObject, ref VanillaOptionParameters TheOption, double Spot, double Vol, double r);
    [DllImport("ProjectX.AnalyticsLibNative")]
    static public extern double BlackScholes_Gamma(IntPtr a_pObject, ref VanillaOptionParameters TheOption, double Spot, double Vol, double r, double epsilon);
    [DllImport("ProjectX.AnalyticsLibNative")]
    static public extern double BlackScholes_Vega(IntPtr a_pObject, ref VanillaOptionParameters TheOption, double Spot, double Vol, double r);
    [DllImport("ProjectX.AnalyticsLibNative")]
    static public extern double BlackScholes_Theta(IntPtr a_pObject, ref VanillaOptionParameters TheOption, double Spot, double Vol, double r);
    [DllImport("ProjectX.AnalyticsLibNative")]
    static public extern double BlackScholes_Rho(IntPtr a_pObject, ref VanillaOptionParameters TheOption, double Spot, double Vol, double r);
    [DllImport("ProjectX.AnalyticsLibNative")]
    static public extern double BlackScholes_ImpliedVolatility(IntPtr a_pObject, ref VanillaOptionParameters TheOption, double Spot, double r, double optionPrice);
    [DllImport("ProjectX.AnalyticsLibNative")]
    static public extern GreekResults MonteCarlo_PV(IntPtr a_pObject, ref VanillaOptionParameters TheOption, double Spot, double Vol, double r, ulong NumberOfPaths);
    [DllImport("ProjectX.AnalyticsLibNative")]
    static public extern double MonteCarlo_ImpliedVolatility(IntPtr a_pObject, ref VanillaOptionParameters TheOption, double Spot, double r, ulong NumberOfPaths, double optionPrice);

    public double BlackScholes_PV(VanillaOptionParameters TheOption, double Spot, double Vol, double r) => 
        (double)SafeExecute((pAPI) => BlackScholes_PV(pAPI, ref TheOption, Spot, Vol, r));
    public double BlackScholes_Delta(VanillaOptionParameters TheOption, double Spot, double Vol, double r) => 
        (double)SafeExecute((pAPI) => BlackScholes_Delta(pAPI, ref TheOption, Spot, Vol, r));
    public double BlackScholes_Gamma(VanillaOptionParameters TheOption, double Spot, double Vol, double r, double epsilon) => 
        (double)SafeExecute((pAPI) => BlackScholes_Gamma(pAPI, ref TheOption, Spot, Vol, r, epsilon));
    public double BlackScholes_Theta(VanillaOptionParameters TheOption, double Spot, double Vol, double r) => 
        (double)SafeExecute((pAPI) => BlackScholes_Theta(pAPI, ref TheOption, Spot, Vol, r));
    public double BlackScholes_Rho(VanillaOptionParameters TheOption, double Spot, double Vol, double r) => 
        (double)SafeExecute((pAPI) => BlackScholes_Rho(pAPI, ref TheOption, Spot, Vol, r));
    public double BlackScholes_Vega(VanillaOptionParameters TheOption, double Spot, double Vol, double r) =>
        (double)SafeExecute((pAPI) => BlackScholes_Vega(pAPI, ref TheOption, Spot, Vol, r));
    public double BlackScholes_ImpliedVolatility(VanillaOptionParameters TheOption, double Spot, double r, double optionPrice) =>
      (double)SafeExecute((pAPI) => BlackScholes_ImpliedVolatility(pAPI, ref TheOption, Spot, r, optionPrice));
    public GreekResults MonteCarlo_PV(VanillaOptionParameters TheOption, double Spot, double Vol, double r, ulong NumberOfPaths) =>
      (GreekResults)SafeExecute((pAPI) => MonteCarlo_PV(pAPI, ref TheOption, Spot, Vol, r, NumberOfPaths));
    public double MonteCarlo_ImpliedVolatility(VanillaOptionParameters TheOption, double Spot, double r, ulong NumberOfPaths, double optionPrice) =>
      (double)SafeExecute((pAPI) => MonteCarlo_ImpliedVolatility(pAPI, ref TheOption, Spot, r, NumberOfPaths, optionPrice));

    private static T SafeExecute<T>(Func<nint, T> action)
    {
        IntPtr pAPI = IntPtr.Zero;
        try
        {
            //use the functions
            pAPI = CreateAPI();

            return action(pAPI);

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error occured {ex.Message}");
            throw;
        }
        finally
        {
            DisposeAPI(pAPI);
        }
    }
}