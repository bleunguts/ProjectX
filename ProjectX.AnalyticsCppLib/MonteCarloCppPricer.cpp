#include "pch.h"
#include "MonteCarloCppPricer.h"
#include "Parameters.h"
#include "BlackScholesFunctions.h"
#include <cmath>

ProjectXAnalyticsCppLib::GreekResults^ ProjectXAnalyticsCppLib::MonteCarloCppPricer::MCValue(VanillaOptionParameters^% OptionParams,
	Double Spot,
	Double Vol,
	Double r,
	UInt64 NumberOfPaths)
{
	double K = OptionParams->Strike();
	double T = OptionParams->Expiry();	
	double dt = T / NumberOfPaths;
	double q = 0;

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
		double callPayOff = callOption.OptionPayOff(thisSpot);
		double putPayOff = putOption.OptionPayOff(thisSpot);
		sum_payoffs += callPayOff;
		sum_payoffsput += putPayOff;

		// Greeks
		double step = T - i * dt;
		double d1 = BlackScholesFunctions::d1(thisSpot, K, r, Vol, step);
		Double d2 = BlackScholesFunctions::d2(d1, Vol, step);
		Double N_d1 = normcdf(d1);
		Double N_d2 = normcdf(d2);
		Double NPrime_d1 = normpdf(d1);
		double NNegative_d1 = normcdf(-d1);
		Double NNegative_d2 = normcdf(-d2);

		double delta_i = N_d1;
		sum_delta += delta_i;
		double deltaPut_i = Math::Exp(-q * step) * (N_d1 - 1);
		sum_deltaput += deltaPut_i;

		double gamma_i = Math::Exp(-q * step) / (thisSpot * Vol * Math::Sqrt(step)) * NPrime_d1;
		sum_gamma += gamma_i;

		double vega_i = thisSpot * Math::Exp(-q * step) * Math::Sqrt(step) * NPrime_d1 / 100;
		sum_vega += vega_i;

		double rho_i = (K * step * Math::Exp(-r * step) * N_d2) / 100;
		sum_rho += rho_i;		
		double rhoPut_i = -(K * step * Math::Exp(-r * step) * NNegative_d2) / 100;
		sum_rhoput += rhoPut_i;

		double p1 = -(thisSpot * Vol * Math::Exp(-q * step) / (2 * Math::Sqrt(step)) * NPrime_d1);
		double p2 = -r * K * Math::Exp(-r * step) * N_d2;
		double p3 = q * thisSpot * Math::Exp(-q * step) * N_d1;
		double theta = p1 + p2 + p3;
		sum_theta += theta;
						
		double p2_p = r * K * Math::Exp(-r * step) * NNegative_d2;
		double p3_p = -q * thisSpot * Math::Exp(-q * step) * NNegative_d1;
		double thetaPut = p1 + p2_p + p3_p;
		sum_thetaput += thetaPut;
	}

	double mean = sum_payoffs / NumberOfPaths;
	mean *= exp(-rate.Integral(0, T));
	double meanPut = sum_payoffsput / NumberOfPaths;
	meanPut *= exp(-rate.Integral(0, T));
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

Double ProjectXAnalyticsCppLib::MonteCarloCppPricer::DeltaMC(VanillaOptionParameters^% TheOption,
	Double Spot,
	Double Vol,
	Double r,
	UInt64 NumberOfPaths // Number of Monte Carlo simulations
)
{
	double S = Spot;
	double sigma = Vol;
	double T = TheOption->Expiry();
	double K = TheOption->Strike();
	double epsilon = 0.01; // Small change in stock price
	double deltaSum = 0.0;

	for (int i = 0; i < NumberOfPaths; i++)
	{
		double randNorm = m_randomWalk->GetOneGaussian();;
		double ST = S * Math::Exp((r - (Math::Pow(sigma, 2) / 2)) * T + sigma * Math::Sqrt(T) * randNorm);
		double payoff = Math::Max(ST - K, 0.0);
		double priceUp = Math::Exp(-r * T) * payoff;

		ST = S * Math::Exp((r - (Math::Pow(sigma, 2) / 2)) * T - sigma * Math::Sqrt(T) * randNorm);
		payoff = Math::Max(ST - K, 0.0);
		double priceDown = Math::Exp(-r * T) * payoff;

		deltaSum += (priceUp - priceDown) / (2.0 * epsilon);
	}

	return deltaSum / NumberOfPaths;
}

