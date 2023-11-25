#pragma once
#include "VanillaOption.h"
#include "RandomWalk.h"
#include "PayOff.h"
#include "Parameters.h"
#include "BlackScholesFunctions.h"
#include "ProjectX.AnalyticsCppLib.h"

namespace ProjectXAnalyticsCppLib 
{
	class MonteCarloCppPricer : IMonteCarloCppPricer
	{
	private:
		RandomWalk& m_randomWalk;
	public:
		MonteCarloCppPricer(RandomWalk& randomWalk) 
			: m_randomWalk(randomWalk)
		{			
		};
		virtual GreekResults MCValue(
			VanillaOptionParameters& TheOption,
			double Spot,
			double Vol,
			double r,
			unsigned int NumberOfPaths);						
		virtual double ImpliedVolatilityMC(
			VanillaOptionParameters& TheOption,
			double Spot,
			double r,
			unsigned int NumberOfPaths,
			double optionPrice
		);
	private:
		double BlackScholes_DeltaCall(double S, double K, double r, double q, double sigma, double t);
		double BlackScholes_DeltaPut(double S, double K, double r, double q, double sigma, double t);
		double BlackScholes_Gamma(double S, double K, double r, double q, double sigma, double t);
		double BlackScholes_Vega(double S, double K, double r, double q, double sigma, double t);
		double BlackScholes_RhoCall(double S, double K, double r, double q, double sigma, double t);
		double BlackScholes_RhoPut(double S, double K, double r, double q, double sigma, double t);
		double BlackScholes_ThetaCall(double S, double K, double r, double q, double sigma, double t);
		double BlackScholes_ThetaPut(double S, double K, double r, double q, double sigma, double t);			
	};
}