using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectX.Core.Analytics;
public class BlackScholesFunctions
{
    private const double ONEOVERSQRT2PI = 0.39894228;
    private const double PI = 3.1415926;

    // Approximation of cumulative normal distributuion function
    public static double CummulativeNormal(double x)
    {
        if (x < 0)
            return 1.0 - CummulativeNormal(-x);

        double k = 1.0 / (1.0 + 0.2316419 * x);
        return 1.0 - ONEOVERSQRT2PI * Math.Exp(-0.5 * x * x) *
                ((((1.330274429 * k - 1.821255978) * k + 1.781477937) * k - 0.356563782) * k + 0.319381530) * k;
    }

    // Standard Normal Density function        
    public static double NormalDensity(double z) => Math.Exp(-z * z * 0.5) / Math.Sqrt(2.0 * PI);

    public static double d1_(double spot, double strike, double carry, double volatility, double maturity) =>
        (Math.Log(spot / strike) + (carry + Math.Pow(volatility, 2) / 2) * maturity) /
        (volatility * Math.Sqrt(maturity));

    public static double d2_(double d1, double volatility, double maturity) =>
        d1 - volatility * Math.Sqrt(maturity);
}
