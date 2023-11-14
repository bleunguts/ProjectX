#include "pch.h"
#include "MonteCarloHestonCppPricer.h"
using namespace ProjectXAnalyticsCppLib;
using namespace System::Collections::Generic;

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
	
	std::normal_distribution<> N1(0, 1);
	std::normal_distribution<> N3(0, 1);	
	std::random_device random; // random device class instance, source of 'true' randomness for initializing random seed
	std::mt19937 randomGenerator(random()); // Mersenne twister PRNG, initialized with seed from previous random device instance

	double payoff_sum = 0.0;
	double payoff_sumPut = 0.0;
	double S;
	double v;

	// generate random parameters	
	double dt = T / numberOfSteps;	 
	Dictionary<double, List<double>^>^ spotGraph = gcnew Dictionary<double, List<double>^>();
	for (int i = 0; i < numberOfSimulations; i++) 
	{				
		S = S0;		
		v = v0;
		spotGraph[i] = gcnew List<double>();
		for (int j = 0; j < numberOfSteps; j++) 
		{																		
			double dw_v = N1(randomGenerator) * Math::Sqrt(dt);
			double dw_i = N3(randomGenerator) * Math::Sqrt(dt);
			double dw_s = rho * dw_v + Math::Sqrt(1 - Math::Pow(rho, 2)) * dw_i; 
			
			// update Volatility for the next time step
			double dv_t = kappa * (theta - v) * dt + Math::Sqrt(v) * dw_v;
			// calc incremental Stock Price for this time step
			double dS_t = (r - q) * S * dt + Math::Sqrt(v) * S * dw_s;						

			S = S + dS_t;
			v = v + Math::Max((double)dv_t, 0.0);									
			
			payoff_sum += Math::Max((double)S - (double)K, 0.0);
			payoff_sumPut += Math::Max((double)K - (double)S, 0.0);
			spotGraph[i]->Add(S);
		}		
	}
	int callCounter = 0;
	int putCounter = 0;
	for each (List<double>^ spotBucket in spotGraph->Values) 
	{
		for each (double thisSpot in spotBucket) 
		{
			double call = Math::Max((double)thisSpot - (double)K, 0.0);
			double put = Math::Max((double)K - (double)thisSpot, 0.0);
			if (call != 0) {
				callCounter++;
			}
			if (put != 0) {
				putCounter++;
			}
		}
	}
	int totalSimulations = numberOfSimulations * numberOfSteps;	
	double call = Math::Exp(-r * T) * payoff_sum / totalSimulations;
	double put = Math::Exp(-r * T) * payoff_sumPut / totalSimulations;

	GreekResults^ results = gcnew GreekResults(call, put, Double::MinValue, Double::MinValue, Double::MinValue, Double::MinValue, Double::MinValue, Double::MinValue, Double::MinValue, Double::MinValue);
	return results;
}


