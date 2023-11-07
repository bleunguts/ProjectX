#include "pch.h"

#include "ProjectX.AnalyticsCppLib.h"
#include "Parameters.h"
#include "BlackScholesFunctions.h"
#include <cmath>

Double ProjectXAnalyticsCppLib::OptionsPricingCppCalculator::MCValue(VanillaOptionParameters^% OptionParams,
	Double Spot,
	Double Vol,
	Double r,
	UInt64 NumberOfPaths)
{
	PayOffBridge* payOffBridge = nullptr;
	switch (OptionParams->OptionType())
	{
	case OptionType::Call:
	{
		PayOffCall call = PayOffCall(OptionParams->Strike());
		payOffBridge = new PayOffBridge(call);
		break;
	}
	case OptionType::Put:
	{
		PayOffPut put = PayOffPut(OptionParams->Strike());
		payOffBridge = new PayOffBridge(put);
		break;
	}
	default:
		throw gcnew System::String("Shouldnt get here");
	}

	VanillaOption TheOption = VanillaOption(*payOffBridge, OptionParams->Expiry());
	ParametersConstant vol = ParametersConstant(Vol);
	ParametersConstant rate = ParametersConstant(r);
	double Expiry = TheOption.GetExpiry();
	double variance = vol.IntegralSquare(0, Expiry);
	double rootVariance = sqrt(variance);
	double itoCorrection = -0.5 * variance;
	double movedSpot = Spot * exp(rate.Integral(0, Expiry) + itoCorrection);
	double thisSpot;
	double runningSum = 0;

	for (unsigned long i = 0; i < NumberOfPaths; i++)
	{
		double thisGaussian = m_randomWalk->GetOneGaussian();
		thisSpot = movedSpot * exp(rootVariance * thisGaussian);
		double thisPayOff = TheOption.OptionPayOff(thisSpot);
		runningSum += thisPayOff;
	}

	double mean = runningSum / NumberOfPaths;
	mean *= exp(-rate.Integral(0, Expiry));

	if (payOffBridge != NULL) {
		delete payOffBridge;
		payOffBridge = NULL;
	}
	return mean;
}

Double ProjectXAnalyticsCppLib::OptionsPricingCppCalculator::Delta(VanillaOptionParameters^% TheOption,
	Double Spot,
	Double Vol,
	Double r,	
	UInt64 NumberOfPaths
) 
{
	double epsilon = 0.01; // Small change in stock price
	// Calculate the option price using Monte Carlo simulation
	double optionPrice = MCValue(TheOption, Spot, Vol, r, NumberOfPaths);

	// Calculate delta using Black-Scholes formula	
	double delta = BlackScholesFunctions::BlackScholesDelta(Spot, TheOption->Strike(), r, TheOption->Expiry(), optionPrice, epsilon, Vol);
	return delta;
}

Double ProjectXAnalyticsCppLib::OptionsPricingCppCalculator::DeltaMC(VanillaOptionParameters^% TheOption,
	Double Spot,
	Double Vol,
	Double r,	
	UInt64 NumberOfPaths // Number of Monte Carlo simulations
)
{
	double S = Spot;
	double sigma = Vol;
	double T = TheOption->Expiry();
	double K = TheOption->Strike();
	double epsilon = 0.01; // Small change in stock price
	double deltaSum = 0.0;

	for (int i = 0; i < NumberOfPaths; i++)
	{
		double randNorm = RandomStandardNormal();
		double ST = S * Math::Exp((r - (Math::Pow(sigma, 2) / 2)) * T + sigma * Math::Sqrt(T) * randNorm);
		double payoff = Math::Max(ST - K, 0.0);
		double priceUp = Math::Exp(-r * T) * payoff;

		ST = S * Math::Exp((r - (Math::Pow(sigma, 2) / 2)) * T - sigma * Math::Sqrt(T) * randNorm);
		payoff = Math::Max(ST - K, 0.0);
		double priceDown = Math::Exp(-r * T) * payoff;

		deltaSum += (priceUp - priceDown) / (2.0 * epsilon);
	}

	return deltaSum / NumberOfPaths;
}

