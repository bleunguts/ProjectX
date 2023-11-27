#include "pch.h"
#include "PayOff.h"
//#include <minmax.h>
using namespace ProjectXAnalyticsCppLib;

ProjectXAnalyticsCppLib::PayOffCall::PayOffCall(double Strike_) : Strike(Strike_)
{
}

double ProjectXAnalyticsCppLib::PayOffCall::operator() (double Spot) const
{
	return max(Spot - Strike, 0.0);
}

PayOff* ProjectXAnalyticsCppLib::PayOffCall::clone() const
{
	return new PayOffCall(*this);
}

ProjectXAnalyticsCppLib::PayOffPut::PayOffPut(double Strike_) : Strike(Strike_)
{
}

double ProjectXAnalyticsCppLib::PayOffPut::operator() (double Spot) const
{
	return max(Strike - Spot, 0.0);
}

PayOff* ProjectXAnalyticsCppLib::PayOffPut::clone() const
{
	return new PayOffPut(*this);
}

ProjectXAnalyticsCppLib::PayOffBridge::PayOffBridge(const PayOffBridge& original)
{
	ThePayOffPtr = original.ThePayOffPtr->clone();
}

ProjectXAnalyticsCppLib::PayOffBridge::PayOffBridge(const PayOff& innerPayOff)
{
	ThePayOffPtr = innerPayOff.clone();
}

ProjectXAnalyticsCppLib::PayOffBridge::~PayOffBridge(void)
{
	delete ThePayOffPtr;
}

PayOffBridge& ProjectXAnalyticsCppLib::PayOffBridge::operator=(const PayOffBridge& original)
{
	if (this != &original)
	{
		delete ThePayOffPtr;
		ThePayOffPtr = original.ThePayOffPtr->clone();
	}

	return *this;
}
