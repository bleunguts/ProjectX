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

double CallExecute(API* a_pObject)
{
	if (a_pObject != NULL)
	{
		double spot = 100;
		double strike = 110;
		double expiry = 0.5;
		double vol = 0.3;
		double r = 0.1;		
		VanillaOptionParameters callOption = VanillaOptionParameters(OptionType::Call, strike, expiry);		

		double pv = a_pObject->BlackScholes_PV(callOption, spot, vol, r);
		return pv;
	}
}

API::API()
{
	m_blackScholesCppPricer = new BlackScholesCppPricer();	
	RandomWalk w = RandomWalk(RandomAlgorithm::BoxMuller);
	m_monteCarloCppPricer = new MonteCarloCppPricer(w);
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
