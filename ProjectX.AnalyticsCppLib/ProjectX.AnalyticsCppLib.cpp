#include "pch.h"

#include "ProjectX.AnalyticsCppLib.h"
#include <cmath>

Double ProjectXAnalyticsCppLib::OptionsPricingCppCalculator::MCValue(VanillaOptionParameters^ %OptionParams,
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
		{
			throw gcnew System::String("Shouldnt get here");
		}
	}		
	VanillaOption TheOption = VanillaOption(*payOffBridge, OptionParams->Expiry());

	double Expiry = TheOption.GetExpiry();
	double variance = MathFunctions::IntegralSquare(Vol, 0, Expiry);		
	double rootVariance = sqrt(variance);
	double itoCorrection = -0.5 * variance;
	double movedSpot = Spot * exp(MathFunctions::Integral(r, 0, Expiry) + itoCorrection);
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
	mean *= exp(-MathFunctions::Integral(r, 0, Expiry));

	if (payOffBridge != NULL) {
		delete payOffBridge;
		payOffBridge = NULL;
	}	

	return mean;	
}
