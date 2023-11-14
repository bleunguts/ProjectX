#include "pch.h"
#include "MonteCarloHestonCppPricer.h"
using namespace ProjectXAnalyticsCppLib;

GreekResults^ ProjectXAnalyticsCppLib::MonteCarloHestonCppPricer::MCValue(
	VanillaOptionParameters^% TheOption, 
	Double spotInitial,
	Double interestRate,
	Double dividendYield,
	UInt64 numberOfSteps, 
	UInt64 numberOfSimulations,
	HestonStochasticVolalityParameters^% volParams)
{
	double S0 = spotInitial;
	double K = TheOption->Strike();
	double T = TheOption->Expiry();
	double r = interestRate;
	double q = dividendYield;
	
	double v0 = volParams->InitialVolatility;
	double theta = volParams->LongTermVolatility;
	double kappa = volParams->SpeedOfReversion;
	double xi = volParams->VolOfVol;
	double rho = volParams->Correlation;

	double lambda = 0.02;  // lambda parameter for multidimensional Girsanov theorem
	std::normal_distribution<> N1(0, 1);
	std::normal_distribution<> N3(0, 1);	
	std::random_device random; // random device class instance, source of 'true' randomness for initializing random seed
	std::mt19937 randomGenerator(random()); // Mersenne twister PRNG, initialized with seed from previous random device instance

	double payoff_sum = 0.0;
	double payoff_sumPut = 0.0;
	double S;
	double v;
	double dt = T / numberOfSteps;
	
	for (int i = 0; i < numberOfSimulations; i++) 
	{
		S = S0;
		v = v0;		
		for (int j = 0; j < numberOfSteps; j++) 
		{						
			double rhoSquared = Math::Pow(rho, 2);															
			double step = T - j * dt;
			double z1 = N1(randomGenerator); // dWv
			double z2 = rho * z1 + Math::Sqrt(1 - rhoSquared) * N3(randomGenerator); // dWs brownian motion respect asset price
			
			double prevS = S;
			double prevv = v;
			// Current Stock Price for this path 
			S = S + (r - q) * S * step + Math::Sqrt(v * step) * z1;
			// Update Volatility for the next path
			v = v + kappa * (theta - lambda) * step + xi * Math::Sqrt(v * step) * z2;
			v = Math::Max(v, 0.0);

			_ASSERT(Double::IsNaN(S) == false);
			double payOffCall = Math::Max(S - K, 0.0);
			double payOffPut = Math::Max(K - S, 0.0);

			double limit = 1000000.0;
			_ASSERT(payOffCall < limit && payOffCall > -limit);
			_ASSERT(payOffPut < limit && payOffPut> -limit);
			payoff_sum += payOffCall;
			payoff_sumPut += payOffPut;

			// Historical volatility, as well as implied volatility and volatility in general, can never be negative. 
			// In other words, it can reach values from zero to positive infinite only.
			_ASSERT(v >= 0);
			_ASSERT(Double::IsNaN(v) == false);
		}
	}
	
	double call = Math::Exp(-r * T) * payoff_sum / numberOfSimulations;
	double put = Math::Exp(-r * T) * payoff_sumPut / numberOfSimulations;

	GreekResults^ results = gcnew GreekResults(call, put, Double::MinValue, Double::MinValue, Double::MinValue, Double::MinValue, Double::MinValue, Double::MinValue, Double::MinValue, Double::MinValue);
	return results;
}


