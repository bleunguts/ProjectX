#include "pch.h"
#include "BlackScholesFunctions.h"

// Black-Scholes formula to calculate the option price
Double ProjectXAnalyticsCppLib::BlackScholesFunctions::BlackScholes(
	Double S, // Current stock price
	Double K, // Strike price
	Double r, // Risk-free interest rate
	Double T, // Time to expiration
	Double sigma // Volatility
)
{
	Double d1 = (Math::Log(S / K) + (r + (Math::Pow(sigma, 2) / 2)) * T) / (sigma * Math::Sqrt(T));
	Double d2 = d1 - sigma * Math::Sqrt(T);

	Double N_d1 = normcdf(d1);
	Double N_d2 = normcdf(d2);

	Double optionPrice = S * N_d1 - K * Math::Exp(-r * T) * N_d2;
	Double putOptionPrice = -S * N_d1 + K * Math::Exp(-r * T) * N_d2;

	return optionPrice;
}

Double ProjectXAnalyticsCppLib::BlackScholesFunctions::BlackScholesDelta(
	Double S, // Current stock price
	Double K, // Strike price
	Double r, // Risk-free interest rate
	Double T, // Time to expiration			
	Double optionPrice, // Option Price
	Double epsilon, // Small change in stock price
	Double sigma // Volatility
)
{
	double d1 = (Math::Log(S / K) + (r + (Math::Pow(sigma, 2) / 2)) * T) / (sigma * Math::Sqrt(T));
	double delta = Math::Exp(-r * T) * normcdf(d1);

	return delta * optionPrice + (optionPrice - delta * optionPrice) / (S + epsilon - K) * (S - K);
}

// Black-Scholes formula to calculate the option price
Double ProjectXAnalyticsCppLib::BlackScholesFunctions::BlackScholesGamma(
	Double S, // Current stock price
	Double K, // Strike price
	Double r, // Risk-free interest rate
	Double T, // Time to expiration
	Double sigma, // Volatility
	Double epsilon // Small change in stock price
)
{
	double optionPrice = BlackScholes(S, K, r, T, sigma);

	double optionPriceUp = BlackScholes(S + epsilon, K, r, T, sigma);
	double optionPriceDown = BlackScholes(S - epsilon, K, r, T, sigma);

	double gamma = (optionPriceUp - 2 * optionPrice + optionPriceDown) / (epsilon * epsilon);

	return gamma;
}

Double ProjectXAnalyticsCppLib::BlackScholesFunctions::BlackScholesRho(
	Double S, // Current stock price
	Double K, // Strike price
	Double r, // Risk-free interest rate
	Double T, // Time to expiration
	Double sigma // Volatility
)
{
	double d2 = (Math::Log(S / K) + (r - (Math::Pow(sigma, 2) / 2)) * T) / (sigma * Math::Sqrt(T));
	return T * K * Math::Exp(-r * T) * normcdf(d2);
}

Double ProjectXAnalyticsCppLib::BlackScholesFunctions::BlackScholesTheta(
	Double S, // Current stock price
	Double K, // Strike price
	Double r, // Risk-free interest rate
	Double T, // Time to expiration
	Double sigma // Volatility
)
{
	double d1 = (Math::Log(S / K) + (r + (Math::Pow(sigma, 2) / 2)) * T) / (sigma * Math::Sqrt(T));
	double d2 = d1 - sigma * Math::Sqrt(T);

	double N_d1 = normcdf(d1);
	double N_d2 = normcdf(d2);
	double N_prime_d1 = normpdf(d1);

	double theta = -((S * N_prime_d1 * sigma) / (2.0 * Math::Sqrt(T))) - r * K * Math::Exp(-r * T) * N_d2;

	return theta;
}

// Function to calculate the Black-Scholes Vega
Double ProjectXAnalyticsCppLib::BlackScholesFunctions::BlackScholesVega(
	Double S, // Current stock price
	Double K, // Strike price
	Double r, // Risk-free interest rate
	Double T, // Time to expiration
	Double sigma // Volatility
)
{
	double d1 = (Math::Log(S / K) + (r + (Math::Pow(sigma, 2) / 2)) * T) / (sigma * Math::Sqrt(T));

	double N_prime_d1 = normpdf(d1);

	double vega = S * Math::Sqrt(T) * N_prime_d1;

	return vega;
}

