// ProjectX.AnalyticsLibNative.cpp : Defines the entry point for the application.
//
#include "pch.h"
#include "API.h"
#include "RandomWalk.h"

using namespace std;
using namespace ProjectXAnalyticsCppLib;

API* CreateAPI()
{
	return new API();
}

void DisposeAPI(API* a_pObject)
{
	if (a_pObject != NULL)
	{
		delete a_pObject;
		a_pObject = NULL;
	}
}

double BlackScholes_PV(API* a_pObject, VanillaOptionParameters& TheOption, double Spot, double Vol, double r)
{
	if (a_pObject != NULL) 
	{
		return a_pObject->BlackScholes_PV(TheOption, Spot, Vol, r);
	}
	throw "Fatal error: disposed api pointer";
}
double BlackScholes_Delta(API* a_pObject, VanillaOptionParameters& TheOption, double Spot, double Vol, double r)
{
	if (a_pObject != NULL)
	{
		return a_pObject->BlackScholes_Delta(TheOption, Spot, Vol, r);
	}
	throw "Fatal error: disposed api pointer";
}
double BlackScholes_Gamma(API* a_pObject, VanillaOptionParameters& TheOption, double Spot, double Vol, double r, double epsilon)
{
	if (a_pObject != NULL)
	{
		return a_pObject->BlackScholes_Gamma(TheOption, Spot, Vol, r, epsilon);
	}
	throw "Fatal error: disposed api pointer";
}
double BlackScholes_Vega(API* a_pObject, VanillaOptionParameters& TheOption, double Spot, double Vol, double r)
{
	if (a_pObject != NULL)
	{
		return a_pObject->BlackScholes_Vega(TheOption, Spot, Vol, r);
	}
	throw "Fatal error: disposed api pointer";
}
double BlackScholes_Theta(API* a_pObject, VanillaOptionParameters& TheOption, double Spot, double Vol, double r)
{
	if (a_pObject != NULL)
	{
		return a_pObject->BlackScholes_Theta(TheOption, Spot, Vol, r);
	}
	throw "Fatal error: disposed api pointer";
}
double BlackScholes_Rho(API* a_pObject, VanillaOptionParameters& TheOption, double Spot, double Vol, double r)
{
	if (a_pObject != NULL)
	{
		return a_pObject->BlackScholes_Rho(TheOption, Spot, Vol, r);
	}
	throw "Fatal error: disposed api pointer";
}
double BlackScholes_ImpliedVolatility(API* a_pObject, VanillaOptionParameters& TheOption, double Spot, double r, double optionPrice)
{
	if (a_pObject != NULL)
	{
		return a_pObject->BlackScholes_ImpliedVolatility(TheOption, Spot, r, optionPrice);
	}
	throw "Fatal error: disposed api pointer";
}
GreekResults MonteCarlo_PV(API* a_pObject, VanillaOptionParameters& TheOption, double Spot, double Vol, double r, unsigned int NumberOfPaths)
{
	if (a_pObject != NULL)
	{
		return a_pObject->MonteCarlo_PV(TheOption, Spot, Vol, r, NumberOfPaths);
	}
	throw "Fatal error: disposed api pointer";
}
double MonteCarlo_ImpliedVolatility(API* a_pObject, VanillaOptionParameters& TheOption, double Spot, double r, unsigned int NumberOfPaths, double optionPrice)
{
	if (a_pObject != NULL)
	{
		return a_pObject->MonteCarlo_ImpliedVolatility(TheOption, Spot, r, NumberOfPaths, optionPrice);
	}
	throw "Fatal error: disposed api pointer";
}

GreekResults Heston_MCValue(API* a_pObject, VanillaOptionParameters& TheOption, double spotInitial, double interestRate, double dividendYield, unsigned int numberOfSteps, unsigned int numberOfSimulations, HestonStochasticVolalityParameters& volParams)
{
	if (a_pObject != NULL)
	{
		return a_pObject->Heston_MCValue(TheOption, spotInitial, interestRate, dividendYield, numberOfSteps, numberOfSimulations, volParams);
	}
	throw "Fatal error: disposed api pointer";
}

API::API()
{
	m_blackScholesCppPricer = new BlackScholesCppPricer();	
	RandomWalk w = RandomWalk(RandomAlgorithm::BoxMuller);
	m_monteCarloCppPricer = new MonteCarloCppPricer(w);
	m_hestonCppPricer = new MonteCarloHestonCppPricer2();
};

API::~API()
{
	if (m_blackScholesCppPricer == NULL)
	{
		delete m_blackScholesCppPricer;
	}
	if (m_monteCarloCppPricer == NULL)
	{
		delete m_monteCarloCppPricer;
	}
};

void API::execute(void)
{
	cout << "Hello world";
}

double API::BlackScholes_PV(
	VanillaOptionParameters& TheOption,
	double Spot,
	double Vol,
	double r)
{		
	return m_blackScholesCppPricer->Value(TheOption, Spot, Vol, r);
}

double API::BlackScholes_Delta(VanillaOptionParameters& TheOption,
	double Spot,
	double Vol,
	double r
)
{
	return m_blackScholesCppPricer->Delta(TheOption, Spot, Vol, r);
}

double API::BlackScholes_Gamma(VanillaOptionParameters& TheOption,
	double Spot,
	double Vol,
	double r,
	double epsilon
)
{
	return m_blackScholesCppPricer->Gamma(TheOption, Spot, Vol, r, epsilon);
}

double API::BlackScholes_Rho(VanillaOptionParameters& TheOption,
	double Spot,
	double Vol,
	double r
)
{
	return m_blackScholesCppPricer->Rho(TheOption, Spot, Vol, r);
}

double API::BlackScholes_Theta(VanillaOptionParameters& TheOption,
	double Spot,
	double Vol,
	double r
)
{
	return m_blackScholesCppPricer->Theta(TheOption, Spot, Vol, r);
}

double API::BlackScholes_Vega(VanillaOptionParameters& TheOption,
	double Spot,
	double Vol,
	double r
)
{
	return m_blackScholesCppPricer->Vega(TheOption, Spot, Vol, r);
}

double API::BlackScholes_ImpliedVolatility(
	VanillaOptionParameters& TheOption,
	double Spot,
	double r,
	double optionPrice
)
{
	return m_blackScholesCppPricer->ImpliedVolatility(TheOption, Spot, r, optionPrice);
}

GreekResults API::MonteCarlo_PV(VanillaOptionParameters& TheOption, double Spot, double Vol, double r,
	unsigned int NumberOfPaths) 
{
	return m_monteCarloCppPricer->MCValue(TheOption, Spot, Vol, r, NumberOfPaths);
}

double API::MonteCarlo_ImpliedVolatility(VanillaOptionParameters& TheOption,
	double Spot,
	double r,
	unsigned int NumberOfPaths,
	double optionPrice)
{
	return m_monteCarloCppPricer->ImpliedVolatilityMC(TheOption, Spot, r, NumberOfPaths, optionPrice);
}

GreekResults API::Heston_MCValue(VanillaOptionParameters& TheOption, double spotInitial, double interestRate,
	double dividendYield, unsigned int numberOfSteps, unsigned int numberOfSimulations,
	HestonStochasticVolalityParameters& volParams)
{
	return m_hestonCppPricer->MCValue(TheOption, spotInitial, interestRate, dividendYield, numberOfSteps, numberOfSimulations, volParams);
}