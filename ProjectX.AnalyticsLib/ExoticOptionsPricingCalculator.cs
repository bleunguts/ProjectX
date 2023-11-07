using ProjectX.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlackScholesFns = ProjectX.Core.Analytics.BlackScholesFunctions;


namespace ProjectX.AnalyticsLib;
public interface IExoticOptionsPricingCalculator
{
    double American_BaroneAdesiWhaley(OptionType optionType, double spot, double strike, double rate, double divYield, double maturity, double vol);
    double? BarrierOptions(OptionType optionType, BarrierType barrierType, double spot, double strike, double rate, double divYield, double maturity, double vol, double barrierLevel, double rebate);
}

[Export(typeof(IExoticOptionsPricingCalculator)), PartCreationPolicy(CreationPolicy.Shared)]
public class ExoticOptionsPricingCalculator : IExoticOptionsPricingCalculator
{
    private readonly IOptionsGreeksCalculator _bs;

    [ImportingConstructor]
    public ExoticOptionsPricingCalculator(IOptionsGreeksCalculator blackScholesOptionsPricingCalculator)
    {
        _bs = blackScholesOptionsPricingCalculator;
    }
    public double American_BaroneAdesiWhaley(OptionType optionType, double spot, double strike, double rate, double divYield, double maturity, double vol)
    {
        double carry = rate - divYield;
        return optionType == OptionType.Put ? AmericanPut_BaroneAdesiWhaley(spot, strike, rate, carry, maturity, vol) : AmericanCall_BaroneAdesiWhaley(spot, strike, rate, carry, maturity, vol);
    }

    private double AmericanCall_BaroneAdesiWhaley(double spot, double strike, double rate, double carry, double maturity, double volatility)
    {
        // When b>=r the american call price is equal to the european clall price for generalized Black-Scholes Formula
        if (carry >= rate) return _bs.PV(OptionType.Call, spot, strike, rate, carry, maturity, volatility);

        double sk = AmericanCall_NewtonRaphson(strike, rate, carry, maturity, volatility);
        double d1 = BlackScholesFns.d1_(spot, strike, carry, volatility, maturity);
        double q2 = q2_(carry, rate, volatility, maturity);

        double A2 = sk / q2 * (1.0 - Math.Exp((carry - rate) * maturity) * BlackScholesFns.CummulativeNormal(d1));

        return spot < sk
            ? _bs.PV(OptionType.Call, spot, strike, rate, carry, maturity, volatility) + A2 * Math.Pow(spot / sk, q2)
            : spot - strike;
    }

    private double AmericanPut_BaroneAdesiWhaley(double spot, double strike, double rate, double carry, double maturity, double volatility)
    {
        // When b>=r the american call price is equal to the european clall price for generalized Black-Scholes Formula
        if (carry >= rate) return _bs.PV(OptionType.Put, spot, strike, rate, carry, maturity, volatility);

        double sk = AmericanPut_NewtonRaphson(strike, rate, carry, maturity, volatility);
        double d1 = BlackScholesFns.d1_(spot, strike, carry, volatility, maturity);
        double q1 = q1_(carry, rate, volatility, maturity);
        double A1 = -(sk / q1) * (1.0 - Math.Exp((carry - rate) * maturity)) * BlackScholesFns.CummulativeNormal(-d1);

        return spot > sk
            ? _bs.PV(OptionType.Put, spot, strike, rate, carry, maturity, volatility) + A1 * Math.Pow(spot / sk, q1)
            : strike - spot;
    }

    #region American Option Fns
    static double N_(double carry, double vol) => 2 * carry / Math.Pow(vol, 2);
    static double M_(double rate, double vol) => 2 * rate / Math.Pow(vol, 2);
    static double L_(double rate, double maturity) => 1 - Math.Exp(-rate * maturity);
    static double q2_(double carry, double rate, double vol, double maturity) => q2_(carry, rate, vol, maturity, L_(rate, maturity));
    static double q2_(double carry, double rate, double vol, double maturity, double L)
        => -(N_(carry, vol) - 1.0) + Math.Sqrt(Math.Pow(N_(carry, vol) - 1.0, 2) + 4 * M_(rate, vol) / L) / 2.0;
    static double q1_(double carry, double rate, double vol, double maturity) => q1_(carry, rate, vol, maturity, L_(rate, maturity));
    static double q1_(double carry, double rate, double vol, double maturity, double L)
   => (N_(carry, vol) - 1.0 - Math.Sqrt(Math.Pow(N_(carry, vol) - 1.0, 2) + 4 * M_(rate, vol) / L)) / 2.0;
    #endregion

