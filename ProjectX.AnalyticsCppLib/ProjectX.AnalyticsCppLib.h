#pragma once
#include "VanillaOption.h"
#include "Parameters.h"
#include "RandomWalk.h"

using namespace System;

namespace ProjectXAnalyticsCppLib {

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
	
	public ref class OptionsPricingCalculator
	{
	public:				
		inline Double ProjectXAnalyticsCppLib::OptionsPricingCalculator::MCValue(VanillaOptionParameters^% TheOption,
			Double Spot,
			Double Vol,
			Double r,
			UInt64 NumberOfPaths) {
			return MCValue(TheOption, Spot, Vol, r, NumberOfPaths, gcnew RandomWalk(RandomAlgorithm::BoxMuller));
		};

	private:
		Double ProjectXAnalyticsCppLib::OptionsPricingCalculator::MCValue(VanillaOptionParameters^% TheOption,
			Double Spot,
			Double Vol,
			Double r,
			UInt64 NumberOfPaths,
			RandomWalk^ randomWalk);
	};
}
