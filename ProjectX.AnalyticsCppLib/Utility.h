#pragma once
#include "Parameters.h"

public ref class MathFunctions abstract sealed
{
public:
	static double IntegralSquare(double value, double time1, double time2)
	{
		ParametersConstant p = ParametersConstant(value);
		return p.IntegralSquare(time1, time2);
	}
	static double Integral(double value, double time1, double time2)
	{
		ParametersConstant p = ParametersConstant(value);
		return p.Integral(time1, time2);
	}
};