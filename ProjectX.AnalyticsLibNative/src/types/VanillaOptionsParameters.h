#pragma once
#include "../pch.h"

namespace ProjectXAnalyticsCppLib {
	class VanillaOptionParameters
	{
	private:
		ProjectXAnalyticsCppLib::OptionType m_optionType;
		double m_strike;
		double m_expiry;

	public:
		VanillaOptionParameters(ProjectXAnalyticsCppLib::OptionType optionType, double strike, double expiry)
		{
			m_optionType = optionType;
			m_strike = strike;
			m_expiry = expiry;
		};
		inline ProjectXAnalyticsCppLib::OptionType OptionType() { return m_optionType; };
		inline double Strike() { return m_strike; };
		inline double Expiry() { return m_expiry; };
	};
}