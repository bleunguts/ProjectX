#include "pch.h"
#include "Parameters.h"
#include "ProjectX.AnalyticsCppLib.h"
#include "BlackScholesCppPricer.h"
#include "BlackScholesFunctions.h"
#include <cmath>

Double ProjectXAnalyticsCppLib::BlackScholesCppPricer::Value(
	VanillaOptionParameters^% TheOption,
	Double Spot,
	Double Vol,
	Double r)
{
	return BlackScholesFunctions::BlackScholes(Spot, TheOption->Strike(), r, TheOption->Expiry(), Vol);
}

Double ProjectXAnalyticsCppLib::BlackScholesCppPricer::Delta(VanillaOptionParameters^% TheOption,
	Double Spot,
	Double Vol,
	Double r	
)
{
	double epsilon = 0.01; // Small change in stock price	
	double optionPrice = Value(TheOption, Spot, Vol, r);

	// Calculate delta using Black-Scholes formula	
	double delta = BlackScholesFunctions::BlackScholesDelta(Spot, TheOption->Strike(), r, TheOption->Expiry(), optionPrice, epsilon, Vol);
	return delta;
}

Double ProjectXAnalyticsCppLib::BlackScholesCppPricer::Gamma(VanillaOptionParameters^% TheOption,
	Double Spot,
	Double Vol,
	Double r,
	Double epsilon
)
{
	double gamma = BlackScholesFunctions::BlackScholesGamma(Spot, TheOption->Strike(), r, TheOption->Expiry(), Vol, epsilon);
	return gamma;
}



Double ProjectXAnalyticsCppLib::BlackScholesCppPricer::Rho(VanillaOptionParameters^% TheOption,
	Double Spot,
	Double Vol,
	Double r
)
{
	double rho = BlackScholesFunctions::BlackScholesRho(Spot, TheOption->Strike(), r, TheOption->Expiry(), Vol);
	return rho;
}

Double ProjectXAnalyticsCppLib::BlackScholesCppPricer::Theta(VanillaOptionParameters^% TheOption,
	Double Spot,
	Double Vol,
	Double r	
)
{
	double theta = BlackScholesFunctions::BlackScholesTheta(Spot, TheOption->Strike(), r, TheOption->Expiry(), Vol);
	return theta;
}

Double ProjectXAnalyticsCppLib::BlackScholesCppPricer::Vega(VanillaOptionParameters^% TheOption,
	Double Spot,
	Double Vol,
	Double r
)
{
	double vega = BlackScholesFunctions::BlackScholesVega(Spot, TheOption->Strike(), r, TheOption->Expiry(), Vol);
	return vega;
}

Double ProjectXAnalyticsCppLib::BlackScholesCppPricer::ImpliedVolatility(
	VanillaOptionParameters^% TheOption,
	Double Spot,
	Double r,	
	Double optionPrice
)
{
	double S = Spot;
	double T = TheOption->Expiry();
	double K = TheOption->Strike();
	double epsilon = 1e-6;  // Tolerance for convergence
	double sigma = 0.2;     // Initial guess for implied volatility
	int maxIterations = 100;  // Maximum number of iterations = 100

	for (int i = 0; i < maxIterations; i++)
	{
		double price = BlackScholesFunctions::BlackScholes(S, K, r, T, sigma);
		double vega = BlackScholesFunctions::BlackScholesVega(S, K, r, T, sigma);
		double diff = price - optionPrice;

		if (Math::Abs(diff) < epsilon)
		{
			return sigma;
		}

		sigma -= diff / vega;
	}

	// If the maximum number of iterations is reached, return NaN (no solution found).
	return Double::NaN;
}