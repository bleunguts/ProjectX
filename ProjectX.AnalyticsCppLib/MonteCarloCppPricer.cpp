#include "pch.h"
#include "MonteCarloCppPricer.h"
#include "Parameters.h"
#include "BlackScholesFunctions.h"
#include <cmath>

GreekResults^ ProjectXAnalyticsCppLib::MonteCarloCppPricer::MCValue(VanillaOptionParameters^% OptionParams,
	Double Spot,
	Double Vol,
	Double r,
	UInt64 NumberOfPaths)
{
	double K = OptionParams->Strike();
	double T = OptionParams->Expiry();
	PayOffCall call = PayOffCall(K);
	PayOffPut put = PayOffPut(K);
	PayOffBridge callBridge = PayOffBridge(call);
	PayOffBridge putBridge = PayOffBridge(put);
	VanillaOption callOption = VanillaOption(callBridge, T);
	VanillaOption putOption = VanillaOption(putBridge, T);

	ParametersConstant vol = ParametersConstant(Vol);
	ParametersConstant rate = ParametersConstant(r);
	double variance = vol.IntegralSquare(0, T);
	double rootVariance = sqrt(variance);
	double itoCorrection = -0.5 * variance;
	double movedSpot = Spot * exp(rate.Integral(0, T) + itoCorrection);
	double thisSpot;
	
	double dt = T / NumberOfPaths;
	double q = 0;
	double sum_payoffs = 0.0;	
	double sum_payoffsput = 0.0;	
	double sum_delta = 0.0;
	double sum_deltaput = 0.0;
	double sum_gamma = 0.0;	
	double sum_vega = 0.0;
	double sum_rho = 0.0;
	double sum_rhoput = 0.0;
	double sum_theta = 0.0;
	double sum_thetaput = 0.0;	

	for (unsigned long i = 0; i < NumberOfPaths; i++)
	{
		// PVs
		double thisGaussian = m_randomWalk->GetOneGaussian();
		thisSpot = movedSpot * exp(rootVariance * thisGaussian);		
		sum_payoffs += callOption.OptionPayOff(thisSpot);
		sum_payoffsput += putOption.OptionPayOff(thisSpot);

		// Greeks
		double step = T - i * dt;		
		sum_delta += BlackScholes_DeltaCall(thisSpot, K, r, q, Vol, step);;		
		sum_deltaput += BlackScholes_DeltaPut(thisSpot, K, r, q, Vol, step);		
		sum_gamma += BlackScholes_Gamma(thisSpot, K, r, q, Vol, step);		
		sum_vega += BlackScholes_Vega(thisSpot, K, r, q, Vol, step);
		sum_rho += BlackScholes_RhoCall(thisSpot, K, r, q, Vol, step);				
		sum_rhoput += BlackScholes_RhoPut(thisSpot, K, r, q, Vol, step);		
		sum_theta += BlackScholes_ThetaCall(thisSpot, K, r, q, Vol, step);								
		sum_thetaput += BlackScholes_ThetaPut(thisSpot, K, r, q, Vol, step);;
	}

	double mean = sum_payoffs / NumberOfPaths * exp(-rate.Integral(0, T));	
	double meanPut = sum_payoffsput / NumberOfPaths * exp(-rate.Integral(0, T));
	double delta = sum_delta / NumberOfPaths;	
	double gamma = sum_gamma / NumberOfPaths;
	double vega = sum_vega / NumberOfPaths;
	double rho = sum_rho / NumberOfPaths;
	double theta = sum_theta / NumberOfPaths;
	double delta_put = sum_deltaput / NumberOfPaths;
	double rho_put = sum_rhoput / NumberOfPaths;
	double theta_put = sum_thetaput / NumberOfPaths;		
	
	GreekResults^ results = gcnew GreekResults(mean, meanPut, delta, delta_put, gamma, vega, rho, rho_put, theta, theta_put);
	return results;
}
	
Double ProjectXAnalyticsCppLib::MonteCarloCppPricer::BlackScholes_DeltaCall(double S, double K, double r, double q, double sigma, double t) 
{
	double d1 = BlackScholesFunctions::d1(S, K, r, sigma, t);
	Double N_d1 = normcdf(d1);
	return Math::Exp(-q * t) * N_d1;
}

Double ProjectXAnalyticsCppLib::MonteCarloCppPricer::BlackScholes_DeltaPut(double S, double K, double r, double q, double sigma, double t)
{
	double d1 = BlackScholesFunctions::d1(S, K, r, sigma, t);	
	Double N_d1 = normcdf(d1);
	return Math::Exp(-q * t)* (N_d1 - 1);
}

