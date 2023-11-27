#pragma once
#include "pch.h"

namespace ProjectXAnalyticsCppLib
{
	class PROJECT_API MonteCarloHestonCppPricer2
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
	private:
		void Heston_VolPath(const vector<double>& dw_v, vector<double>& v_t, double T, double Kappa, double Theta, double Epslon);
		void Heston_SpotPath(const vector<double>& dw_i, const vector<double>& v_t, vector<double>& S_t, double T, double r);
	};
}