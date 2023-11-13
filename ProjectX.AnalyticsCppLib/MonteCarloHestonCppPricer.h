#pragma once
using namespace System;

namespace ProjectXAnalyticsCppLib
{
	public ref class MonteCarloHestonCppPricer 
	{
	public:
		Double ProjectXAnalyticsCppLib::MonteCarloHestonCppPricer::MCValue(
			VanillaOptionParameters^% TheOption,
			Double Spot,
			Double Vol,
			Double r,
			UInt64 NumberOfPaths);
	};
}