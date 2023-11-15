#pragma once
using namespace System;

namespace ProjectXAnalyticsCppLib
{
	public ref struct HestonStochasticVolalityParameters
	{
	public:
		HestonStochasticVolalityParameters(Double initialVolatility, Double longTermVolatility, Double speedOfReversion, Double volOfVol, IEnumerable<Double>^ rhoProbabilities, IEnumerable<Double>^ rhoChoices)
		{
			this->InitialVolatility = initialVolatility;
			this->LongTermVolatility = longTermVolatility;
			this->SpeedOfReversion = speedOfReversion;
			this->VolOfVol = volOfVol;			
			this->RhoProbabilities = rhoProbabilities;
			this->RhoChoices = rhoChoices;
		}
		HestonStochasticVolalityParameters(Double initialVolatility, Double longTermVolatility, Double speedOfReversion, Double volOfVol, Double rho)
		{
			this->InitialVolatility = initialVolatility;
			this->LongTermVolatility = longTermVolatility;
			this->SpeedOfReversion = speedOfReversion;
			this->VolOfVol = volOfVol;
			this->Rho = rho;			
		}
	public:
		Double InitialVolatility;  // v0
		Double LongTermVolatility; // theta
		Double SpeedOfReversion;   // kappa
		Double VolOfVol;		   // xi
		// rho correlation between W1 and W2 brownian motions under the risk-neutral probability measure
		// -0.7 is a standard correlation used in the Broadie paper
		IEnumerable<Double>^ RhoProbabilities; // rho probabilitiees 
		IEnumerable<Double>^ RhoChoices;
		Double Rho; // straight Rho value
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