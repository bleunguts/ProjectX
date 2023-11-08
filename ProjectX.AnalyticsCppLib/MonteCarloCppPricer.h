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
		Double ProjectXAnalyticsCppLib::MonteCarloCppPricer::MCValue(
			VanillaOptionParameters^% TheOption,
			Double Spot,
			Double Vol,
			Double r,
			UInt64 NumberOfPaths);				
		Double ProjectXAnalyticsCppLib::MonteCarloCppPricer::DeltaMC(
			VanillaOptionParameters^% TheOption,
			Double Spot,
			Double Vol,
			Double r,
			UInt64 NumberOfPaths
		);
		Double ProjectXAnalyticsCppLib::MonteCarloCppPricer::GammaMC(
			VanillaOptionParameters^% TheOption,
			Double Spot,
			Double Vol,
			Double r,
			UInt64 NumberOfPaths
		);				
		Double ProjectXAnalyticsCppLib::MonteCarloCppPricer::RhoMC(
			VanillaOptionParameters^% TheOption,
			Double Spot,
			Double Vol,
			Double r,
			UInt64 NumberOfPaths
		);		
		Double ProjectXAnalyticsCppLib::MonteCarloCppPricer::ThetaMC(
			VanillaOptionParameters^% TheOption,
			Double Spot,
			Double Vol,
			Double r,
			UInt64 NumberOfPaths,
			Double timeStep // Time step for simulation
		);		
		Double ProjectXAnalyticsCppLib::MonteCarloCppPricer::VegaMC(
			VanillaOptionParameters^% TheOption,
			Double Spot,
			Double Vol,
			Double r,
			UInt64 NumberOfPaths
		);	
		Double ProjectXAnalyticsCppLib::MonteCarloCppPricer::ImpliedVolatilityMC(
			VanillaOptionParameters^% TheOption,
			Double Spot,
			Double r,
			UInt64 NumberOfPaths,
			Double optionPrice
		);
	};
}