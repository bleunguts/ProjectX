#pragma once
#include "pch.h"

namespace ProjectXAnalyticsCppLib
{
	class BlackScholesCppPricer 
	{
	public:
		double Value(
			VanillaOptionParameters& TheOption,
			double Spot,
			double Vol,
			double r);
		double Delta(
			VanillaOptionParameters& TheOption,
			double Spot,
			double Vol,
			double r
		);
		double Gamma(
			VanillaOptionParameters& TheOption,
			double Spot,
			double Vol,
			double r,
			double epsilon
		);
		double Vega(
			VanillaOptionParameters& TheOption,
			double Spot,
			double Vol,
			double r
		);
		double Theta(
			VanillaOptionParameters& TheOption,
			double Spot,
			double Vol,
			double r
		);
		double Rho(
			VanillaOptionParameters& TheOption,
			double Spot,
			double Vol,
			double r
		);
		double ImpliedVolatility(
			VanillaOptionParameters& TheOption,
			double Spot,
			double r,
			double optionPrice
		);

	};
}