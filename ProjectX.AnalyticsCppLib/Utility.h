#pragma once
#include <cmath>
#include <random>

using namespace System;

// unused - using std:erfc instead
static double Erf(double z)
{
    double t = 1.0 / (1.0 + 0.5 * Math::Abs(z));

    double result = 1 - t * Math::Exp(-z * z - 1.26551223 +
        t * (1.00002368 +
            t * (0.37409196 +
                t * (0.09678418 +
                    t * (-0.18628806 +
                        t * (0.27886807 +
                            t * (-1.13520398 +
                                t * (1.48851587 +
                                    t * (-0.82215223 +
                                        t * (0.17087277))))))))));

    return z >= 0 ? result : -result;
};

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

// Function to generate a random number from a standard normal distribution
static Double RandomStandardNormal()
{
    Random^ random = gcnew Random();
    return Math::Sqrt(-2.0 * Math::Log(1.0 - random->NextDouble())) * Math::Sin(2.0 * Math::PI * random->NextDouble());
}
