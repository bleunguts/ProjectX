#pragma once
#include "../pch.h"

using namespace std;

namespace ProjectXAnalyticsCppLib
{
	struct HestonStochasticVolalityParameters
	{
	public:
		HestonStochasticVolalityParameters(double initialVolatility, double longTermVolatility, double speedOfReversion, double volOfVol, vector<double> rhoProbabilities, vector<double> rhoChoices)
		{
			this->InitialVolatility = initialVolatility;
			this->LongTermVolatility = longTermVolatility;
			this->SpeedOfReversion = speedOfReversion;
			this->VolOfVol = volOfVol;
			this->RhoProbabilities = rhoProbabilities;
			this->RhoChoices = rhoChoices;
		}
		HestonStochasticVolalityParameters(double initialVolatility, double longTermVolatility, double speedOfReversion, double volOfVol, double rho)
		{
			this->InitialVolatility = initialVolatility;
			this->LongTermVolatility = longTermVolatility;
			this->SpeedOfReversion = speedOfReversion;
			this->VolOfVol = volOfVol;
			this->Rho = rho;
		}
	public:
		double InitialVolatility;  // v0
		double LongTermVolatility; // theta
		double SpeedOfReversion;   // kappa
		double VolOfVol;		   // xi
		// rho correlation between W1 and W2 brownian motions under the risk-neutral probability measure
		// -0.7 is a standard correlation used in the Broadie paper
		vector<double> RhoProbabilities; // rho probabilitiees 
		vector<double> RhoChoices;
		double Rho; // straight Rho value
	};
}