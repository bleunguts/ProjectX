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
		Double ProjectXAnalyticsCppLib::OptionsPricingCppCalculator::Delta(VanillaOptionParameters^% TheOption,
			Double Spot,
			Double Vol,
			Double r,
			Double epsilon,
			UInt64 NumberOfPaths
		);
		Double ProjectXAnalyticsCppLib::OptionsPricingCppCalculator::Gamma(VanillaOptionParameters^% TheOption,
			Double Spot,
			Double Vol,
			Double r,
			Double epsilon			
		);
		Double ProjectXAnalyticsCppLib::OptionsPricingCppCalculator::Rho(VanillaOptionParameters^% TheOption,
			Double Spot,
			Double Vol,
			Double r,
			UInt64 NumberOfPaths
		);
		Double ProjectXAnalyticsCppLib::OptionsPricingCppCalculator::Theta(VanillaOptionParameters^% TheOption,
			Double Spot,
			Double Vol,
			Double r,
			UInt64 NumberOfPaths
		);
		Double ProjectXAnalyticsCppLib::OptionsPricingCppCalculator::Vega(VanillaOptionParameters^% TheOption,
			Double Spot,
			Double Vol,
			Double r,
			UInt64 NumberOfPaths
		);
		Double ProjectXAnalyticsCppLib::OptionsPricingCppCalculator::ImpliedVolatilityMC(VanillaOptionParameters^% TheOption,
			Double Spot,			
			Double r,
			UInt64 NumberOfPaths,
			Double optionPrice
		);
	private:
		static Double ProjectXAnalyticsCppLib::OptionsPricingCppCalculator::BlackScholes(
			Double S, // Current stock price
			Double K, // Strike price
			Double r, // Risk-free interest rate
			Double T, // Time to expiration
			Double sigma // Volatility
		);
		static Double ProjectXAnalyticsCppLib::OptionsPricingCppCalculator::BlackScholesRho(
			Double S, // Current stock price
			Double K, // Strike price
			Double r, // Risk-free interest rate
			Double T, // Time to expiration
			Double sigma // Volatility
		);
		static Double ProjectXAnalyticsCppLib::OptionsPricingCppCalculator::BlackScholesDelta(VanillaOptionParameters^% TheOption,
			Double Spot,
			Double Vol,
			Double r, 
			Double optionPrice,
			Double epsilon);
		static Double ProjectXAnalyticsCppLib::OptionsPricingCppCalculator::BlackScholesGamma(
			Double S, // Current stock price
			Double K, // Strike price
			Double r, // Risk-free interest rate
			Double T, // Time to expiration
			Double sigma, // Volatility
			Double epsilon // Small change in stock price
		);		
		static Double ProjectXAnalyticsCppLib::OptionsPricingCppCalculator::BlackScholesTheta(
			Double S, // Current stock price
			Double K, // Strike price
			Double r, // Risk-free interest rate
			Double T, // Time to expiration
			Double sigma // Volatility
		);
		static Double ProjectXAnalyticsCppLib::OptionsPricingCppCalculator::BlackScholesVega(
			Double S, // Current stock price
			Double K, // Strike price
			Double r, // Risk-free interest rate
			Double T, // Time to expiration
			Double sigma // Volatility
		);
	};
}
