#include "pch.h";
#include "MonteCarloHestonCppPricer2.h"
using namespace ProjectXAnalyticsCppLib;
using namespace std;

GreekResults ProjectXAnalyticsCppLib::MonteCarloHestonCppPricer2::MCValue(
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
	double sigma = volParams->VolOfVol;
	double rho = volParams->Rho;

	// guassian draw
	std::normal_distribution<> N1(0, 1);
	std::normal_distribution<> N3(0, 1);
	std::random_device random; // random device class instance, source of 'true' randomness for initializing random seed
	std::mt19937 randomGenerator(random()); // Mersenne twister PRNG, initialized with seed from previous random device instance

	// Correlation matrix
	vector< vector<double> > Correlation(2, vector<double>(2));
	Correlation[0][0] = 1.0, Correlation[0][1] = rho, Correlation[1][0] = rho, Correlation[1][1] = 1.0;
	// Cholesky decomposition of corr matrix
	vector< vector<double> > CorrelationMatrix = Cholesky(Correlation);
	// Generate two uncorrelated vectors
	vector< vector<double> > rnd(numberOfSteps, vector<double>(2));
	// Generate two correlated vectors
	vector<double> dw_v(numberOfSteps, v0), dw_i(numberOfSteps, S0);

	// payoffs
	double payoff_sum = 0.0;
	double payoff_sumPut = 0.0;
	// diagnostics
	Dictionary<double, List<double>^>^ spotGraphDiag = gcnew Dictionary<double, List<double>^>();
	List<double>^ rhosDiag = gcnew List<double>();
	int callCountDiag = 0;
	int putCountDiag = 0;

	// Create the spot and vol paths
	vector<double> dS_t(numberOfSteps, S0);
	vector<double> dv_t(numberOfSteps, v0);
	for (int i = 0; i < numberOfSimulations; i++)
	{
		spotGraphDiag[i] = gcnew List<double>();
		for (int t = 0; t < numberOfSteps; t++)
		{
			rnd[t][0] = N1(randomGenerator);
			rnd[t][1] = N3(randomGenerator);
			dw_v[t] = rnd[t][0] * CorrelationMatrix[0][0] + rnd[t][1] * CorrelationMatrix[1][0];
			dw_i[t] = rnd[t][0] * CorrelationMatrix[0][1] + rnd[t][1] * CorrelationMatrix[1][1];
		}
		Heston_VolPath(dw_v, dv_t, T, kappa, theta, rho);
		Heston_SpotPath(dw_i, dv_t, dS_t, T, r);
		double S = dS_t[numberOfSteps - 1];
		double payoff_call = Math::Max((double)S - (double)K, 0.0);
		double payoff_put = Math::Max((double)K - (double)S, 0.0);
		double S_call = payoff_call;
		double S_put = payoff_put;
		payoff_sum += S_call;
		payoff_sumPut += S_put;

		// diagnostics
		spotGraphDiag[i]->Add(S);		
		if (Math::Max((double)S - (double)K, 0.0) != 0) callCountDiag++;
		if (Math::Max((double)K - (double)S, 0.0) != 0) putCountDiag++;
	}	
	double call = Math::Exp(-r * T) * payoff_sum / numberOfSimulations;
	double put = Math::Exp(-r * T) * payoff_sumPut / numberOfSimulations;

	Debug debug = Debug(callCountDiag, putCountDiag, rhosDiag, spotGraphDiag, numberOfSimulations * numberOfSteps);
	GreekResults results = GreekResults(call, put, debug);
	return results;
}

void ProjectXAnalyticsCppLib::MonteCarloHestonCppPricer2::Heston_VolPath(const vector<double>& dw_v, vector<double>& v_t, double T, double Kappa, double Theta, double Epslon)
{
	int numberOfSteps = dw_v.size();
	double dt = T / numberOfSteps;
	// Start iterating from i = 1
	double v = 0.0;
	double v_i = 0.0;
	for (int i = 1; i < numberOfSteps; ++i)
	{
		v = Math::Max(v_t[i - 1], 0.0);
		v_i = v_t[i - 1] + Kappa * dt * (Theta - v) + Epslon * Math::Sqrt(v * dt) * dw_v[i - 1];
		v_t[i] = Math::Max(v_i, 0.0);
	}
}

// Calculate the asset price path
void ProjectXAnalyticsCppLib::MonteCarloHestonCppPricer2::Heston_SpotPath(const vector<double>& dw_i, const vector<double>& v_t, vector<double>& S_t, double T, double r)
{
	int NumberOfSteps = dw_i.size();
	double dt = T / NumberOfSteps;
	double v = 0.0;
	// Start iterating from i = 1
	for (int i = 1; i < NumberOfSteps; ++i)
	{
		v = Math::Max(v_t[i - 1], 0.0);
		S_t[i] = S_t[i - 1] * exp((r - 0.5 * v) * dt + sqrt(v * dt) * dw_i[i - 1]);
	}
}

