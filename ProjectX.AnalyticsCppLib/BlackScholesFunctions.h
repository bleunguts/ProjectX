#pragma once
using namespace System;

namespace ProjectXAnalyticsCppLib {
	public ref class BlackScholesFunctions abstract sealed
	{
	public:
		static Double ProjectXAnalyticsCppLib::BlackScholesFunctions::BlackScholes(
			Double S, // Current stock price
			Double K, // Strike price
			Double r, // Risk-free interest rate
			Double T, // Time to expiration
			Double sigma // Volatility
		);
		static Double ProjectXAnalyticsCppLib::BlackScholesFunctions::BlackScholesRho(
			Double S, // Current stock price
			Double K, // Strike price
			Double r, // Risk-free interest rate
			Double T, // Time to expiration
			Double sigma // Volatility
		);
		static Double ProjectXAnalyticsCppLib::BlackScholesFunctions::BlackScholesDelta(
			Double S, // Current stock price
			Double K, // Strike price
			Double r, // Risk-free interest rate
			Double T, // Time to expiration		
			Double sigma, // Volatility
			Double optionPrice, // Option Price
			Double epsilon // Small change in stock price
		);
		static Double ProjectXAnalyticsCppLib::BlackScholesFunctions::BlackScholesGamma(
			Double S, // Current stock price
			Double K, // Strike price
			Double r, // Risk-free interest rate
			Double T, // Time to expiration
			Double sigma, // Volatility
			Double epsilon // Small change in stock price
		);
		static Double ProjectXAnalyticsCppLib::BlackScholesFunctions::BlackScholesTheta(
			Double S, // Current stock price
			Double K, // Strike price
			Double r, // Risk-free interest rate
			Double T, // Time to expiration
			Double sigma // Volatility
		);
		static Double ProjectXAnalyticsCppLib::BlackScholesFunctions::BlackScholesVega(
			Double S, // Current stock price
			Double K, // Strike price
			Double r, // Risk-free interest rate
			Double T, // Time to expiration
			Double sigma // Volatility
		);		
		static Double ProjectXAnalyticsCppLib::BlackScholesFunctions::d1(
			double S,
			double K,
			double r,
			double sigma,
			double T
		);
		static Double ProjectXAnalyticsCppLib::BlackScholesFunctions::d2(
			double d1,
			double sigma,
			double T
		);
	};
}