    private double AmericanCall_NewtonRaphson(double strike, double rate, double carry, double maturity, double volatility)
    {
        double q2u = q2_(carry, rate, volatility, maturity, 1);
        double su = strike / (1.0 - 1.0 / q2u);
        double h2 = (carry * maturity + 2.0 * volatility * Math.Sqrt(maturity)) * strike / (su - strike);
        double spot = strike + (su - strike) * (1.0 - Math.Exp(h2));

        double d1 = BlackScholesFns.d1_(spot, strike, carry, volatility, maturity);
        double q2 = q2_(carry, rate, volatility, maturity);
        double lhs = lhs_(spot, strike);
        double rhs = rhs_(strike, rate, carry, maturity, volatility, q2, spot, d1);
        double bi = bi_(carry, rate, maturity, d1, q2, volatility);

        while (Math.Abs((lhs - rhs) / strike) > 0.000001)
        {
            spot = (strike + rhs - bi * spot) / (1.0 - bi);
            d1 = BlackScholesFns.d1_(spot, strike, carry, volatility, maturity);
            lhs = lhs_(spot, strike);
            rhs = rhs_(strike, rate, carry, maturity, volatility, q2, spot, d1);
            bi = bi_(carry, rate, maturity, d1, q2, volatility);
        }
        return spot;

        double lhs_(double spot_, double strike_) => spot_ - strike_;
        double rhs_(double strike_, double rate_, double carry_, double maturity_, double volatility_, double q2_, double spot_, double d1_) => _bs.PV(OptionType.Call, spot_, strike_, rate_, carry_, maturity_, volatility_) + (1.0 - Math.Exp((carry_ - rate_) * maturity_) * BlackScholesFns.CummulativeNormal(d1_)) * spot_ / q2_;
        double bi_(double carry_, double rate_, double maturity_, double d1_, double q2_, double volatility_) =>
            Math.Exp((carry_ - rate_) * maturity_) * BlackScholesFns.CummulativeNormal(d1_) * (1.0 - 1.0 / q2_) +
            (1.0 - Math.Exp((carry_ - rate_) * maturity_) * BlackScholesFns.NormalDensity(d1_) / (volatility_ * Math.Sqrt(maturity_))) / q2_;
    }
    private double AmericanPut_NewtonRaphson(double strike, double rate, double carry, double maturity, double volatility)
    {
        double q1u = q1_(carry, rate, volatility, maturity, 1);
        double su = strike / (1.0 - 1.0 / q1u);
        double h1 = (carry * maturity - 2.0 * volatility * Math.Sqrt(maturity)) * strike / (strike - su);
        double spot = su + (strike - su) * Math.Exp(h1);

        double d1 = BlackScholesFns.d1_(spot, strike, carry, volatility, maturity);
        double q1 = q1_(carry, rate, volatility, maturity);
        double lhs = lhs_(spot, strike);
        double rhs = rhs_(strike, rate, carry, maturity, volatility, q1, spot, d1);
        double bi = bi_(carry, rate, maturity, d1, q1, volatility);

        while (Math.Abs((lhs - rhs) / strike) > 0.000001)
        {
            spot = (strike - rhs + bi * spot) / (1.0 + bi);
            d1 = BlackScholesFns.d1_(spot, strike, carry, volatility, maturity);
            lhs = lhs_(spot, strike);
            rhs = rhs_(strike, rate, carry, maturity, volatility, q1, spot, d1);
            bi = bi_(carry, rate, maturity, d1, q1, volatility);
        }
        return spot;

        double lhs_(double spot_, double strike_) => strike_ - spot_;
        double rhs_(double strike_, double rate_, double carry_, double maturity_, double volatility_, double q1_, double spot_, double d1_) => _bs.PV(OptionType.Put, spot_, strike_, rate_, carry_, maturity_, volatility_) - (1.0 - Math.Exp((carry_ - rate_) * maturity_) * BlackScholesFns.CummulativeNormal(-d1_)) * spot_ / q1_;
        double bi_(double carry_, double rate_, double maturity_, double d1_, double q1_, double volatility_) =>
            -Math.Exp((carry_ - rate_) * maturity_) * BlackScholesFns.CummulativeNormal(-d1_) * (1.0 - 1.0 / q1) -
          (1.0 + Math.Exp((carry_ - rate_) * maturity_) * BlackScholesFns.NormalDensity(-d1_) / (volatility_ * Math.Sqrt(maturity_))) / q1;
    }

    public double? BarrierOptions(OptionType optionType, BarrierType barrierType, double spot, double strike, double rate, double divYield, double maturity, double vol, double barrierLevel, double rebate)
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

    private double? BarrierOptionsCall(BarrierType barrierType, double spot, double strike, double rate, double divYield, double maturity, double vol, double barrierLevel, double rebate)
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

    private double? BarrierOptionsPut(BarrierType barrierType, double spot, double strike, double rate, double divYield, double maturity, double vol, double barrierLevel, double rebate)
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

