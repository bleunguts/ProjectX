#pragma once
#include "MonteCarloHestonCppPricer.h"
using namespace System;

namespace ProjectXAnalyticsCppLib
{
	public ref class MonteCarloHestonCppPricer2
	{
	public:
		GreekResults ProjectXAnalyticsCppLib::MonteCarloHestonCppPricer2::MCValue(
			VanillaOptionParameters^% TheOption,
			Double spotInitial,
			Double interestRate,
			Double dividendYield,
			UInt64 numberOfSteps,
			UInt64 numberOfSimulations,
			HestonStochasticVolalityParameters^% volParams);
	private:
		void ProjectXAnalyticsCppLib::MonteCarloHestonCppPricer2::Heston_VolPath(const vector<double>& dw_v, vector<double>& v_t, double T, double Kappa, double Theta, double Epslon);
		void ProjectXAnalyticsCppLib::MonteCarloHestonCppPricer2::Heston_SpotPath(const vector<double>& dw_i, const vector<double>& v_t, vector<double>& S_t, double T, double r);
	};
}