Double ProjectXAnalyticsCppLib::MonteCarloCppPricer::BlackScholes_Gamma(double S, double K, double r, double q, double sigma, double t)
{
	double d1 = BlackScholesFunctions::d1(S, K, r, sigma, t);
	Double NPrime_d1 = normpdf(d1);
	return Math::Exp(-q * t) / (S * sigma * Math::Sqrt(t)) * NPrime_d1;
}

Double ProjectXAnalyticsCppLib::MonteCarloCppPricer::BlackScholes_Vega(double S, double K, double r, double q, double sigma, double t)
{
	double d1 = BlackScholesFunctions::d1(S, K, r, sigma, t);
	Double NPrime_d1 = normpdf(d1);
	return S * Math::Exp(-q * t) * Math::Sqrt(t) * NPrime_d1 / 100;
}

Double ProjectXAnalyticsCppLib::MonteCarloCppPricer::BlackScholes_RhoCall(double S, double K, double r, double q, double sigma, double t)
{
	double d1 = BlackScholesFunctions::d1(S, K, r, sigma, t);
	Double d2 = BlackScholesFunctions::d2(d1, sigma, t);
	Double N_d2 = normcdf(d2);
	double rho = (K * t * Math::Exp(-r * t) * N_d2) / 100;
	return rho;
}

Double ProjectXAnalyticsCppLib::MonteCarloCppPricer::BlackScholes_RhoPut(double S, double K, double r, double q, double sigma, double t)
{
	double d1 = BlackScholesFunctions::d1(S, K, r, sigma, t);
	Double d2 = BlackScholesFunctions::d2(d1, sigma, t);
	Double NNegative_d2 = normcdf(-d2);
	double rhoPut = -(K * t * Math::Exp(-r * t) * NNegative_d2) / 100;
	return rhoPut;
}

Double ProjectXAnalyticsCppLib::MonteCarloCppPricer::BlackScholes_ThetaCall(double S, double K, double r, double q, double sigma, double t)
{
	double d1 = BlackScholesFunctions::d1(S, K, r, sigma, t);
	Double d2 = BlackScholesFunctions::d2(d1, sigma, t);
	Double N_d1 = normcdf(d1);
	Double N_d2 = normcdf(d2);
	Double NPrime_d1 = normpdf(d1);

	double p1 = -(S * sigma * Math::Exp(-q * t) / (2 * Math::Sqrt(t)) * NPrime_d1);
	double p2 = -r * K * Math::Exp(-r * t) * N_d2;
	double p3 = q * S * Math::Exp(-q * t) * N_d1;
	double theta = p1 + p2 + p3;
	return theta;
}

Double ProjectXAnalyticsCppLib::MonteCarloCppPricer::BlackScholes_ThetaPut(double S, double K, double r, double q, double sigma, double t)
{
	double d1 = BlackScholesFunctions::d1(S, K, r, sigma, t);
	Double d2 = BlackScholesFunctions::d2(d1, sigma, t);	
	Double NPrime_d1 = normpdf(d1);
	double NNegative_d1 = normcdf(-d1);
	Double NNegative_d2 = normcdf(-d2);

	double p1 = -(S * sigma * Math::Exp(-q * t) / (2 * Math::Sqrt(t)) * NPrime_d1);
	double p2 = r * K * Math::Exp(-r * t) * NNegative_d2;
	double p3 = -q * S * Math::Exp(-q * t) * NNegative_d1;
	double thetaPut = p1 + p2 + p3;
	return thetaPut;
}

// Function to calculate implied volatility using a Monte Carlo simulation
Double ProjectXAnalyticsCppLib::MonteCarloCppPricer::ImpliedVolatilityMC(VanillaOptionParameters^% TheOption,
	Double Spot,
	Double r,
	UInt64 NumberOfPaths,
	Double optionPrice)
{
	double T = TheOption->Expiry();
	double K = TheOption->Strike();
	double S = Spot;
	double sigma = 0.2; // Initial guess for implied volatility
	double epsilon = 0.01; // Small change in volatility
	double vega, price;

	for (int i = 0; i < NumberOfPaths; i++)
	{
		double randNorm = m_randomWalk->GetOneGaussian();
		double ST = S * Math::Exp((r - (Math::Pow(sigma, 2) / 2)) * T + sigma * Math::Sqrt(T) * randNorm);
		double payoff = Math::Max(ST - K, 0.0);
		price = Math::Exp(-r * T) * payoff;

		vega = (price - optionPrice) / epsilon;

		if (Math::Abs(price - optionPrice) < epsilon)
		{
			return sigma;
		}

		sigma -= (price - optionPrice) / vega;
	}

	// If the maximum number of simulations is reached, return NaN (no solution found).
	return Double::NaN;
}