    #region Barrier Fns
    private static double fac_(double vol, double maturity) => vol * Math.Sqrt(maturity);
    private static double m_(double rate, double divYield, double vol) => (rate - divYield - Math.Pow(vol, 2) / 2) / Math.Pow(vol, 2);

    private static double hs_(double barrierLevel, double spot) => barrierLevel / spot;

    private static double A(double phi, double barrierLevel, double spot, double strike, double vol, double maturity, double rate, double divYield)
    {
        double fac = fac_(vol, maturity);
        double m = m_(rate, divYield, vol);
        double x1 = Math.Log(spot / strike) / fac + (1.0 + m) * fac;
        return phi * spot * Math.Exp(-divYield * maturity) * BlackScholesFns.CummulativeNormal(phi * x1) -
            phi * strike * Math.Exp(-rate * maturity) * BlackScholesFns.CummulativeNormal(phi * x1 - phi * fac);
    }

    private static double B(double phi, double barrierLevel, double spot, double strike, double vol, double maturity, double rate, double divYield)
    {
        double fac = fac_(vol, maturity);
        double m = m_(rate, divYield, vol);
        double x2 = Math.Log(spot / barrierLevel) / fac + (1.0 + m) * fac;
        return phi * spot * Math.Exp(-divYield * maturity) * BlackScholesFns.CummulativeNormal(phi * x2) -
           phi * strike * Math.Exp(-rate * maturity) * BlackScholesFns.CummulativeNormal(phi * x2 - phi * fac_(vol, maturity));
    }

    private static double C(double phi, double eta, double barrierLevel, double spot, double strike, double vol, double maturity, double rate, double divYield)
    {
        double hs = hs_(barrierLevel, spot);
        double fac = fac_(vol, maturity);
        double m = m_(rate, divYield, vol);

        double y1 = Math.Log(barrierLevel * barrierLevel / strike / spot) / fac + (1.0 + m) * fac;
        return phi * spot * Math.Exp(-divYield * maturity) * Math.Pow(hs, 2 * m) * Math.Pow(hs, 2) * BlackScholesFns.CummulativeNormal(eta * y1) -
            phi * strike * Math.Exp(-rate * maturity) * Math.Pow(hs, 2 * m) * BlackScholesFns.CummulativeNormal(eta * y1 - eta * fac);
    }

    private static double D(double phi, double eta, double barrierLevel, double spot, double strike, double vol, double maturity, double rate, double divYield)
    {
        double hs = hs_(barrierLevel, spot);
        double fac = fac_(vol, maturity);
        double m = m_(rate, divYield, vol);

        double y2 = Math.Log(barrierLevel / spot) / fac + (1.0 + m) * fac;
        return phi * spot * Math.Exp(-divYield * maturity) * Math.Pow(hs, 2 * m) * Math.Pow(hs, 2) * BlackScholesFns.CummulativeNormal(eta * y2) - phi * strike * Math.Exp(-rate * maturity) * Math.Pow(hs, 2 * m) * BlackScholesFns.CummulativeNormal(eta * y2 - eta * fac);

    }

    private static double E(double eta, double rebate, double barrierLevel, double spot, double strike, double vol, double maturity, double rate, double divYield)
    {
        if (rebate <= 0.0) return 0.0;

        double hs = hs_(barrierLevel, spot);
        double fac = fac_(vol, maturity);
        double m = m_(rate, divYield, vol);

        double x2 = Math.Log(spot / barrierLevel) / fac + (1.0 + m) * fac;
        double y2 = Math.Log(barrierLevel / spot) / fac + (1.0 + m) * fac;
        return rebate * Math.Exp(-rate * maturity) * BlackScholesFns.CummulativeNormal(eta * y2 - eta * fac) - Math.Pow(hs, 2 * m) * BlackScholesFns.CummulativeNormal(eta * y2 - eta * fac);
    }

    private static double F(double eta, double rebate, double barrierLevel, double spot, double strike, double vol, double maturity, double rate, double divYield)
    {
        if (rebate <= 0.0) return 0.0;

        double hs = hs_(barrierLevel, spot);
        double fac = fac_(vol, maturity);
        double m = m_(rate, divYield, vol);


        double lambda = Math.Sqrt(Math.Pow(m, 2) + 2.0 * (rate - divYield) / Math.Pow(vol, 2));
        double z = Math.Log(barrierLevel / spot) / fac + lambda * fac;
        return rebate * (Math.Pow(hs, m + lambda) * BlackScholesFns.CummulativeNormal(eta * z) - Math.Pow(hs, m - lambda) * BlackScholesFns.CummulativeNormal(eta * z - 2.0 * eta * lambda * fac));
    }
    #endregion 
}
