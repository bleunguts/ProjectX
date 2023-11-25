#pragma once
#define _USE_MATH_DEFINES
#include <cmath>
#include <algorithm>
#include <random>
using namespace std;

static class Math
{
public:	
	inline static double Log(double v) {
		return log(v);
	}
	inline static double Exp(double v) {
		return exp(v);
	};
	inline static double Sqrt(double v) {
		return sqrt(v);
	};
	inline static double Max(double x1, double x2) {
		return max(x1, x2);
	};
	inline static double Pow(double x1, double x2) {
		return pow(x1, x2);
	};
	inline static double Abs(double v) {
		return abs(v);
	};
	inline static double PI() {
		return M_PI;
	};
};

// Function to calculate the cumulative distribution function of the standard normal distribution using std c++ libs
static double normcdf(double x)
{    
	return std::erfc(-x / Math::Sqrt(2.0)) / 2;
};

// Function to calculate the probability density function of the standard normal distribution
static double normpdf(double x)
{
	return (1.0 / (Math::Sqrt(2.0 * Math::PI()))) * Math::Exp(-0.5 * x * x);
};

// Function to do Cholesky decomposition
static vector< vector<double> > Cholesky(vector< vector<double> >& data)
{
	int n = data.size();
	vector< vector<double> > mat(n, vector<double>(n));
	double sum1 = 0.0;
	double sum2 = 0.0;
	double sum3 = 0.0;
	// Initialize the first element
	mat[0][0] = sqrt(data[0][0]);

	// First elements of each row
	for (int j = 1; j <= n - 1; j++)
	{
		mat[j][0] = data[j][0] / mat[0][0];
	}
	for (int i = 1; i <= (n - 2); i++)
	{
		for (int k = 0; k <= (i - 1); k++)
		{
			sum1 += pow(mat[i][k], 2);
		}
		mat[i][i] = sqrt(data[i][i] - sum1);
		for (int j = (i + 1); j <= (n - 1); j++)
		{
			for (int k = 0; k <= (i - 1); k++)
			{
				sum2 += mat[j][k] * mat[i][k];
			}
			mat[j][i] = (data[j][i] - sum2) / mat[i][i];
		}
	}
	for (int k = 0; k <= (n - 2); k++)
	{
		sum3 += pow(mat[n - 1][k], 2);
	}
	mat[n - 1][n - 1] = sqrt(data[n - 1][n - 1] - sum3);

	return mat;
};