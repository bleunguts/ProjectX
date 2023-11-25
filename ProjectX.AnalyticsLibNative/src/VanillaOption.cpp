#include "pch.h"
#include "VanillaOption.h"

ProjectXAnalyticsCppLib::VanillaOption::VanillaOption(PayOffBridge& ThePayOff_, double Expiry_) : ThePayOff(ThePayOff_), Expiry(Expiry_)
{
}

double ProjectXAnalyticsCppLib::VanillaOption::GetExpiry() const
{
	return Expiry;
}

double ProjectXAnalyticsCppLib::VanillaOption::OptionPayOff(double Spot) const
{
	return ThePayOff(Spot);
}

