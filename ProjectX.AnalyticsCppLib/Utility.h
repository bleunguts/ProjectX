#pragma once
#define _USE_MATH_DEFINES
#include <cmath>
#include <random>

using namespace System;

// Function to calculate the cumulative distribution function of the standard normal distribution using std c++ libs
static double normcdf(double x)
{    
	return std::erfc(-x / Math::Sqrt(2.0)) / 2;
};

// Function to calculate the probability density function of the standard normal distribution
static double normpdf(double x)
{        
    return (1.0 / (Math::Sqrt(2.0 * Math::PI))) * Math::Exp(-0.5 * x * x);
}

static double CDF_N(double _X) {
	static const double INF = _HUGE_ENUF * _HUGE_ENUF * _HUGE_ENUF * _HUGE_ENUF;
	static const double NINF = -_HUGE_ENUF * _HUGE_ENUF * _HUGE_ENUF * _HUGE_ENUF;
	static const double C = 2.0 - sqrt(2.0);

	if (NINF == _X) return 0.0;
	if (INF == _X) return 1.0;
	if (0.0 == _X) return 0.5;

	double X = abs(_X);
	double Y = X * X;
	double N = 0.5 * sqrt(1.0 - 1.0 / 30.0 * (
		7.0 * exp(-Y / 2.0) + 16.0 * exp(-Y * C) + (7.0 + M_PI_4 * Y) * exp(-Y)
		));
	if (_X > 0) return 0.5 + N;
	return 0.5 - N;
}
