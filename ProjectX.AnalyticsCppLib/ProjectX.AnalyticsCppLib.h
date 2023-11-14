#pragma once

using namespace System;
using namespace System::Collections::Generic;

namespace ProjectXAnalyticsCppLib {		
	public value struct Debug 
	{
	public:
		Debug(int calls, int puts, List<double>^ rhos, Dictionary<double, List<double>^>^ spots, int simulations)
		{
			this->callsCount = calls;
			this->putsCount = puts;
			this->rhos = rhos;
			this->spotGraph = spots;
			this->totalSimulations = simulations;
		}
	public:
		int callsCount;
		int putsCount;
		List<double>^ rhos;
		Dictionary<double, List<double>^>^ spotGraph;
		int totalSimulations;
	};

	public value struct GreekResults
	{
	public:
		GreekResults(Double pv, Double pvPut, Debug debug) 
		{
			this->PV = pv;
			this->PVPut = pvPut;
			this->Debug = debug;
		}
		GreekResults(Double pv, Double pvPut, Double delta,Double deltaPut,  Double gamma, Double vega, Double rho, Double rhoPut, Double theta, Double thetaPut)
		{	
			this->PV = pv;
			this->PVPut = pvPut;
			this->Delta = delta;
			this->DeltaPut = deltaPut;
			this->Gamma = gamma;
			this->Vega = vega;
			this->Rho = rho;
			this->RhoPut = rhoPut;
			this->Theta = theta;
			this->ThetaPut = thetaPut;
		}
	public:
		Double PV;
	 	Double PVPut;
		Nullable<Double> Delta;
		Nullable<Double> DeltaPut;
		Nullable<Double> Gamma;
		Nullable<Double> Vega;
		Nullable<Double> Rho;
		Nullable<Double> RhoPut;
		Nullable<Double> Theta;
		Nullable<Double> ThetaPut;
		Nullable<Debug> Debug;		
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

	public interface class IMonteCarloCppPricer
	{
		GreekResults MCValue(
			VanillaOptionParameters^% TheOption,
			Double Spot,
			Double Vol,
			Double r,
			UInt64 NumberOfPaths);
		Double ImpliedVolatilityMC(
			VanillaOptionParameters^% TheOption,
			Double Spot,
			Double r,
			UInt64 NumberOfPaths,
			Double optionPrice
		);
	};
}
