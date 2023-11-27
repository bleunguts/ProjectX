#include "pch.h"
#include "MonteCarloHestonCppPricer.h"
#include <map>

using namespace std;
using namespace ProjectXAnalyticsCppLib;

GreekResults ProjectXAnalyticsCppLib::MonteCarloHestonCppPricer::MCValue(
	VanillaOptionParameters& TheOption,
	double spotInitial,
	double interestRate,
	double dividendYield,
	unsigned int numberOfSteps,
	unsigned int numberOfSimulations,
	HestonStochasticVolalityParameters& volParams)
{
	double S0 = spotInitial;
	double K = TheOption.Strike();
	double T = TheOption.Expiry();
	double r = interestRate;
	double q = dividendYield;
	
	double v0 = volParams.InitialVolatility;
	double theta = volParams.LongTermVolatility;
	double kappa = volParams.SpeedOfReversion;
	double sigma = volParams.VolOfVol;	

	double payoff_sum = 0.0;
	double payoff_sumPut = 0.0;
	double S;
	double v;
	double dt = T / numberOfSteps;	

	// guassian draw
	std::normal_distribution<> N1(0, 1);
	std::normal_distribution<> N3(0, 1);
	std::random_device random; // random device class instance, source of 'true' randomness for initializing random seed
	std::mt19937 randomGenerator(random()); // Mersenne twister PRNG, initialized with seed from previous random device instance

	// use well known distribution for rho correlation between spot and vol stochastic processes
	std::vector<double> rho_probabilities;
	for (auto & element : volParams.RhoProbabilities) 
	{
    	rho_probabilities.push_back(element);
	}	
	std::vector<double> rho_choices;
	for (auto & element : volParams.RhoChoices) 
	{
    	rho_choices.push_back(element);
	}	
	std::discrete_distribution<int> distribution{ rho_probabilities.begin(), rho_probabilities.end() };

	// diagnostics
	map<double, vector<double>> spotGraphDiag = map<double, vector<double>>();
	vector<double> rhosDiag = vector<double>();
	int callCountDiag = 0;
	int putCountDiag = 0;
	for (int i = 0; i < numberOfSimulations; i++) 
	{				
		S = S0;		
		v = v0;
		spotGraphDiag[i] = vector<double>();
		for (int j = 0; j < numberOfSteps; j++) 
		{												
			double dw_v = N1(randomGenerator) * Math::Sqrt(dt);
			double dw_i = N3(randomGenerator) * Math::Sqrt(dt);
			int weightedProbabilityIndex = distribution(random);
			double rho_i = rho_choices[weightedProbabilityIndex];			
			double dw_s = rho_i * dw_v + Math::Sqrt(1 - Math::Pow(rho_i, 2)) * dw_i; 
			
			// update Volatility for the next time step
			double dv_t = kappa * (theta - v) * dt + sigma * Math::Sqrt(v) * dw_v;
			// calc incremental Stock Price for this time step
			double dS_t = (r - q) * S * dt + Math::Sqrt(v) * S * dw_s;						

			S = S + dS_t;
			v = v + Math::Max((double)dv_t, 0.0);												
			double payoff_call = Math::Max((double)S - (double)K, 0.0);
			double payoff_put = Math::Max((double)K - (double)S, 0.0);
			payoff_sum += payoff_call;
			payoff_sumPut += payoff_put;
		
			// diagnostics
			spotGraphDiag[i].push_back(S);
			rhosDiag.push_back(rho_i);
			if (Math::Max((double)S - (double)K, 0.0) != 0) callCountDiag++;
			if (Math::Max((double)K - (double)S, 0.0) != 0) putCountDiag++;
		}		
	}	
	int totalSimulations = numberOfSimulations * numberOfSteps;
	double call = Math::Exp(-r * T) * payoff_sum / totalSimulations;
	double put = Math::Exp(-r * T) * payoff_sumPut / totalSimulations;

	Debug debug = Debug::Build(callCountDiag, putCountDiag, rhosDiag, spotGraphDiag, totalSimulations);		
	GreekResults results = GreekResults(call, put, debug);	
	return results;
}
