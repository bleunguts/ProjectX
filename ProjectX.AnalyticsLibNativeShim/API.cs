namespace ProjectX.AnalyticsLibNativeShim;

using System;
using System.Runtime.InteropServices;

// https://mark-borg.github.io/blog/2017/interop/#:~:text=Platform%20Invocation%20(PInvoke%20for%20short,from%20within%20a%20C%23%20program.
// https://stackoverflow.com/questions/315051/using-a-class-defined-in-a-c-dll-in-c-sharp-code
public class API
{    
    [DllImport("ProjectX.AnalyticsLibNative")]
    static public extern IntPtr CreateAPI();

    [DllImport("ProjectX.AnalyticsLibNative")]
    static public extern void DisposeAPI(IntPtr pClassNameObject);

    [DllImport("ProjectX.AnalyticsLibNative")]
    static public extern double CallExecute(IntPtr pClassNameObject);    

    public void Execute()
    {
        IntPtr pAPI = IntPtr.Zero;
        try
        {
            //use the functions
            pAPI = CreateAPI();

            var value = CallExecute(pAPI);
            Console.WriteLine($"PV = {value}");
            
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error occured {ex.Message}");
        }
        finally
        {
            DisposeAPI(pAPI);
        }        
    }
}