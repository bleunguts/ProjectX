﻿#include "pch.h"
#include "BlackScholesFunctions.h"

Double ProjectXAnalyticsCppLib::BlackScholesFunctions::d1(double S, double K, double r, double sigma, double T) {
	//d1 = ( ln(SP/ST) + (r - d + (σ2/2)) t ) / σ √t	
	double sigmasquared = Math::Pow(sigma, 2);
	double logChunk = Math::Log(S / K);
	double rhsnumerator = (r + (sigmasquared / 2)) * T;
	double numerator = logChunk + rhsnumerator;
	double denom = (sigma * Math::Sqrt(T));
	double d1 = numerator / denom;
	return d1;
}

Double ProjectXAnalyticsCppLib::BlackScholesFunctions::d2(double d1, double sigma, double T) {
	//d2 = ( ln(SP/ST) + (r - d - (σ2/2)) t ) / σ √t = d1 - σ √t
	Double d2 = d1 - sigma * Math::Sqrt(T);
	return d2;
}

// Black-Scholes formula to calculate the option price
Double ProjectXAnalyticsCppLib::BlackScholesFunctions::BlackScholesCall(
	Double S, // Current stock price
	Double K, // Strike price
	Double r, // Risk-free interest rate
	Double T, // Time to expiration
	Double sigma // Volatility
)
{	
	//C = SP e-dt N(d1) - ST e-rt N(d2)

	Double d1 = BlackScholesFunctions::d1(S, K, r, sigma, T);
	Double d2 = BlackScholesFunctions::d2(d1, sigma, T);

	Double N_d1 = normcdf(d1);
	Double N_d2 = normcdf(d2);

	Double optionPrice = S * N_d1 - K * Math::Exp(-r * T) * N_d2;
	return optionPrice;
}

// Black-Scholes formula to calculate the option price
Double ProjectXAnalyticsCppLib::BlackScholesFunctions::BlackScholesPut(
	Double S, // Current stock price
	Double K, // Strike price
	Double r, // Risk-free interest rate
	Double T, // Time to expiration
	Double sigma // Volatility
)
{
	//P = ST e-rt N(-d2) - SP e-dt N(-d1)

	Double d1 = BlackScholesFunctions::d1(S, K, r, sigma, T);
	Double d2 = BlackScholesFunctions::d2(d1, sigma, T);

	Double Neg_N_d1 = normcdf(-d1);
	Double Neg_N_d2 = normcdf(-d2);

	Double putOptionPrice = K * Math::Exp(-r * T) * Neg_N_d2 - S * Neg_N_d1;

	return putOptionPrice;
}

Double ProjectXAnalyticsCppLib::BlackScholesFunctions::BlackScholesDeltaCall(
	Double S, // Current stock price
	Double K, // Strike price
	Double r, // Risk-free interest rate
	Double T, // Time to expiration				
	Double sigma // Volatility
)
{
	Double q = 0;
	Double d1 = BlackScholesFunctions::d1(S,K,r,sigma,T);
	double delta = Math::Exp(-q * T) * normcdf(d1);
	return delta;
}

Double ProjectXAnalyticsCppLib::BlackScholesFunctions::BlackScholesDeltaPut(
	Double S, // Current stock price
	Double K, // Strike price
	Double r, // Risk-free interest rate
	Double T, // Time to expiration					
	Double sigma // Volatility
)
{
	Double q = 0;
	Double d1 = BlackScholesFunctions::d1(S, K, r, sigma, T);
	double deltaPut = Math::Exp(-q * T) * (normcdf(d1) - 1);
	return deltaPut;
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
	Double q = 0; 
	Double d1 = BlackScholesFunctions::d1(S, K, r, sigma, T);	
	Double N_d1 = normcdf(d1);
	Double NPrime_d1 = normpdf(d1);
	double gamma = Math::Exp(-q * T) / (S * sigma * Math::Sqrt(T)) * NPrime_d1;
	return gamma;
}

Double ProjectXAnalyticsCppLib::BlackScholesFunctions::BlackScholesRhoCall(
	Double S, // Current stock price
	Double K, // Strike price
	Double r, // Risk-free interest rate
	Double T, // Time to expiration
	Double sigma // Volatility
)
{
	Double d1 = BlackScholesFunctions::d1(S, K, r, sigma, T);
	Double d2 = BlackScholesFunctions::d2(d1, sigma, T);
	Double N_d2 = normcdf(d2);
	double rho = (K * T * Math::Exp(-r * T) * N_d2) / 100;
	return rho;
}

Double ProjectXAnalyticsCppLib::BlackScholesFunctions::BlackScholesRhoPut(
	Double S, // Current stock price
	Double K, // Strike price
	Double r, // Risk-free interest rate
	Double T, // Time to expiration
	Double sigma // Volatility
)
{
	Double d1 = BlackScholesFunctions::d1(S, K, r, sigma, T);
	Double d2 = BlackScholesFunctions::d2(d1, sigma, T);
	Double NNegative_d2 = normcdf(-d2);
	double rho = - (K * T * Math::Exp(-r * T) * NNegative_d2);
	return rho / 100;
}

Double ProjectXAnalyticsCppLib::BlackScholesFunctions::BlackScholesThetaCall(
	Double S, // Current stock price
	Double K, // Strike price
	Double r, // Risk-free interest rate
	Double T, // Time to expiration
	Double sigma // Volatility
)
{
	double q = 0;
	Double d1 = BlackScholesFunctions::d1(S,K,r,sigma,T);
	Double d2 = BlackScholesFunctions::d2(d1, sigma, T);
	
	double N_d1 = normcdf(d1);
	double N_d2 = normcdf(d2);
	double N_prime_d1 = normpdf(d1);

	double p1 = - (S * sigma * Math::Exp(-q * T)/(2 * Math::Sqrt(T)) * N_prime_d1);
	double p2 = -r * K * Math::Exp(-r * T) * N_d2;
	double p3 = q * S * Math::Exp(-q * T) * N_d1;

	double theta = p1 + p2 + p3;
	return theta;
}

Double ProjectXAnalyticsCppLib::BlackScholesFunctions::BlackScholesThetaPut(
	Double S, // Current stock price
	Double K, // Strike price
	Double r, // Risk-free interest rate
	Double T, // Time to expiration
	Double sigma // Volatility
)
{
	double q = 0;
	Double d1 = BlackScholesFunctions::d1(S, K, r, sigma, T);
	Double d2 = BlackScholesFunctions::d2(d1, sigma, T);

	double NNegative_d1 = normcdf(-d1);
	double NNegative_d2 = normcdf(-d2);
	double N_prime_d1 = normpdf(d1);

	double p1 = -(S * sigma * Math::Exp(-q * T)) / (2 * Math::Sqrt(T)) * N_prime_d1;
	double p2 = r * K * Math::Exp(-r * T) * NNegative_d2;
	double p3 = -q * S * Math::Exp(-q * T) * NNegative_d1;

	double theta = p1 + p2 + p3;
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
	double q = 0;
	Double d1 = BlackScholesFunctions::d1(S,K,r,sigma,T);
	double NPrime_d1 = normpdf(d1);

	double vega = S * Math::Exp(-q * T) * Math::Sqrt(T) * NPrime_d1;
	return vega / 100;
}

