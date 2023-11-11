#pragma once
#include "ProjectX.AnalyticsCppLib.h"
#include "VanillaOption.h"
#include "RandomWalk.h"

using namespace System;

namespace ProjectXAnalyticsCppLib 
{
	public ref class MonteCarloCppPricer
	{
	private:
		RandomWalk^ m_randomWalk;
	public:
		MonteCarloCppPricer(RandomWalk^ randomWalk)
		{
			m_randomWalk = randomWalk;
		};
		GreekResults^ ProjectXAnalyticsCppLib::MonteCarloCppPricer::MCValue(
			VanillaOptionParameters^% TheOption,
			Double Spot,
			Double Vol,
			Double r,
			UInt64 NumberOfPaths);						
		Double ProjectXAnalyticsCppLib::MonteCarloCppPricer::ImpliedVolatilityMC(
			VanillaOptionParameters^% TheOption,
			Double Spot,
			Double r,
			UInt64 NumberOfPaths,
			Double optionPrice
		);
	};
}