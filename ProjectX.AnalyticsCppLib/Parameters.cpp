#include "pch.h"
#include "Parameters.h"

Parameters::Parameters(const ParametersInner& innerObject)
{
	InnerObjectPtr = innerObject.clone();
}

Parameters::Parameters(const Parameters& original)
{
	InnerObjectPtr = original.InnerObjectPtr->clone();
}

Parameters& Parameters::operator=(const Parameters& original)
{
	if (this != &original)
	{
		delete InnerObjectPtr;
		InnerObjectPtr = original.InnerObjectPtr->clone();
	}
	return *this;
}

Parameters::~Parameters(void)
{
	delete InnerObjectPtr;
}

double Parameters::Mean(double time1, double time2) const
{
	double total = Integral(time1, time2);
	return total / (time2 - time1);
}

double Parameters::rootMeanSquare(double time1, double time2) const
{
	double total = IntegralSquare(time1, time2);
	return total / (time2 - time1);
}

ParametersConstant::ParametersConstant(double constant)
{
	Constant = constant;
	ConstantSquare = Constant * Constant;
}

ParametersInner* ParametersConstant::clone() const
{
	return new ParametersConstant(*this);
}

double ParametersConstant::Integral(double time1, double time2) const
{
	return (time2 - time1) * Constant;
}

double ParametersConstant::IntegralSquare(double time1, double time2) const
{
	return (time2 - time1) * ConstantSquare;

}