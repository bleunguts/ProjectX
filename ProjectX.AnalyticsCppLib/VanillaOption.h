#pragma once
#include "PayOff.h"

class VanillaOption
{
public:
	VanillaOption(PayOffBridge& ThePayOff_, double Expiry_);

	double GetExpiry() const;
	double OptionPayOff(double Spot) const;
private:
	double Expiry;
	PayOffBridge& ThePayOff;
};