Double ProjectXAnalyticsCppLib::OptionsPricingCppCalculator::Gamma(VanillaOptionParameters^% TheOption,
	Double Spot,
	Double Vol,
	Double r,
	Double epsilon
) 
{
	double gamma = BlackScholesFunctions::BlackScholesGamma(Spot, TheOption->Strike(), r, TheOption->Expiry(), Vol, epsilon);
	return gamma;
}


// Function to calculate Gamma using Monte Carlo simulation
Double ProjectXAnalyticsCppLib::OptionsPricingCppCalculator::GammaMC(
	VanillaOptionParameters^% TheOption,
	Double Spot,
	Double Vol,
	Double r,		
	UInt64 NumberOfPaths // Number of Monte Carlo simulations
)
{
	double S = Spot;
	double sigma = Vol;
	double T = TheOption->Expiry();
	double K = TheOption->Strike();
	double epsilon = 0.01; // Small change in stock price
	double gammaSum = 0.0;

	for (int i = 0; i < NumberOfPaths; i++)
	{
		double randNorm = RandomStandardNormal();
		double ST = S * Math::Exp((r - (Math::Pow(sigma, 2) / 2)) * T + sigma * Math::Sqrt(T) * randNorm);
		double payoff = Math::Max(ST - K, 0.0);
		double priceUp = Math::Exp(-r * T) * payoff;

		ST = S * Math::Exp((r - (Math::Pow(sigma, 2) / 2)) * T - sigma * Math::Sqrt(T) * randNorm);
		payoff = Math::Max(ST - K, 0.0);
		double priceDown = Math::Exp(-r * T) * payoff;

		gammaSum += ((priceUp - priceDown) / (2.0 * epsilon)) / (2.0 * S * epsilon);
	}

	return gammaSum / NumberOfPaths;
}


Double ProjectXAnalyticsCppLib::OptionsPricingCppCalculator::Rho(VanillaOptionParameters^% TheOption,
	Double Spot,
	Double Vol,
	Double r,	
	UInt64 NumberOfPaths
) 
{
	double optionPrice = MCValue(TheOption, Spot, Vol, r, NumberOfPaths);
	
	double rho = BlackScholesFunctions::BlackScholesRho( Spot, TheOption->Strike(), r, TheOption->Expiry(), Vol);
	return rho;
}

// Function to calculate Rho using Monte Carlo simulation
Double ProjectXAnalyticsCppLib::OptionsPricingCppCalculator::RhoMC(VanillaOptionParameters^% TheOption,
	Double Spot,
	Double Vol,
	Double r,
	UInt64 NumberOfPaths
)
{
	double S = Spot;
	double sigma = Vol;
	double T = TheOption->Expiry();
	double K = TheOption->Strike();
	double epsilon = 0.01; // Small change in interest rate
	double rhoSum = 0.0;

	for (int i = 0; i < NumberOfPaths; i++)
	{
		double randNorm = RandomStandardNormal();
		double ST = S * Math::Exp((r - (Math::Pow(sigma, 2) / 2)) * T + sigma * Math::Sqrt(T) * randNorm);
		double payoff = Math::Max(ST - K, 0.0);
		double priceUp = Math::Exp(-(r + epsilon) * T) * payoff;

		rhoSum += (priceUp - BlackScholesFunctions::BlackScholes(S, K, r, T, sigma)) / (epsilon * r);
	}

	return rhoSum / NumberOfPaths;
}
Double ProjectXAnalyticsCppLib::OptionsPricingCppCalculator::Theta(VanillaOptionParameters^% TheOption,
	Double Spot,
	Double Vol,
	Double r,
	UInt64 NumberOfPaths
) 
{
	double optionPrice = MCValue(TheOption, Spot, Vol, r, NumberOfPaths);

	double theta = BlackScholesFunctions::BlackScholesTheta(Spot, TheOption->Strike(), r, TheOption->Expiry(), Vol);
	return theta;
}
Double ProjectXAnalyticsCppLib::OptionsPricingCppCalculator::ThetaMC(
	VanillaOptionParameters^% TheOption,
	Double Spot,
	Double Vol,
	Double r,
	UInt64 NumberOfPaths,
	Double timeStep // Time step for simulation
)
{
	Double S = Spot;
	Double sigma = Vol;
	Double T = TheOption->Expiry();
	Double K = TheOption->Strike();
	Double thetaSum = 0.0;

	for (int i = 0; i < NumberOfPaths; i++)
	{
		Double t = 0;
		Double ST = S;
		Double payoff = Math::Max(ST - K, 0.0);
		Double price = Math::Exp(-r * t) * payoff;

		while (t < T)
		{
			Double randNorm = RandomStandardNormal();
			ST = ST * Math::Exp((r - (Math::Pow(sigma, 2) / 2)) * timeStep + sigma * Math::Sqrt(timeStep) * randNorm);
			t += timeStep;

			payoff = Math::Max(ST - K, 0.0);
			price = Math::Exp(-r * t) * payoff;
		}		
		Double PV = BlackScholesFunctions::BlackScholes(S, K, r, T - t, sigma);
		thetaSum += (price - PV);
	}

	return thetaSum / NumberOfPaths;
}


