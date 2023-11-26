#pragma once
#include "pch.h"

using namespace std;

namespace ProjectXAnalyticsCppLib
{	
	class PROJECT_API MonteCarloHestonCppPricer
	{
	public:
		GreekResults MCValue(
			VanillaOptionParameters& TheOption,
			double spotInitial,
			double interestRate,
			double dividendYield,
			unsigned int numberOfSteps,
			unsigned int numberOfSimulations,
			HestonStochasticVolalityParameters& volParams);
	};
}