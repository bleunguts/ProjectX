#pragma once
using namespace System;

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
    return 0.5 * (1.0 + Erf(x / Math::Sqrt(2.0)));
};