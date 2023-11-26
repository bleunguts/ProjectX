#include "../src/pch.h"
#include "HestonPricerTest.h"
#include <gtest/gtest.h>

namespace HestonPricerConstants
{
	// Protect against fuel prices by buying Long Call
	// 2009 fuel price was $1.9
	// 2019 rocketed to $2.97 would of lost 1.44 mio with notional of 2 mio
	// 2019 $1.39

	double spot = 2.0;   // $2 
	double strike = 2.0; // $2
	double r = 0.0319; // 3.19 %
	double q = 0;
	double T = 1.0; // 1 year to expiry
	unsigned int n_TimeSteps = 200;
	unsigned int m_Simulations = 200;

	double v0 = 0.010201; // starting volatility
	double theta = 0.019; // Long-term mean volatility
	double kappa = 6.21; // speed of reversion
	double sigma = 0.61;   // vol of vol        	

	VanillaOptionParameters callOption = VanillaOptionParameters(OptionType::Call, strike, T);	
}

using namespace HestonPricerConstants;

TEST_F(HestonPricerTest, HestonAPI1PV)
{	
	vector<double> rhoProbabilities{ 0.25, 0.5, 0.25 };
	vector<double> rhoChoices{ -0.5, -0.7, -0.9 };
	HestonStochasticVolalityParameters volParams = HestonStochasticVolalityParameters{ v0, theta, kappa, sigma, rhoProbabilities, rhoChoices };
	GreekResults results = api1->MCValue(callOption, spot, r, q, n_TimeSteps, m_Simulations, volParams);
	double PV = results.PV;
	double PVPut = results.PVPut;		
	EXPECT_NEAR(PV, 0.1421, 0.2) << "PV: " << PV << std::endl;
	EXPECT_NEAR(PVPut, 0.075, 0.4) << "PVPut: " << PVPut << std::endl;

	/*EXPECT_EQ(1, 2) << "callsCount: " << results.Debug.callsCount << " putsCount: " << results.Debug.putsCount << std::endl;
	EXPECT_EQ(1, 2) << "rhos: " << results.Debug.rhos << std::endl;
	EXPECT_EQ(1, 2) << "spots: " << results.Debug.spotGraph << std::endl;*/
}

TEST_F(HestonPricerTest, HestonAPI2PV)
{
	double rho = -0.7;     // correlation between brownian motions spot and vol	
	HestonStochasticVolalityParameters volParams2 = HestonStochasticVolalityParameters{ v0, theta, kappa, sigma, rho };

	GreekResults results = api2->MCValue(callOption, spot, r, q, n_TimeSteps, m_Simulations, volParams2);
	double PV = results.PV;
	double PVPut = results.PVPut;		
	EXPECT_NEAR(PV, 0.1421, 0.1) << "PV: " << PV << std::endl;
	EXPECT_NEAR(PVPut, 0.075, 0.4) << "PVPut: " << PVPut << std::endl;
}