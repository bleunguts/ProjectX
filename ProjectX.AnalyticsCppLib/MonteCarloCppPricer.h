#pragma once
#include "VanillaOption.h"
#include "RandomWalk.h"

using namespace System;
using namespace ProjectXAnalyticsCppLib;

namespace ProjectXAnalyticsCppLib 
{
	public ref class MonteCarloCppPricer : IMonteCarloCppPricer
	{
	private:
		RandomWalk^ m_randomWalk;
	public:
		MonteCarloCppPricer(RandomWalk^ randomWalk)
		{
			m_randomWalk = randomWalk;
		};
		virtual GreekResults ProjectXAnalyticsCppLib::MonteCarloCppPricer::MCValue(
			VanillaOptionParameters^% TheOption,
			Double Spot,
			Double Vol,
			Double r,
			UInt64 NumberOfPaths);						
		virtual Double ProjectXAnalyticsCppLib::MonteCarloCppPricer::ImpliedVolatilityMC(
			VanillaOptionParameters^% TheOption,
			Double Spot,
			Double r,
			UInt64 NumberOfPaths,
			Double optionPrice
		);
	private:
		Double ProjectXAnalyticsCppLib::MonteCarloCppPricer::BlackScholes_DeltaCall(double S, double K, double r, double q, double sigma, double t);
		Double ProjectXAnalyticsCppLib::MonteCarloCppPricer::BlackScholes_DeltaPut(double S, double K, double r, double q, double sigma, double t);
		Double ProjectXAnalyticsCppLib::MonteCarloCppPricer::BlackScholes_Gamma(double S, double K, double r, double q, double sigma, double t);
		Double ProjectXAnalyticsCppLib::MonteCarloCppPricer::BlackScholes_Vega(double S, double K, double r, double q, double sigma, double t);
		Double ProjectXAnalyticsCppLib::MonteCarloCppPricer::BlackScholes_RhoCall(double S, double K, double r, double q, double sigma, double t);
		Double ProjectXAnalyticsCppLib::MonteCarloCppPricer::BlackScholes_RhoPut(double S, double K, double r, double q, double sigma, double t);
		Double ProjectXAnalyticsCppLib::MonteCarloCppPricer::BlackScholes_ThetaCall(double S, double K, double r, double q, double sigma, double t);
		Double ProjectXAnalyticsCppLib::MonteCarloCppPricer::BlackScholes_ThetaPut(double S, double K, double r, double q, double sigma, double t);			
	};
}