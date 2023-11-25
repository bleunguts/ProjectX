#include "pch.h"
#include "BlackScholesFunctions.h"

double ProjectXAnalyticsCppLib::BlackScholesFunctions::d1(double S, double K, double r, double sigma, double T) {
	//d1 = ( ln(SP/ST) + (r - d + (σ2/2)) t ) / σ √t	
	double sigmasquared = Math::Pow(sigma, 2);
	double logChunk = Math::Log(S / K);
	double rhsnumerator = (r + (sigmasquared / 2)) * T;
	double numerator = logChunk + rhsnumerator;
	double denom = (sigma * Math::Sqrt(T));
	double d1 = numerator / denom;
	return d1;
}

double ProjectXAnalyticsCppLib::BlackScholesFunctions::d2(double d1, double sigma, double T) {
	//d2 = ( ln(SP/ST) + (r - d - (σ2/2)) t ) / σ √t = d1 - σ √t
	double d2 = d1 - sigma * Math::Sqrt(T);
	return d2;
}

// Black-Scholes formula to calculate the option price
double ProjectXAnalyticsCppLib::BlackScholesFunctions::BlackScholesCall(
	double S, // Current stock price
	double K, // Strike price
	double r, // Risk-free interest rate
	double T, // Time to expiration
	double sigma // Volatility
)
{	
	//C = SP e-dt N(d1) - ST e-rt N(d2)

	double d1 = BlackScholesFunctions::d1(S, K, r, sigma, T);
	double d2 = BlackScholesFunctions::d2(d1, sigma, T);

	double N_d1 = normcdf(d1);
	double N_d2 = normcdf(d2);

	double optionPrice = S * N_d1 - K * Math::Exp(-r * T) * N_d2;
	return optionPrice;
}

// Black-Scholes formula to calculate the option price
double ProjectXAnalyticsCppLib::BlackScholesFunctions::BlackScholesPut(
	double S, // Current stock price
	double K, // Strike price
	double r, // Risk-free interest rate
	double T, // Time to expiration
	double sigma // Volatility
)
{
	//P = ST e-rt N(-d2) - SP e-dt N(-d1)

	double d1 = BlackScholesFunctions::d1(S, K, r, sigma, T);
	double d2 = BlackScholesFunctions::d2(d1, sigma, T);

	double Neg_N_d1 = normcdf(-d1);
	double Neg_N_d2 = normcdf(-d2);

	double putOptionPrice = K * Math::Exp(-r * T) * Neg_N_d2 - S * Neg_N_d1;

	return putOptionPrice;
}

double ProjectXAnalyticsCppLib::BlackScholesFunctions::BlackScholesDeltaCall(
	double S, // Current stock price
	double K, // Strike price
	double r, // Risk-free interest rate
	double T, // Time to expiration				
	double sigma // Volatility
)
{
	double q = 0;
	double d1 = BlackScholesFunctions::d1(S,K,r,sigma,T);
	double delta = Math::Exp(-q * T) * normcdf(d1);
	return delta;
}

double ProjectXAnalyticsCppLib::BlackScholesFunctions::BlackScholesDeltaPut(
	double S, // Current stock price
	double K, // Strike price
	double r, // Risk-free interest rate
	double T, // Time to expiration					
	double sigma // Volatility
)
{
	double q = 0;
	double d1 = BlackScholesFunctions::d1(S, K, r, sigma, T);
	double deltaPut = Math::Exp(-q * T) * (normcdf(d1) - 1);
	return deltaPut;
}

// Black-Scholes formula to calculate the option price
double ProjectXAnalyticsCppLib::BlackScholesFunctions::BlackScholesGamma(
	double S, // Current stock price
	double K, // Strike price
	double r, // Risk-free interest rate
	double T, // Time to expiration
	double sigma, // Volatility
	double epsilon // Small change in stock price
)
{
	double q = 0; 
	double d1 = BlackScholesFunctions::d1(S, K, r, sigma, T);	
	double N_d1 = normcdf(d1);
	double NPrime_d1 = normpdf(d1);
	double gamma = Math::Exp(-q * T) / (S * sigma * Math::Sqrt(T)) * NPrime_d1;
	return gamma;
}

double ProjectXAnalyticsCppLib::BlackScholesFunctions::BlackScholesRhoCall(
	double S, // Current stock price
	double K, // Strike price
	double r, // Risk-free interest rate
	double T, // Time to expiration
	double sigma // Volatility
)
{
	double d1 = BlackScholesFunctions::d1(S, K, r, sigma, T);
	double d2 = BlackScholesFunctions::d2(d1, sigma, T);
	double N_d2 = normcdf(d2);
	double rho = (K * T * Math::Exp(-r * T) * N_d2) / 100;
	return rho;
}

double ProjectXAnalyticsCppLib::BlackScholesFunctions::BlackScholesRhoPut(
	double S, // Current stock price
	double K, // Strike price
	double r, // Risk-free interest rate
	double T, // Time to expiration
	double sigma // Volatility
)
{
	double d1 = BlackScholesFunctions::d1(S, K, r, sigma, T);
	double d2 = BlackScholesFunctions::d2(d1, sigma, T);
	double NNegative_d2 = normcdf(-d2);
	double rho = - (K * T * Math::Exp(-r * T) * NNegative_d2);
	return rho / 100;
}

double ProjectXAnalyticsCppLib::BlackScholesFunctions::BlackScholesThetaCall(
	double S, // Current stock price
	double K, // Strike price
	double r, // Risk-free interest rate
	double T, // Time to expiration
	double sigma // Volatility
)
{
	double q = 0;
	double d1 = BlackScholesFunctions::d1(S,K,r,sigma,T);
	double d2 = BlackScholesFunctions::d2(d1, sigma, T);
	
	double N_d1 = normcdf(d1);
	double N_d2 = normcdf(d2);
	double N_prime_d1 = normpdf(d1);

	double p1 = - (S * sigma * Math::Exp(-q * T)/(2 * Math::Sqrt(T)) * N_prime_d1);
	double p2 = -r * K * Math::Exp(-r * T) * N_d2;
	double p3 = q * S * Math::Exp(-q * T) * N_d1;

	double theta = p1 + p2 + p3;
	return theta;
}

double ProjectXAnalyticsCppLib::BlackScholesFunctions::BlackScholesThetaPut(
	double S, // Current stock price
	double K, // Strike price
	double r, // Risk-free interest rate
	double T, // Time to expiration
	double sigma // Volatility
)
{
	double q = 0;
	double d1 = BlackScholesFunctions::d1(S, K, r, sigma, T);
	double d2 = BlackScholesFunctions::d2(d1, sigma, T);

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
double ProjectXAnalyticsCppLib::BlackScholesFunctions::BlackScholesVega(
	double S, // Current stock price
	double K, // Strike price
	double r, // Risk-free interest rate
	double T, // Time to expiration
	double sigma // Volatility
)
{
	double q = 0;
	double d1 = BlackScholesFunctions::d1(S,K,r,sigma,T);
	double NPrime_d1 = normpdf(d1);

	double vega = S * Math::Exp(-q * T) * Math::Sqrt(T) * NPrime_d1;
	return vega / 100;
}

