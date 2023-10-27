using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectX.Core.Analytics;
public class OptionsCalculatorFunctions
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
    private static double NormalDensity(double z) => Math.Exp(-z * z * 0.5) / Math.Sqrt(2.0 * PI);

    static double d1_(double spot, double strike, double carry, double volatility, double maturity) =>
        (Math.Log(spot / strike) + (carry + Math.Pow(volatility, 2) / 2) * maturity) /
        (volatility * Math.Sqrt(maturity));

    static double d2_(double d1, double volatility, double maturity) =>
        d1 - volatility * Math.Sqrt(maturity);

    /// <summary>
    /// Generalized Black Scholes Model 
    /// Supports: 
    /// * Pricing European options on stocks
    /// * Stocks paying a continuous dividend yield
    /// * Options on Futures Contracts
    /// * Currency Options
    /// </summary>
    /// <param name="optionType"></param>
    /// <param name="spot">spot price</param>
    /// <param name="strike">strike price</param>
    /// <param name="rate">interest rate used by TVM calcs</param>
    /// <param name="carry">cost-of-carry rate. The net cost of holding a position e.g. storage costs for physicals, interest expenses, opportunity costs lost for taking this position over another</param>
    /// <param name="maturity">number of days to maturity</param>
    /// <param name="volatility">price volatility std.dev</param>
    /// <returns></returns>
    public static double BlackScholes(OptionType optionType, double spot, double strike, double rate, double carry, double maturity, double volatility)
    {
        double d1 = d1_(spot, strike, carry, volatility, maturity);
        double d2 = d2_(d1, volatility, maturity);

        double? option = null;
        switch (optionType)
        {
            case OptionType.Put:
                option = strike * Math.Exp(-rate * maturity) * CummulativeNormal(-d2) - spot * Math.Exp((carry - rate) * maturity) * CummulativeNormal(-d1);
                break;
            case OptionType.Call:
                option = spot * Math.Exp((carry - rate) * maturity) * CummulativeNormal(d1) - strike * Math.Exp(-rate * maturity) * CummulativeNormal(d2);
                break;
        }
        return option.Value;
    }

    public static double BlackScholes_Delta(OptionType optionType, double spot, double strike, double rate, double carry, double maturity, double volatility)
    {
        double d1 = d1_(spot, strike, carry, volatility, maturity);

        double? option = null;
        switch (optionType)
        {
            case OptionType.Put:
                option = Math.Exp((carry - rate) * maturity) * (CummulativeNormal(d1) - 1.0);
                break;
            case OptionType.Call:
                option = Math.Exp((carry - rate) * maturity) * CummulativeNormal(d1);
                break;
        }
        return option.Value;
    }

    public static double BlackScholes_Gamma(double spot, double strike, double rate, double carry, double maturity, double volatility)
    {
        double d1 = d1_(spot, strike, carry, volatility, maturity);

        double option = NormalDensity(d1) * Math.Exp((carry - rate) * maturity) / (spot * volatility * Math.Sqrt(maturity));
        return option;
    }

    public static double BlackScholes_Theta(OptionType optionType, double spot, double strike, double rate, double carry, double maturity, double volatility)
    {
        double d1 = d1_(spot, strike, carry, volatility, maturity);
        double d2 = d2_(d1, volatility, maturity);

        double? option = null;
        switch (optionType)
        {
            case OptionType.Put:
                var p1 = spot * Math.Exp((carry - rate) * maturity) * NormalDensity(d1) * volatility / (2 * Math.Sqrt(maturity));
                var p2 = (carry - rate) * spot * Math.Exp((carry - rate) * maturity) * CummulativeNormal(-d1);
                var p3 = rate * strike * Math.Exp(-rate * maturity) * CummulativeNormal(-d2);
                option = -p1 + p2 + p3;
                break;
            case OptionType.Call:
                var c1 = spot * Math.Exp((carry - rate) * maturity) * NormalDensity(d1) * volatility / (2 * Math.Sqrt(maturity));
                var c2 = (carry - rate) * spot * Math.Exp((carry - rate) * maturity) * CummulativeNormal(d1);
                var c3 = rate * strike * Math.Exp(-rate * maturity) * CummulativeNormal(d2);
                option = -c1 - c2 - c3;
                break;
        }
        return option.Value;
    }

    public static double BlackScholes_Rho(OptionType optionType, double spot, double strike, double rate, double carry, double maturity, double volatility)
    {
        double d1 = d1_(spot, strike, carry, volatility, maturity);
        double d2 = d2_(d1, volatility, maturity);

        // carry == 0 means option on a futures contract simple formula in that case
        if (carry == 0)
        {
            return -maturity * BlackScholes(optionType, spot, strike, rate, 0, maturity, volatility);
        }

        // otherwise
        double? option = null;
        switch (optionType)
        {
            case OptionType.Put:
                option = -maturity * strike * Math.Exp(-rate * maturity) * CummulativeNormal(-d2);
                break;
            case OptionType.Call:
                option = maturity * strike * Math.Exp(-rate * maturity) * CummulativeNormal(d2);
                break;
        }

        return option.Value;
    }

    public static double BlackScholes_Vega(double spot, double strike, double rate, double carry, double maturity, double vol)
    {
        double d1 = d1_(spot, strike, carry, vol, maturity);
        return spot * Math.Exp((carry - rate) * maturity) * CummulativeNormal(d1) * Math.Sqrt(maturity);
    }

    public static double BlackScholes_ImpliedVol(OptionType optionType, double spot, double strike, double rate, double carry, double maturity, double price)
    {
        double low = 0.0;
        double high = 4.0;
        if (BlackScholes(optionType, spot, strike, rate, carry, maturity, high) < price) return high;
        if (BlackScholes(optionType, spot, strike, rate, carry, maturity, low) > price) return low;

        double vol = (high + low) * 0.5; // 2.0
        int count = 0;
        while (vol - low > 0.0001 && count < 100_000)
        {
            double impliedPrice = BlackScholes(optionType, spot, strike, rate, carry, maturity, vol);
            if (impliedPrice < price)
                low = vol;
            else if (impliedPrice > price)
                high = vol;
            vol = (high + low) * 0.5;
            count++;
        }
        return vol;
    }

    public static double American_BaroneAdesiWhaley(OptionType optionType, double spot, double strike, double rate, double divYield, double maturity, double vol)
    {
        double carry = rate - divYield;
        return optionType == OptionType.Put ? AmericanPut_BaroneAdesiWhaley(spot, strike, rate, carry, maturity, vol) : AmericanCall_BaroneAdesiWhaley(spot, strike, rate, carry, maturity, vol);
    }

    private static double AmericanCall_BaroneAdesiWhaley(double spot, double strike, double rate, double carry, double maturity, double volatility)
    {
        // When b>=r the american call price is equal to the european clall price for generalized Black-Scholes Formula
        if (carry >= rate) return BlackScholes(OptionType.Call, spot, strike, rate, carry, maturity, volatility);

        double sk = AmericanCall_NewtonRaphson(strike, rate, carry, maturity, volatility);
        double d1 = d1_(spot, strike, carry, volatility, maturity);
        double q2 = q2_(carry, rate, volatility, maturity);

        double A2 = sk / q2 * (1.0 - Math.Exp((carry - rate) * maturity) * CummulativeNormal(d1));

        return spot < sk
            ? BlackScholes(OptionType.Call, spot, strike, rate, carry, maturity, volatility) + A2 * Math.Pow(spot / sk, q2)
            : spot - strike;
    }

    private static double AmericanPut_BaroneAdesiWhaley(double spot, double strike, double rate, double carry, double maturity, double volatility)
    {
        // When b>=r the american call price is equal to the european clall price for generalized Black-Scholes Formula
        if (carry >= rate) return BlackScholes(OptionType.Put, spot, strike, rate, carry, maturity, volatility);

        double sk = AmericanPut_NewtonRaphson(strike, rate, carry, maturity, volatility);
        double d1 = d1_(spot, strike, carry, volatility, maturity);
        double q1 = q1_(carry, rate, volatility, maturity);
        double A1 = -(sk / q1) * (1.0 - Math.Exp((carry - rate) * maturity)) * CummulativeNormal(-d1);

        return spot > sk
            ? BlackScholes(OptionType.Put, spot, strike, rate, carry, maturity, volatility) + A1 * Math.Pow(spot / sk, q1)
            : strike - spot;
    }

    static double N_(double carry, double vol) => 2 * carry / Math.Pow(vol, 2);
    static double M_(double rate, double vol) => 2 * rate / Math.Pow(vol, 2);
    static double L_(double rate, double maturity) => 1 - Math.Exp(-rate * maturity);
    static double q2_(double carry, double rate, double vol, double maturity) => q2_(carry, rate, vol, maturity, L_(rate, maturity));
    static double q2_(double carry, double rate, double vol, double maturity, double L)
        => -(N_(carry, vol) - 1.0) + Math.Sqrt(Math.Pow(N_(carry, vol) - 1.0, 2) + 4 * M_(rate, vol) / L) / 2.0;
    static double q1_(double carry, double rate, double vol, double maturity) => q1_(carry, rate, vol, maturity, L_(rate, maturity));
    static double q1_(double carry, double rate, double vol, double maturity, double L)
   => (N_(carry, vol) - 1.0 - Math.Sqrt(Math.Pow(N_(carry, vol) - 1.0, 2) + 4 * M_(rate, vol) / L)) / 2.0;

    private static double AmericanCall_NewtonRaphson(double strike, double rate, double carry, double maturity, double volatility)
    {
        double q2u = q2_(carry, rate, volatility, maturity, 1);
        double su = strike / (1.0 - 1.0 / q2u);
        double h2 = (carry * maturity + 2.0 * volatility * Math.Sqrt(maturity)) * strike / (su - strike);
        double spot = strike + (su - strike) * (1.0 - Math.Exp(h2));

        double d1 = d1_(spot, strike, carry, volatility, maturity);
        double q2 = q2_(carry, rate, volatility, maturity);
        double lhs = lhs_(spot, strike);
        double rhs = rhs_(strike, rate, carry, maturity, volatility, q2, spot, d1);
        double bi = bi_(carry, rate, maturity, d1, q2, volatility);

        while (Math.Abs((lhs - rhs) / strike) > 0.000001)
        {
            spot = (strike + rhs - bi * spot) / (1.0 - bi);
            d1 = d1_(spot, strike, carry, volatility, maturity);
            lhs = lhs_(spot, strike);
            rhs = rhs_(strike, rate, carry, maturity, volatility, q2, spot, d1);
            bi = bi_(carry, rate, maturity, d1, q2, volatility);
        }
        return spot;

        double lhs_(double spot_, double strike_) => spot_ - strike_;
        double rhs_(double strike_, double rate_, double carry_, double maturity_, double volatility_, double q2_, double spot_, double d1_) => BlackScholes(OptionType.Call, spot_, strike_, rate_, carry_, maturity_, volatility_) + (1.0 - Math.Exp((carry_ - rate_) * maturity_) * CummulativeNormal(d1_)) * spot_ / q2_;
        double bi_(double carry_, double rate_, double maturity_, double d1_, double q2_, double volatility_) =>
            Math.Exp((carry_ - rate_) * maturity_) * CummulativeNormal(d1_) * (1.0 - 1.0 / q2_) +
            (1.0 - Math.Exp((carry_ - rate_) * maturity_) * NormalDensity(d1_) / (volatility_ * Math.Sqrt(maturity_))) / q2_;
    }
    private static double AmericanPut_NewtonRaphson(double strike, double rate, double carry, double maturity, double volatility)
    {
        double q1u = q1_(carry, rate, volatility, maturity, 1);
        double su = strike / (1.0 - 1.0 / q1u);
        double h1 = (carry * maturity - 2.0 * volatility * Math.Sqrt(maturity)) * strike / (strike - su);
        double spot = su + (strike - su) * Math.Exp(h1);

        double d1 = d1_(spot, strike, carry, volatility, maturity);
        double q1 = q1_(carry, rate, volatility, maturity);
        double lhs = lhs_(spot, strike);
        double rhs = rhs_(strike, rate, carry, maturity, volatility, q1, spot, d1);
        double bi = bi_(carry, rate, maturity, d1, q1, volatility);

        while (Math.Abs((lhs - rhs) / strike) > 0.000001)
        {
            spot = (strike - rhs + bi * spot) / (1.0 + bi);
            d1 = d1_(spot, strike, carry, volatility, maturity);
            lhs = lhs_(spot, strike);
            rhs = rhs_(strike, rate, carry, maturity, volatility, q1, spot, d1);
            bi = bi_(carry, rate, maturity, d1, q1, volatility);
        }
        return spot;

        double lhs_(double spot_, double strike_) => strike_ - spot_;
        double rhs_(double strike_, double rate_, double carry_, double maturity_, double volatility_, double q1_, double spot_, double d1_) => BlackScholes(OptionType.Put, spot_, strike_, rate_, carry_, maturity_, volatility_) - (1.0 - Math.Exp((carry_ - rate_) * maturity_) * CummulativeNormal(-d1_)) * spot_ / q1_;
        double bi_(double carry_, double rate_, double maturity_, double d1_, double q1_, double volatility_) =>
            -Math.Exp((carry_ - rate_) * maturity_) * CummulativeNormal(-d1_) * (1.0 - 1.0 / q1) -
          (1.0 + Math.Exp((carry_ - rate_) * maturity_) * NormalDensity(-d1_) / (volatility_ * Math.Sqrt(maturity_))) / q1;
    }

    public static double? BarrierOptions(OptionType optionType, BarrierType barrierType, double spot, double strike, double rate, double divYield, double maturity, double vol, double barrierLevel, double rebate)
    {
        switch (optionType)
        {
            case OptionType.Put:
                return BarrierOptionsPut(barrierType, spot, strike, rate, divYield, maturity, vol, barrierLevel, rebate);
            case OptionType.Call:
                return BarrierOptionsCall(barrierType, spot, strike, rate, divYield, maturity, vol, barrierLevel, rebate);
        }

        throw new NotSupportedException($"OptionType {optionType} and BarrierType {barrierType} not supported.");
    }

    private static double? BarrierOptionsCall(BarrierType barrierType, double spot, double strike, double rate, double divYield, double maturity, double vol, double barrierLevel, double rebate)
    {
        switch (barrierType)
        {
            case BarrierType.DownIn:
                if (!(spot > barrierLevel)) return null;

                if (strike >= barrierLevel)
                    return C(1, 1, barrierLevel, spot, strike, vol, maturity, rate, divYield) +
                            E(1, rebate, barrierLevel, spot, strike, vol, maturity, rate, divYield);
                else
                    return A(1, barrierLevel, spot, strike, vol, maturity, rate, divYield) -
                            B(1, barrierLevel, spot, strike, vol, maturity, rate, divYield) +
                            D(1, 1, barrierLevel, spot, strike, vol, maturity, rate, divYield) +
                            E(1, rebate, barrierLevel, spot, strike, vol, maturity, rate, divYield);

            case BarrierType.UpIn:
                if (!(spot < barrierLevel)) return null;

                if (strike >= barrierLevel)
                    return A(1, barrierLevel, spot, strike, vol, maturity, rate, divYield) +
                        E(-1, rebate, barrierLevel, spot, strike, vol, maturity, rate, divYield);
                else
                    return B(1, barrierLevel, spot, strike, vol, maturity, rate, divYield) +
                        C(-1, 1, barrierLevel, spot, strike, vol, maturity, rate, divYield) +
                        D(-1, 1, barrierLevel, spot, strike, vol, maturity, rate, divYield) +
                        E(-1, rebate, barrierLevel, spot, strike, vol, maturity, rate, divYield);
            case BarrierType.DownOut:
                if (!(spot > barrierLevel)) return null;
                if (strike > barrierLevel)
                    return A(1, barrierLevel, spot, strike, vol, maturity, rate, divYield) -
                        C(1, 1, barrierLevel, spot, strike, vol, maturity, rate, divYield) +
                        F(1, rebate, barrierLevel, spot, strike, vol, maturity, rate, divYield);
                else
                    return B(1, barrierLevel, spot, strike, vol, maturity, rate, divYield) -
                        D(1, 1, barrierLevel, spot, strike, vol, maturity, rate, divYield) +
                        F(1, rebate, barrierLevel, spot, strike, vol, maturity, rate, divYield);

            case BarrierType.UpOut:
                if (!(spot < barrierLevel)) return null;
                if (strike > barrierLevel)
                    return F(-1, rebate, barrierLevel, spot, strike, vol, maturity, rate, divYield);
                else
                    return A(1, barrierLevel, spot, strike, vol, maturity, rate, divYield) -
                        B(1, barrierLevel, spot, strike, vol, maturity, rate, divYield) +
                        C(-1, 1, barrierLevel, spot, strike, vol, maturity, rate, divYield) -
                        D(-1, 1, barrierLevel, spot, strike, vol, maturity, rate, divYield) +
                        F(-1, rebate, barrierLevel, spot, strike, vol, maturity, rate, divYield);
        }

        throw new NotSupportedException($"BarrierType {barrierType} not supported.");
    }

    private static double? BarrierOptionsPut(BarrierType barrierType, double spot, double strike, double rate, double divYield, double maturity, double vol, double barrierLevel, double rebate)
    {
        switch (barrierType)
        {
            case BarrierType.DownIn:
                if (!(spot > barrierLevel)) return null;

                if (strike >= barrierLevel)
                    return B(-1, barrierLevel, spot, strike, vol, maturity, rate, divYield) -
                            C(1, -1, barrierLevel, spot, strike, vol, maturity, rate, divYield) +
                            D(1, -1, barrierLevel, spot, strike, vol, maturity, rate, divYield) +
                            E(1, rebate, barrierLevel, spot, strike, vol, maturity, rate, divYield);
                else
                    return A(-1, barrierLevel, spot, strike, vol, maturity, rate, divYield) -
                            E(1, rebate, barrierLevel, spot, strike, vol, maturity, rate, divYield);

            case BarrierType.UpIn:
                if (!(spot < barrierLevel)) return null;

                if (strike >= barrierLevel)
                    return A(-1, barrierLevel, spot, strike, vol, maturity, rate, divYield) +
                        B(-1, barrierLevel, spot, strike, vol, maturity, rate, divYield) +
                        D(-1, -1, barrierLevel, spot, strike, vol, maturity, rate, divYield) +
                        E(-1, rebate, barrierLevel, spot, strike, vol, maturity, rate, divYield);
                else
                    return C(-1, -1, barrierLevel, spot, strike, vol, maturity, rate, divYield) +
                        E(-1, rebate, barrierLevel, spot, strike, vol, maturity, rate, divYield);
            case BarrierType.DownOut:
                if (!(spot > barrierLevel)) return null;
                if (strike > barrierLevel)
                    return A(-1, barrierLevel, spot, strike, vol, maturity, rate, divYield) -
                        B(-1, barrierLevel, spot, strike, vol, maturity, rate, divYield) +
                        C(1, -1, barrierLevel, spot, strike, vol, maturity, rate, divYield) -
                        D(1, -1, barrierLevel, spot, strike, vol, maturity, rate, divYield) +
                        F(1, rebate, barrierLevel, spot, strike, vol, maturity, rate, divYield);
                else
                    return F(1, rebate, barrierLevel, spot, strike, vol, maturity, rate, divYield);

            case BarrierType.UpOut:
                if (!(spot < barrierLevel)) return null;
                if (strike > barrierLevel)
                    return B(-1, barrierLevel, spot, strike, vol, maturity, rate, divYield) -
                        D(-1, -1, barrierLevel, spot, strike, vol, maturity, rate, divYield) +
                        F(-1, rebate, barrierLevel, spot, strike, vol, maturity, rate, divYield);
                else
                    return A(-1, barrierLevel, spot, strike, vol, maturity, rate, divYield) -
                        C(-1, -1, barrierLevel, spot, strike, vol, maturity, rate, divYield) +
                        F(-1, rebate, barrierLevel, spot, strike, vol, maturity, rate, divYield);
        }

        throw new NotSupportedException($"BarrierType {barrierType} not supported.");
    }

    private static double fac_(double vol, double maturity) => vol * Math.Sqrt(maturity);
    private static double m_(double rate, double divYield, double vol) => (rate - divYield - Math.Pow(vol, 2) / 2) / Math.Pow(vol, 2);

    private static double hs_(double barrierLevel, double spot) => barrierLevel / spot;

    private static double A(double phi, double barrierLevel, double spot, double strike, double vol, double maturity, double rate, double divYield)
    {
        double fac = fac_(vol, maturity);
        double m = m_(rate, divYield, vol);
        double x1 = Math.Log(spot / strike) / fac + (1.0 + m) * fac;
        return phi * spot * Math.Exp(-divYield * maturity) * CummulativeNormal(phi * x1) -
            phi * strike * Math.Exp(-rate * maturity) * CummulativeNormal(phi * x1 - phi * fac);
    }

    private static double B(double phi, double barrierLevel, double spot, double strike, double vol, double maturity, double rate, double divYield)
    {
        double fac = fac_(vol, maturity);
        double m = m_(rate, divYield, vol);
        double x2 = Math.Log(spot / barrierLevel) / fac + (1.0 + m) * fac;
        return phi * spot * Math.Exp(-divYield * maturity) * CummulativeNormal(phi * x2) -
           phi * strike * Math.Exp(-rate * maturity) * CummulativeNormal(phi * x2 - phi * fac_(vol, maturity));
    }

    private static double C(double phi, double eta, double barrierLevel, double spot, double strike, double vol, double maturity, double rate, double divYield)
    {
        double hs = hs_(barrierLevel, spot);
        double fac = fac_(vol, maturity);
        double m = m_(rate, divYield, vol);

        double y1 = Math.Log(barrierLevel * barrierLevel / strike / spot) / fac + (1.0 + m) * fac;
        return phi * spot * Math.Exp(-divYield * maturity) * Math.Pow(hs, 2 * m) * Math.Pow(hs, 2) * CummulativeNormal(eta * y1) -
            phi * strike * Math.Exp(-rate * maturity) * Math.Pow(hs, 2 * m) * CummulativeNormal(eta * y1 - eta * fac);
    }

    private static double D(double phi, double eta, double barrierLevel, double spot, double strike, double vol, double maturity, double rate, double divYield)
    {
        double hs = hs_(barrierLevel, spot);
        double fac = fac_(vol, maturity);
        double m = m_(rate, divYield, vol);

        double y2 = Math.Log(barrierLevel / spot) / fac + (1.0 + m) * fac;
        return phi * spot * Math.Exp(-divYield * maturity) * Math.Pow(hs, 2 * m) * Math.Pow(hs, 2) * CummulativeNormal(eta * y2) - phi * strike * Math.Exp(-rate * maturity) * Math.Pow(hs, 2 * m) * CummulativeNormal(eta * y2 - eta * fac);

    }

    private static double E(double eta, double rebate, double barrierLevel, double spot, double strike, double vol, double maturity, double rate, double divYield)
    {
        if (rebate <= 0.0) return 0.0;

        double hs = hs_(barrierLevel, spot);
        double fac = fac_(vol, maturity);
        double m = m_(rate, divYield, vol);

        double x2 = Math.Log(spot / barrierLevel) / fac + (1.0 + m) * fac;
        double y2 = Math.Log(barrierLevel / spot) / fac + (1.0 + m) * fac;
        return rebate * Math.Exp(-rate * maturity) * CummulativeNormal(eta * y2 - eta * fac) - Math.Pow(hs, 2 * m) * CummulativeNormal(eta * y2 - eta * fac);
    }

    private static double F(double eta, double rebate, double barrierLevel, double spot, double strike, double vol, double maturity, double rate, double divYield)
    {
        if (rebate <= 0.0) return 0.0;

        double hs = hs_(barrierLevel, spot);
        double fac = fac_(vol, maturity);
        double m = m_(rate, divYield, vol);


        double lambda = Math.Sqrt(Math.Pow(m, 2) + 2.0 * (rate - divYield) / Math.Pow(vol, 2));
        double z = Math.Log(barrierLevel / spot) / fac + lambda * fac;
        return rebate * (Math.Pow(hs, m + lambda) * CummulativeNormal(eta * z) - Math.Pow(hs, m - lambda) * CummulativeNormal(eta * z - 2.0 * eta * lambda * fac));
    }
}
