#pragma once
#include "ProjectX.AnalyticsCppLib.h"

using namespace System;

namespace ProjectXAnalyticsCppLib
{
	public ref class BlackScholesCppPricer 
	{
	public:		
		Double ProjectXAnalyticsCppLib::BlackScholesCppPricer::Value(
			VanillaOptionParameters^% TheOption,
			Double Spot,
			Double Vol,
			Double r);
		Double ProjectXAnalyticsCppLib::BlackScholesCppPricer::Delta(
			VanillaOptionParameters^% TheOption,
			Double Spot,
			Double Vol,
			Double r			
		);		
		Double ProjectXAnalyticsCppLib::BlackScholesCppPricer::Gamma(
			VanillaOptionParameters^% TheOption,
			Double Spot,
			Double Vol,
			Double r,
			Double epsilon
		);
		Double ProjectXAnalyticsCppLib::BlackScholesCppPricer::Rho(
			VanillaOptionParameters^% TheOption,
			Double Spot,
			Double Vol,
			Double r			
		);		
		Double ProjectXAnalyticsCppLib::BlackScholesCppPricer::Theta(
			VanillaOptionParameters^% TheOption,
			Double Spot,
			Double Vol,
			Double r
		);		
		Double ProjectXAnalyticsCppLib::BlackScholesCppPricer::Vega(
			VanillaOptionParameters^% TheOption,
			Double Spot,
			Double Vol,
			Double r
		);		
		Double ProjectXAnalyticsCppLib::BlackScholesCppPricer::ImpliedVolatility(
			VanillaOptionParameters^% TheOption,
			Double Spot,
			Double r,			
			Double optionPrice
		);	
	};
}