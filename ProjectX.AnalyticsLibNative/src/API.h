// ProjectX.AnalyticsLibNative.h : Include file for standard system include files,
// or project specific include files.

#pragma once
#include "pch.h"
#include <iostream>

#ifdef BUILD_DLL
#define PROJECT_API __declspec(dllexport)
#else
#define PROJECT_API __declspec(dllimport)
#endif
using namespace ProjectXAnalyticsCppLib;

class PROJECT_API API
{
public:
    API() {};
    virtual ~API() {};
    void execute(void);
	double Value(
		VanillaOptionParameters& TheOption,
		double Spot,
		double Vol,
		double r);
	double Delta(
		VanillaOptionParameters& TheOption,
		double Spot,
		double Vol,
		double r
	);
	double Gamma(
		VanillaOptionParameters& TheOption,
		double Spot,
		double Vol,
		double r,
		double epsilon
	);
	double Vega(
		VanillaOptionParameters& TheOption,
		double Spot,
		double Vol,
		double r
	);
	double Theta(
		VanillaOptionParameters& TheOption,
		double Spot,
		double Vol,
		double r
	);
	double Rho(
		VanillaOptionParameters& TheOption,
		double Spot,
		double Vol,
		double r
	);
	double ImpliedVolatility(
		VanillaOptionParameters& TheOption,
		double Spot,
		double r,
		double optionPrice
	);
};
