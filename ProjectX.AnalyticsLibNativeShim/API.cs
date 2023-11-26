namespace ProjectX.AnalyticsLibNativeShim;

using System;
using System.Runtime.InteropServices;

public enum OptionType { Call, Put };
[StructLayout(LayoutKind.Sequential)]
public struct VanillaOptionParameters
{
    public OptionType OptionType;
    public double Strike;
    public double Expiry;
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
    
    public double BlackScholes_PV(VanillaOptionParameters TheOption, double Spot, double Vol, double r)
    {
        var value = SafeExecute((pAPI) =>
        {
            return BlackScholes_PV(pAPI, ref TheOption, Spot, Vol, r);                        
        });

        return value;   
    }

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