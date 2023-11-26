#include "pch.h"
#include "BlackScholesCppPricer.h"

double ProjectXAnalyticsCppLib::BlackScholesCppPricer::Value(
	VanillaOptionParameters& TheOption,
	double Spot,
	double Vol,
	double r)
{
	if (TheOption.OptionType() == OptionType::Call)
		return BlackScholesFunctions::BlackScholesCall(Spot, TheOption.Strike(), r, TheOption.Expiry(), Vol);
	if (TheOption.OptionType() == OptionType::Put)
		return BlackScholesFunctions::BlackScholesPut(Spot, TheOption.Strike(), r, TheOption.Expiry(), Vol);

	throw "Not supported option type";
}

double ProjectXAnalyticsCppLib::BlackScholesCppPricer::Delta(VanillaOptionParameters& TheOption,
	double Spot,
	double Vol,
	double r
)
{
	double epsilon = 0.01; // Small change in stock price	
	double optionPrice = Value(TheOption, Spot, Vol, r);

	// Calculate delta using Black-Scholes formula	
	if (TheOption.OptionType() == OptionType::Call)
		return BlackScholesFunctions::BlackScholesDeltaCall(Spot, TheOption.Strike(), r, TheOption.Expiry(), Vol);
	if (TheOption.OptionType() == OptionType::Put)		return BlackScholesFunctions::BlackScholesDeltaPut(Spot, TheOption.Strike(), r, TheOption.Expiry(), Vol);

	throw "Not supported option type";
}

double ProjectXAnalyticsCppLib::BlackScholesCppPricer::Gamma(VanillaOptionParameters& TheOption,
	double Spot,
	double Vol,
	double r,
	double epsilon
)
{
	double gamma = BlackScholesFunctions::BlackScholesGamma(Spot, TheOption.Strike(), r, TheOption.Expiry(), Vol, epsilon);
	return gamma;
}

double ProjectXAnalyticsCppLib::BlackScholesCppPricer::Rho(VanillaOptionParameters& TheOption,
	double Spot,
	double Vol,
	double r
)
{
	if (TheOption.OptionType() == OptionType::Call)
		return BlackScholesFunctions::BlackScholesRhoCall(Spot, TheOption.Strike(), r, TheOption.Expiry(), Vol);
	if (TheOption.OptionType() == OptionType::Put)
		return BlackScholesFunctions::BlackScholesRhoPut(Spot, TheOption.Strike(), r, TheOption.Expiry(), Vol);

	throw "Not supported option type";
}

double ProjectXAnalyticsCppLib::BlackScholesCppPricer::Theta(VanillaOptionParameters& TheOption,
	double Spot,
	double Vol,
	double r
)
{
	if (TheOption.OptionType() == OptionType::Call)
		return BlackScholesFunctions::BlackScholesThetaCall(Spot, TheOption.Strike(), r, TheOption.Expiry(), Vol);
	if (TheOption.OptionType() == OptionType::Put)
		return BlackScholesFunctions::BlackScholesThetaPut(Spot, TheOption.Strike(), r, TheOption.Expiry(), Vol);

	throw "Not supported option type";
}

double ProjectXAnalyticsCppLib::BlackScholesCppPricer::Vega(VanillaOptionParameters& TheOption,
	double Spot,
	double Vol,
	double r
)
{
	return BlackScholesFunctions::BlackScholesVega(Spot, TheOption.Strike(), r, TheOption.Expiry(), Vol);
}

double ProjectXAnalyticsCppLib::BlackScholesCppPricer::ImpliedVolatility(
	VanillaOptionParameters& TheOption,
	double Spot,
	double r,
	double optionPrice
)
{
	double S = Spot;
	double T = TheOption.Expiry();
	double K = TheOption.Strike();
	double epsilon = 1e-6;  // Tolerance for convergence
	double sigma = 0.2;     // Initial guess for implied volatility
	int maxIterations = 100;  // Maximum number of iterations = 100

	for (int i = 0; i < maxIterations; i++)
	{
		double price = BlackScholesFunctions::BlackScholesCall(S, K, r, T, sigma);
		double vega = BlackScholesFunctions::BlackScholesVega(S, K, r, T, sigma);
		double diff = price - optionPrice;

		if (Math::Abs(diff) < epsilon)
		{
			return sigma;
		}

		sigma -= diff / vega;
	}

	// If the maximum number of iterations is reached, return NaN (no solution found).
	return NAN;
}