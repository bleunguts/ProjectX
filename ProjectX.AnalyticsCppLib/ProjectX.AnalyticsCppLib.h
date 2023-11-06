#pragma once
#include "VanillaOption.h"
#include "RandomWalk.h"

using namespace System;

namespace ProjectXAnalyticsCppLib {	
	public enum class OptionType
	{
		Call, Put
	};

	public ref class VanillaOptionParameters 
	{
	private:
		OptionType m_optionType;
		Double m_strike;
		Double m_expiry;		

	public:
		VanillaOptionParameters(OptionType optionType, double strike, double expiry)
		{
			m_optionType = optionType;
			m_strike = strike;
			m_expiry = expiry;			
		};
		inline OptionType OptionType() { return m_optionType; };
		inline Double Strike() { return m_strike; };
		inline Double Expiry() { return m_expiry; };		
	};	
	
	public ref class OptionsPricingCppCalculator
	{
	private:
		RandomWalk^ m_randomWalk;
	public:						
		OptionsPricingCppCalculator(RandomWalk^ randomWalk) {
			m_randomWalk = randomWalk;
		}

		Double ProjectXAnalyticsCppLib::OptionsPricingCppCalculator::MCValue(VanillaOptionParameters^% TheOption,
			Double Spot,
			Double Vol,
			Double r,
			UInt64 NumberOfPaths);
	};
}