Double ProjectXAnalyticsCppLib::OptionsPricingCppCalculator::Vega(VanillaOptionParameters^% TheOption,
	Double Spot,
	Double Vol,
	Double r,
	UInt64 NumberOfPaths
)
{
	double optionPrice = MCValue(TheOption, Spot, Vol, r, NumberOfPaths);

	double vega = BlackScholesFunctions::BlackScholesVega(Spot, TheOption->Strike(), r, TheOption->Expiry(), Vol);
	return vega;
}

// Function to calculate Vega using Monte Carlo simulation
Double ProjectXAnalyticsCppLib::OptionsPricingCppCalculator::VegaMC(VanillaOptionParameters^% TheOption,
	Double Spot,
	Double Vol,
	Double r,
	UInt64 NumberOfPaths
)
{
	double S = Spot;
	double sigma = Vol;
	double T = TheOption->Expiry();
	double K = TheOption->Strike();
	double epsilon = 0.01; // Small change in volatility
	double vegaSum = 0.0;

	for (int i = 0; i < NumberOfPaths; i++)
	{
		double randNorm = RandomStandardNormal();
		double ST = S * Math::Exp((r - (Math::Pow(sigma + epsilon, 2) / 2)) * T + (sigma + epsilon) * Math::Sqrt(T) * randNorm);
		double payoff = Math::Max(ST - K, 0.0);
		double priceUp = Math::Exp(-r * T) * payoff;

		vegaSum += (priceUp - BlackScholesFunctions::BlackScholes(S, K, r, T, sigma)) / epsilon;
	}

	return vegaSum / NumberOfPaths;
}

// Function to calculate implied volatility using a Monte Carlo simulation
Double ProjectXAnalyticsCppLib::OptionsPricingCppCalculator::ImpliedVolatilityMC(VanillaOptionParameters^% TheOption,
	Double Spot,	
	Double r,	
	UInt64 NumberOfPaths,
	Double optionPrice)
{
	double T = TheOption->Expiry();
	double K = TheOption->Strike();
	double S = Spot;
	double sigma = 0.2; // Initial guess for implied volatility
	double epsilon = 0.01; // Small change in volatility
	double vega, price;

	for (int i = 0; i < NumberOfPaths; i++)
	{
		double randNorm = RandomStandardNormal();
		double ST = S * Math::Exp((r - (Math::Pow(sigma, 2) / 2)) * T + sigma * Math::Sqrt(T) * randNorm);
		double payoff = Math::Max(ST - K, 0.0);
		price = Math::Exp(-r * T) * payoff;

		vega = (price - optionPrice) / epsilon;

		if (Math::Abs(price - optionPrice) < epsilon)
		{
			return sigma;
		}

		sigma -= (price - optionPrice) / vega;
	}

	// If the maximum number of simulations is reached, return NaN (no solution found).
	return Double::NaN;
}

