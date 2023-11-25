#pragma once

namespace ProjectXAnalyticsCppLib {
	class BlackScholesFunctions 
	{
	public:
		static double BlackScholesCall(
			double S, // Current stock price
			double K, // Strike price
			double r, // Risk-free interest rate
			double T, // Time to expiration
			double sigma // Volatility
		);
		static double BlackScholesPut(
			double S, // Current stock price
			double K, // Strike price
			double r, // Risk-free interest rate
			double T, // Time to expiration
			double sigma // Volatility
		);
		static double BlackScholesRhoCall(
			double S, // Current stock price
			double K, // Strike price
			double r, // Risk-free interest rate
			double T, // Time to expiration
			double sigma // Volatility
		);
		static double BlackScholesRhoPut(
			double S, // Current stock price
			double K, // Strike price
			double r, // Risk-free interest rate
			double T, // Time to expiration
			double sigma // Volatility
		);
		static double BlackScholesDeltaCall(
			double S, // Current stock price
			double K, // Strike price
			double r, // Risk-free interest rate
			double T, // Time to expiration		
			double sigma // Volatility			
		);
		static double BlackScholesDeltaPut(
			double S, // Current stock price
			double K, // Strike price
			double r, // Risk-free interest rate
			double T, // Time to expiration		
			double sigma // Volatility						
		);
		static double BlackScholesGamma(
			double S, // Current stock price
			double K, // Strike price
			double r, // Risk-free interest rate
			double T, // Time to expiration
			double sigma, // Volatility
			double epsilon // Small change in stock price
		);
		static double BlackScholesThetaCall(
			double S, // Current stock price
			double K, // Strike price
			double r, // Risk-free interest rate
			double T, // Time to expiration
			double sigma // Volatility
		);
		static double BlackScholesThetaPut(
			double S, // Current stock price
			double K, // Strike price
			double r, // Risk-free interest rate
			double T, // Time to expiration
			double sigma // Volatility
		);
		static double BlackScholesVega(
			double S, // Current stock price
			double K, // Strike price
			double r, // Risk-free interest rate
			double T, // Time to expiration
			double sigma // Volatility
		);		
		static double d1(
			double S,
			double K,
			double r,
			double sigma,
			double T
		);
		static double d2(
			double d1,
			double sigma,
			double T
		);
	};
}