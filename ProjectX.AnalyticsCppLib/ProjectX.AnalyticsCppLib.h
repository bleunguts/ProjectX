#pragma once

using namespace System;

namespace ProjectXAnalyticsCppLib {		
	public ref struct GreekResults
	{
	public:
		GreekResults(Double pv, Double delta, Double gamma, Double vega, Double rho, Double theta)
		{
			this->PV = pv;
			this->Delta = delta;
			this->Gamma = gamma;
			this->Vega = vega;
			this->Rho = rho;
			this->Theta = theta;
		}
	public:
		Double PV;
		Double Delta;
		Double Gamma;
		Double Vega;
		Double Rho;
		Double Theta;
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
}
