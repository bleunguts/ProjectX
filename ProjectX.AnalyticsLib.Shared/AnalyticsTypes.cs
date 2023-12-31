﻿using ProjectX.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectX.AnalyticsLib.Shared;

public interface IMonteCarloHestonCppOptionsPricer : IOptionsGreeksCalculator
{

}
public interface IMonteCarloCppOptionsPricer : IOptionsGreeksCalculator
{
}
public interface IBlackScholesCSharpPricer : IOptionsGreeksCalculator
{
}
public interface IBlackScholesCppPricer : IOptionsGreeksCalculator
{
}
public interface IOptionsGreeksCalculator
{
    double PV(OptionType optionType, double spot, double strike, double rate, double carry, double maturity, double volatility);
    double Delta(OptionType optionType, double spot, double strike, double rate, double carry, double maturity, double volatility);
    double Gamma(OptionType optionType, double spot, double strike, double rate, double carry, double maturity, double volatility);
    double ImpliedVol(OptionType optionType, double spot, double strike, double rate, double carry, double maturity, double price);
    double Rho(OptionType optionType, double spot, double strike, double rate, double carry, double maturity, double volatility);
    double Theta(OptionType optionType, double spot, double strike, double rate, double carry, double maturity, double volatility);
    double Vega(OptionType optionType, double spot, double strike, double rate, double carry, double maturity, double volatility);
}
