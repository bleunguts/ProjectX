﻿using ProjectX.AnalyticsLib.Shared;
using ProjectX.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlackScholesFns = ProjectX.Core.Analytics.BlackScholesFunctions;

namespace ProjectX.AnalyticsLib.OptionsCalculators;

[Export(typeof(IBlackScholesCSharpPricer)), PartCreationPolicy(CreationPolicy.Shared)]
public class BlackScholesOptionsPricer : IBlackScholesCSharpPricer
{
    [ImportingConstructor]
    public BlackScholesOptionsPricer()
    {

    }
    /// <summary>
    /// Generalized Black Scholes Model 
    /// Supports: 
    /// * Pricing European options on stocks
    /// * Stocks paying a continuous dividend yield
    /// * Options on Futures Contracts
    /// * Currency Options
    /// Generalized Black-Scholes Model for stocks paying a coninuous dividend yield 
    /// requires incorporating a cost-of-carry rate b into the BlackScholes formula
    /// </summary>
    /// <param name="optionType"></param>
    /// <param name="spot">spot price</param>
    /// <param name="strike">strike price</param>
    /// <param name="rate">interest rate used by TVM calcs</param>
    /// <param name="carry">cost-of-carry rate. The net cost of holding a position e.g. storage costs for physicals, interest expenses, opportunity costs lost for taking this position over another</param>
    /// <param name="maturity">number of days to maturity</param>
    /// <param name="volatility">price volatility std.dev</param>
    /// <returns></returns>
    public double PV(OptionType optionType, double spot, double strike, double rate, double carry, double maturity, double volatility)
    {
        double d1 = BlackScholesFns.d1_(spot, strike, carry, volatility, maturity);
        double d2 = BlackScholesFns.d2_(d1, volatility, maturity);

        double? option = null;
        switch (optionType)
        {
            case OptionType.Put:
                option = strike * Math.Exp(-rate * maturity) * BlackScholesFns.CummulativeNormal(-d2) - spot * Math.Exp((carry - rate) * maturity) * BlackScholesFns.CummulativeNormal(-d1);
                break;
            case OptionType.Call:
                option = spot * Math.Exp((carry - rate) * maturity) * BlackScholesFns.CummulativeNormal(d1) - strike * Math.Exp(-rate * maturity) * BlackScholesFns.CummulativeNormal(d2);
                break;
        }
        return option.Value;
    }

    public double Delta(OptionType optionType, double spot, double strike, double rate, double carry, double maturity, double volatility)
    {
        double d1 = BlackScholesFns.d1_(spot, strike, carry, volatility, maturity);

        double? option = null;
        switch (optionType)
        {
            case OptionType.Put:
                option = Math.Exp((carry - rate) * maturity) * (BlackScholesFns.CummulativeNormal(d1) - 1.0);
                break;
            case OptionType.Call:
                option = Math.Exp((carry - rate) * maturity) * BlackScholesFns.CummulativeNormal(d1);
                break;
        }
        return option.Value;
    }

    public double Gamma(OptionType _, double spot, double strike, double rate, double carry, double maturity, double volatility)
    {
        double d1 = BlackScholesFns.d1_(spot, strike, carry, volatility, maturity);

        double option = BlackScholesFns.NormalDensity(d1) * Math.Exp((carry - rate) * maturity) / (spot * volatility * Math.Sqrt(maturity));
        return option;
    }

    public double Theta(OptionType optionType, double spot, double strike, double rate, double carry, double maturity, double volatility)
    {
        double d1 = BlackScholesFns.d1_(spot, strike, carry, volatility, maturity);
        double d2 = BlackScholesFns.d2_(d1, volatility, maturity);

        double? option = null;
        switch (optionType)
        {
            case OptionType.Put:
                var p1 = spot * Math.Exp((carry - rate) * maturity) * BlackScholesFns.NormalDensity(d1) * volatility / (2 * Math.Sqrt(maturity));
                var p2 = (carry - rate) * spot * Math.Exp((carry - rate) * maturity) * BlackScholesFns.CummulativeNormal(-d1);
                var p3 = rate * strike * Math.Exp(-rate * maturity) * BlackScholesFns.CummulativeNormal(-d2);
                option = -p1 + p2 + p3;
                break;
            case OptionType.Call:
                var c1 = spot * Math.Exp((carry - rate) * maturity) * BlackScholesFns.NormalDensity(d1) * volatility / (2 * Math.Sqrt(maturity));
                var c2 = (carry - rate) * spot * Math.Exp((carry - rate) * maturity) * BlackScholesFns.CummulativeNormal(d1);
                var c3 = rate * strike * Math.Exp(-rate * maturity) * BlackScholesFns.CummulativeNormal(d2);
                option = -c1 - c2 - c3;
                break;
        }
        return option.Value;
    }

    public double Rho(OptionType optionType, double spot, double strike, double rate, double carry, double maturity, double volatility)
    {
        double d1 = BlackScholesFns.d1_(spot, strike, carry, volatility, maturity);
        double d2 = BlackScholesFns.d2_(d1, volatility, maturity);

        // carry == 0 means option on a futures contract simple formula in that case
        if (carry == 0)
        {
            return -maturity * PV(optionType, spot, strike, rate, 0, maturity, volatility);
        }

        // otherwise
        double? option = null;
        switch (optionType)
        {
            case OptionType.Put:
                option = -maturity * strike * Math.Exp(-rate * maturity) * BlackScholesFns.CummulativeNormal(-d2);
                break;
            case OptionType.Call:
                option = maturity * strike * Math.Exp(-rate * maturity) * BlackScholesFns.CummulativeNormal(d2);
                break;
        }

        return option.Value;
    }

    public double Vega(OptionType _, double spot, double strike, double rate, double carry, double maturity, double vol)
    {
        double d1 = BlackScholesFns.d1_(spot, strike, carry, vol, maturity);
        return spot * Math.Exp((carry - rate) * maturity) * BlackScholesFns.CummulativeNormal(d1) * Math.Sqrt(maturity);
    }

    public double ImpliedVol(OptionType optionType, double spot, double strike, double rate, double carry, double maturity, double price)
    {
        double low = 0.0;
        double high = 4.0;
        if (PV(optionType, spot, strike, rate, carry, maturity, high) < price) return high;
        if (PV(optionType, spot, strike, rate, carry, maturity, low) > price) return low;

        double vol = (high + low) * 0.5; // 2.0
        int count = 0;
        while (vol - low > 0.0001 && count < 100_000)
        {
            double impliedPrice = PV(optionType, spot, strike, rate, carry, maturity, vol);
            if (impliedPrice < price)
                low = vol;
            else if (impliedPrice > price)
                high = vol;
            vol = (high + low) * 0.5;
            count++;
        }
        return vol;
    }

}
