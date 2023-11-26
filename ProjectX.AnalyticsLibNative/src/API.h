// ProjectX.AnalyticsLibNative.h : Include file for standard system include files,
// or project specific include files.

#pragma once
#include "pch.h"
#include "BlackScholesCppPricer.h"
#include "MonteCarloCppPricer.h"
#include <iostream>

using namespace ProjectXAnalyticsCppLib;

class PROJECT_API API
{
public:
	API();	
	virtual ~API();
	void execute(void);
	inline double BlackScholes_PV(VanillaOptionParameters& TheOption,double Spot,double Vol,double r);
	inline double BlackScholes_Delta(VanillaOptionParameters& TheOption,double Spot,double Vol,double r);
	inline double BlackScholes_Gamma(VanillaOptionParameters& TheOption,double Spot,double Vol,double r,
		double epsilon);
	inline double BlackScholes_Vega(VanillaOptionParameters& TheOption,double Spot,double Vol,double r);
	inline double BlackScholes_Theta(VanillaOptionParameters& TheOption,double Spot,double Vol,double r);
	inline double BlackScholes_Rho(VanillaOptionParameters& TheOption,double Spot,double Vol,double r);
	inline double BlackScholes_ImpliedVolatility(VanillaOptionParameters& TheOption,double Spot,double r,
		double optionPrice);
	inline GreekResults MonteCarlo_PV(VanillaOptionParameters& TheOption, double Spot, double Vol, double r, 
		unsigned int NumberOfPaths);
	inline double API::MonteCarlo_ImpliedVolatility(VanillaOptionParameters& TheOption, double Spot, double r, unsigned int NumberOfPaths,
		double optionPrice);
private:
	BlackScholesCppPricer* m_blackScholesCppPricer;
	MonteCarloCppPricer* m_monteCarloCppPricer;
};

extern "C"
{
	extern PROJECT_API API* CreateAPI();
	extern PROJECT_API void DisposeAPI(API* a_pObject);
	extern PROJECT_API double CallExecute(API* a_pObject);
};