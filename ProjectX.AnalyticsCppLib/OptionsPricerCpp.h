#pragma once
#include "ProjectX.AnalyticsCppLib.h"
#include "VanillaOption.h"
#include "RandomWalk.h"

using namespace System;

namespace ProjectXAnalyticsCppLib 
{
	public ref class OptionsPricerCpp
	{
	private:
		RandomWalk^ m_randomWalk;
	public:
		OptionsPricerCpp(RandomWalk^ randomWalk)
		{
			m_randomWalk = randomWalk;
		};
		Double ProjectXAnalyticsCppLib::OptionsPricerCpp::MCValue(
			VanillaOptionParameters^% TheOption,
			Double Spot,
			Double Vol,
			Double r,
			UInt64 NumberOfPaths);
		Double ProjectXAnalyticsCppLib::OptionsPricerCpp::Value(
			VanillaOptionParameters^% TheOption,
			Double Spot,
			Double Vol,
			Double r);
		Double ProjectXAnalyticsCppLib::OptionsPricerCpp::Delta(
			VanillaOptionParameters^% TheOption,
			Double Spot,
			Double Vol,
			Double r,
			UInt64 NumberOfPaths
		);
		Double ProjectXAnalyticsCppLib::OptionsPricerCpp::DeltaMC(
			VanillaOptionParameters^% TheOption,
			Double Spot,
			Double Vol,
			Double r,
			UInt64 NumberOfPaths
		);
		Double ProjectXAnalyticsCppLib::OptionsPricerCpp::GammaMC(
			VanillaOptionParameters^% TheOption,
			Double Spot,
			Double Vol,
			Double r,
			UInt64 NumberOfPaths
		);
		Double ProjectXAnalyticsCppLib::OptionsPricerCpp::Gamma(
			VanillaOptionParameters^% TheOption,
			Double Spot,
			Double Vol,
			Double r,
			Double epsilon
		);
		Double ProjectXAnalyticsCppLib::OptionsPricerCpp::Rho(
			VanillaOptionParameters^% TheOption,
			Double Spot,
			Double Vol,
			Double r,
			UInt64 NumberOfPaths
		);
		Double ProjectXAnalyticsCppLib::OptionsPricerCpp::RhoMC(
			VanillaOptionParameters^% TheOption,
			Double Spot,
			Double Vol,
			Double r,
			UInt64 NumberOfPaths
		);
		Double ProjectXAnalyticsCppLib::OptionsPricerCpp::Theta(
			VanillaOptionParameters^% TheOption,
			Double Spot,
			Double Vol,
			Double r,
			UInt64 NumberOfPaths
		);
		Double ProjectXAnalyticsCppLib::OptionsPricerCpp::ThetaMC(
			VanillaOptionParameters^% TheOption,
			Double Spot,
			Double Vol,
			Double r,
			UInt64 NumberOfPaths,
			Double timeStep // Time step for simulation
		);
		Double ProjectXAnalyticsCppLib::OptionsPricerCpp::Vega(
			VanillaOptionParameters^% TheOption,
			Double Spot,
			Double Vol,
			Double r,
			UInt64 NumberOfPaths
		);
		Double ProjectXAnalyticsCppLib::OptionsPricerCpp::VegaMC(
			VanillaOptionParameters^% TheOption,
			Double Spot,
			Double Vol,
			Double r,
			UInt64 NumberOfPaths
		);
		Double ProjectXAnalyticsCppLib::OptionsPricerCpp::ImpliedVolatility(
			VanillaOptionParameters^% TheOption,
			Double Spot,
			Double r,
			UInt64 NumberOfPaths,
			Double optionPrice
		);
		Double ProjectXAnalyticsCppLib::OptionsPricerCpp::ImpliedVolatilityMC(
			VanillaOptionParameters^% TheOption,
			Double Spot,
			Double r,
			UInt64 NumberOfPaths,
			Double optionPrice
		);
	};
}