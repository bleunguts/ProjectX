// ProjectX.AnalyticsLibNative.cpp : Defines the entry point for the application.
//
#include "pch.h"
#include "API.h"

using namespace std;
using namespace ProjectXAnalyticsCppLib;

API::API()
{
	m_blackScholesCppPricer = new BlackScholesCppPricer();
};

API::~API()
{
	if (m_blackScholesCppPricer == NULL)
	{
		delete m_blackScholesCppPricer;
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