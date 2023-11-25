#include "pch.h"
#include "RandomWalk.h"
#include <cmath>

double RandomWalk::GetOneGaussian()
{
	if (m_Algorithm == RandomAlgorithm::Summation) {
		return GetOneGaussianBySummation();
	}

	if (m_Algorithm == RandomAlgorithm::BoxMuller) {
		return GetOneGaussianByBoxMuller();
	}

	throw ("blew up");
}

double RandomWalk::GetOneGaussianBySummation()
{
	double result = 0;
	for (unsigned long j = 0; j < 12; ++j)
	{
		result += rand() / static_cast<double>(RAND_MAX);
	}

	result -= 6.0;
	return result;
}

double RandomWalk::GetOneGaussianByBoxMuller()
{
	double result;

	double x, y;
	double sizeSquared;

	do
	{
		x = 2.0 * rand() / static_cast<double>(RAND_MAX) - 1;
		y = 2.0 * rand() / static_cast<double>(RAND_MAX) - 1;
		sizeSquared = x * x + y * y;
	} while (sizeSquared >= 1.0);

	result = x * sqrt(-2 * log(sizeSquared) / sizeSquared);
	return result;
}
