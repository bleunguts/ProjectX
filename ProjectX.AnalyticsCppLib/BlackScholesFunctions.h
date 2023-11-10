#pragma once
using namespace System;

namespace ProjectXAnalyticsCppLib {
	public ref class BlackScholesFunctions abstract sealed
	{
	public:
		static Double ProjectXAnalyticsCppLib::BlackScholesFunctions::BlackScholesCall(
			Double S, // Current stock price
			Double K, // Strike price
			Double r, // Risk-free interest rate
			Double T, // Time to expiration
			Double sigma // Volatility
		);
		static Double ProjectXAnalyticsCppLib::BlackScholesFunctions::BlackScholesPut(
			Double S, // Current stock price
			Double K, // Strike price
			Double r, // Risk-free interest rate
			Double T, // Time to expiration
			Double sigma // Volatility
		);
		static Double ProjectXAnalyticsCppLib::BlackScholesFunctions::BlackScholesRhoCall(
			Double S, // Current stock price
			Double K, // Strike price
			Double r, // Risk-free interest rate
			Double T, // Time to expiration
			Double sigma // Volatility
		);
		static Double ProjectXAnalyticsCppLib::BlackScholesFunctions::BlackScholesRhoPut(
			Double S, // Current stock price
			Double K, // Strike price
			Double r, // Risk-free interest rate
			Double T, // Time to expiration
			Double sigma // Volatility
		);
		static Double ProjectXAnalyticsCppLib::BlackScholesFunctions::BlackScholesDeltaCall(
			Double S, // Current stock price
			Double K, // Strike price
			Double r, // Risk-free interest rate
			Double T, // Time to expiration		
			Double sigma, // Volatility
			Double optionPrice, // Option Price
			Double epsilon // Small change in stock price
		);
		static Double ProjectXAnalyticsCppLib::BlackScholesFunctions::BlackScholesDeltaPut(
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
		static Double ProjectXAnalyticsCppLib::BlackScholesFunctions::BlackScholesThetaCall(
			Double S, // Current stock price
			Double K, // Strike price
			Double r, // Risk-free interest rate
			Double T, // Time to expiration
			Double sigma // Volatility
		);
		static Double ProjectXAnalyticsCppLib::BlackScholesFunctions::BlackScholesThetaPut(
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