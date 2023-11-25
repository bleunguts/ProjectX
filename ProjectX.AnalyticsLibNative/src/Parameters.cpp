#include "pch.h"
#include "Parameters.h"
using namespace ProjectXAnalyticsCppLib;

ProjectXAnalyticsCppLib::Parameters::Parameters(const ParametersInner& innerObject)
{
	InnerObjectPtr = innerObject.clone();
}

ProjectXAnalyticsCppLib::Parameters::Parameters(const Parameters& original)
{
	InnerObjectPtr = original.InnerObjectPtr->clone();
}

Parameters& ProjectXAnalyticsCppLib::Parameters::operator=(const Parameters& original)
{
	if (this != &original)
	{
		delete InnerObjectPtr;
		InnerObjectPtr = original.InnerObjectPtr->clone();
	}
	return *this;
}

ProjectXAnalyticsCppLib::Parameters::~Parameters(void)
{
	delete InnerObjectPtr;
}

double ProjectXAnalyticsCppLib::Parameters::Mean(double time1, double time2) const
{
	double total = Integral(time1, time2);
	return total / (time2 - time1);
}

double ProjectXAnalyticsCppLib::Parameters::rootMeanSquare(double time1, double time2) const
{
	double total = IntegralSquare(time1, time2);
	return total / (time2 - time1);
}

ProjectXAnalyticsCppLib::ParametersConstant::ParametersConstant(double constant)
{
	Constant = constant;
	ConstantSquare = Constant * Constant;
}

ParametersInner* ProjectXAnalyticsCppLib::ParametersConstant::clone() const
{
	return new ParametersConstant(*this);
}

double ProjectXAnalyticsCppLib::ParametersConstant::Integral(double time1, double time2) const
{
	return (time2 - time1) * Constant;
}

double ProjectXAnalyticsCppLib::ParametersConstant::IntegralSquare(double time1, double time2) const
{
	return (time2 - time1) * ConstantSquare;
}