#pragma once

using namespace System;

namespace ProjectXAnalyticsCppLib {

	public ref class VanillaOption {	
	};

	public ref class Parameters {	
	};
	
	public ref class OptionsPricingCalculator
	{
	public:				
		Double ProjectXAnalyticsCppLib::OptionsPricingCalculator::MCValue(VanillaOption^% TheOption,
			Double Spot,
			Parameters^% Vol,
			Parameters^% r,
			UInt64 NumberOfPaths);
	};
}
