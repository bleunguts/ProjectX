#pragma once
#include <cmath>
#include <random>

using namespace System;

// Function to calculate the cumulative distribution function of the standard normal distribution
static double normcdf(double x)
{    
    return 0.5 * (1.0 + std::erfc(x / Math::Sqrt(2.0)));
};

// Function to calculate the probability density function of the standard normal distribution
static double normpdf(double x)
{        
    return (1.0 / (Math::Sqrt(2.0 * Math::PI))) * Math::Exp(-0.5 * x * x);
}

