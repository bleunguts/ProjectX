// ProjectX.AnalyticsLibNative.h : Include file for standard system include files,
// or project specific include files.

#pragma once
#include "pch.h"
#include "BlackScholesCppPricer.h"
#include "MonteCarloCppPricer.h"
#include "MonteCarloHestonCppPricer2.h"
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
	inline double MonteCarlo_ImpliedVolatility(VanillaOptionParameters& TheOption, double Spot, double r, unsigned int NumberOfPaths,
		double optionPrice);
	inline GreekResults Heston_MCValue(VanillaOptionParameters& TheOption,double spotInitial,double interestRate,
		double dividendYield, unsigned int numberOfSteps,unsigned int numberOfSimulations, 
		HestonStochasticVolalityParameters& volParams);
private:
	BlackScholesCppPricer* m_blackScholesCppPricer;
	MonteCarloCppPricer* m_monteCarloCppPricer;
	MonteCarloHestonCppPricer2* m_hestonCppPricer;
};

extern "C"
{
	extern PROJECT_API API* CreateAPI();
	extern PROJECT_API void DisposeAPI(API* a_pObject);	
	extern PROJECT_API double BlackScholes_PV(API* a_pObject, VanillaOptionParameters& TheOption, double Spot, double Vol, double r);
	extern PROJECT_API double BlackScholes_Delta(API* a_pObject, VanillaOptionParameters& TheOption, double Spot, double Vol, double r);
	extern PROJECT_API double BlackScholes_Gamma(API* a_pObject, VanillaOptionParameters& TheOption, double Spot, double Vol, double r, double epsilon);
	extern PROJECT_API double BlackScholes_Vega(API* a_pObject, VanillaOptionParameters& TheOption, double Spot, double Vol, double r);
	extern PROJECT_API double BlackScholes_Theta(API* a_pObject, VanillaOptionParameters& TheOption, double Spot, double Vol, double r);
	extern PROJECT_API double BlackScholes_Rho(API* a_pObject, VanillaOptionParameters& TheOption, double Spot, double Vol, double r);
	extern PROJECT_API double BlackScholes_ImpliedVolatility(API* a_pObject, VanillaOptionParameters& TheOption, double Spot, double r, double optionPrice);
	extern PROJECT_API GreekResults MonteCarlo_PV(API* a_pObject, VanillaOptionParameters& TheOption, double Spot, double Vol, double r,unsigned int NumberOfPaths);
	extern PROJECT_API double MonteCarlo_ImpliedVolatility(API* a_pObject, VanillaOptionParameters& TheOption, double Spot, double r, unsigned int NumberOfPaths,double optionPrice);
	//extern PROJECT_API GreekResults Heston_MCValue(API* a_pObject, VanillaOptionParameters& TheOption, double spotInitial, double interestRate, double dividendYield, unsigned int numberOfSteps, unsigned int numberOfSimulations,HestonStochasticVolalityParameters& volParams);
};