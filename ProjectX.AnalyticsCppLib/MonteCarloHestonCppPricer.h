#pragma once
using namespace System;

namespace ProjectXAnalyticsCppLib
{
	public ref struct HestonStochasticVolalityParameters
	{
	public:
		HestonStochasticVolalityParameters(Double initialVolatility, Double longTermVolatility, Double speedOfReversion, Double volOfVol, Double correlation)
		{
			this->InitialVolatility = initialVolatility;
			this->LongTermVolatility = longTermVolatility;
			this->SpeedOfReversion = speedOfReversion;
			this->VolOfVol = volOfVol;
			this->Correlation = correlation;
		}
	public:
		Double InitialVolatility;  // v0
		Double LongTermVolatility; // theta
		Double SpeedOfReversion;   // kappa
		Double VolOfVol;		   // xi
		Double Correlation;		   // rho correlation between W1 and W2 brownian motions under the risk-neutral probability measure
	};

	public ref class MonteCarloHestonCppPricer 
	{
	public:
		GreekResults ProjectXAnalyticsCppLib::MonteCarloHestonCppPricer::MCValue(
			VanillaOptionParameters^% TheOption,
			Double spotInitial,
			Double interestRate,
			Double dividendYield,
			UInt64 numberOfSteps,
			UInt64 numberOfSimulations,
			HestonStochasticVolalityParameters^% volParams);
	};
}