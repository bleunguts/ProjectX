#pragma once
#include "..\pch.h"

namespace ProjectXAnalyticsCppLib {
	class VanillaOptionParameters
	{
	private:
		OptionType m_optionType;
		double m_strike;
		double m_expiry;

	public:
		VanillaOptionParameters(OptionType optionType, double strike, double expiry)
		{
			m_optionType = optionType;
			m_strike = strike;
			m_expiry = expiry;
		};
		inline OptionType OptionType() { return m_optionType; };
		inline double Strike() { return m_strike; };
		inline double Expiry() { return m_expiry; };
	};
}