// Function to calculate Gamma using Monte Carlo simulation
Double ProjectXAnalyticsCppLib::MonteCarloCppPricer::GammaMC(
	VanillaOptionParameters^% TheOption,
	Double Spot,
	Double Vol,
	Double r,
	UInt64 NumberOfPaths // Number of Monte Carlo simulations
)
{
	double S = Spot;
	double sigma = Vol;
	double T = TheOption->Expiry();
	double K = TheOption->Strike();
	double epsilon = 0.01; // Small change in stock price
	double gammaSum = 0.0;

	for (int i = 0; i < NumberOfPaths; i++)
	{
		double randNorm = m_randomWalk->GetOneGaussian();
		double ST = S * Math::Exp((r - (Math::Pow(sigma, 2) / 2)) * T + sigma * Math::Sqrt(T) * randNorm);
		double payoff = Math::Max(ST - K, 0.0);
		double priceUp = Math::Exp(-r * T) * payoff;

		ST = S * Math::Exp((r - (Math::Pow(sigma, 2) / 2)) * T - sigma * Math::Sqrt(T) * randNorm);
		payoff = Math::Max(ST - K, 0.0);
		double priceDown = Math::Exp(-r * T) * payoff;

		gammaSum += ((priceUp - priceDown) / (2.0 * epsilon)) / (2.0 * S * epsilon);
	}

	return gammaSum / NumberOfPaths;
}


// Function to calculate Rho using Monte Carlo simulation
Double ProjectXAnalyticsCppLib::MonteCarloCppPricer::RhoMC(VanillaOptionParameters^% TheOption,
	Double Spot,
	Double Vol,
	Double r,
	UInt64 NumberOfPaths
)
{
	double S = Spot;
	double sigma = Vol;
	double T = TheOption->Expiry();
	double K = TheOption->Strike();
	double epsilon = 0.01; // Small change in interest rate
	double rhoSum = 0.0;

	for (int i = 0; i < NumberOfPaths; i++)
	{
		double randNorm = m_randomWalk->GetOneGaussian();
		double ST = S * Math::Exp((r - (Math::Pow(sigma, 2) / 2)) * T + sigma * Math::Sqrt(T) * randNorm);
		double payoff = Math::Max(ST - K, 0.0);
		double priceUp = Math::Exp(-(r + epsilon) * T) * payoff;

		rhoSum += (priceUp - BlackScholesFunctions::BlackScholesCall(S, K, r, T, sigma)) / (epsilon * r);
	}

	return rhoSum / NumberOfPaths;
}

Double ProjectXAnalyticsCppLib::MonteCarloCppPricer::ThetaMC(
	VanillaOptionParameters^% TheOption,
	Double Spot,
	Double Vol,
	Double r,
	UInt64 NumberOfPaths,
	Double timeStep // Time step for simulation
)
{
	Double S = Spot;
	Double sigma = Vol;
	Double T = TheOption->Expiry();
	Double K = TheOption->Strike();
	Double thetaSum = 0.0;

	for (int i = 0; i < NumberOfPaths; i++)
	{
		Double t = 0;
		Double ST = S;
		Double payoff = Math::Max(ST - K, 0.0);
		Double price = Math::Exp(-r * t) * payoff;

		while (t < T)
		{
			Double randNorm = m_randomWalk->GetOneGaussian();
			ST = ST * Math::Exp((r - (Math::Pow(sigma, 2) / 2)) * timeStep + sigma * Math::Sqrt(timeStep) * randNorm);
			t += timeStep;

			payoff = Math::Max(ST - K, 0.0);
			price = Math::Exp(-r * t) * payoff;
		}
		Double PV = BlackScholesFunctions::BlackScholesCall(S, K, r, T - t, sigma);
		thetaSum += (price - PV);
	}

	return thetaSum / NumberOfPaths;
}

// Function to calculate Vega using Monte Carlo simulation
Double ProjectXAnalyticsCppLib::MonteCarloCppPricer::VegaMC(VanillaOptionParameters^% TheOption,
	Double Spot,
	Double Vol,
	Double r,
	UInt64 NumberOfPaths
)
{
	double S = Spot;
	double sigma = Vol;
	double T = TheOption->Expiry();
	double K = TheOption->Strike();
	double epsilon = 0.01; // Small change in volatility
	double vegaSum = 0.0;

	for (int i = 0; i < NumberOfPaths; i++)
	{
		double randNorm = m_randomWalk->GetOneGaussian();
		double ST = S * Math::Exp((r - (Math::Pow(sigma + epsilon, 2) / 2)) * T + (sigma + epsilon) * Math::Sqrt(T) * randNorm);
		double payoff = Math::Max(ST - K, 0.0);
		double priceUp = Math::Exp(-r * T) * payoff;

		vegaSum += (priceUp - BlackScholesFunctions::BlackScholesCall(S, K, r, T, sigma)) / epsilon;
	}

	return vegaSum / NumberOfPaths;
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

