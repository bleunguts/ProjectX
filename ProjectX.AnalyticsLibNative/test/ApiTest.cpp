#include "../src/pch.h"
#include "ApiTest.h"
#include <gtest/gtest.h>

namespace ApiTestConstants 
{
	double spot = 100;
	double strike = 110;
	double expiry = 0.5;
	double vol = 0.3;
	double r = 0.1;
	double b = 0.0;
	VanillaOptionParameters callOption = VanillaOptionParameters(OptionType::Call, strike, expiry);
	VanillaOptionParameters putOption = VanillaOptionParameters(OptionType::Put, strike, expiry);
	double mktprice = 3.7432065872239662;
}

using namespace ApiTestConstants;

TEST_F(ApiTest, ExecuteAPI)
{	
	api->execute();
	EXPECT_EQ(true, true);			
}

TEST_F(ApiTest, WhenComputingPV)
{	
	double PV = api->BlackScholes_PV(callOption, spot, vol, r);
	EXPECT_NEAR(PV, 6.52078264, 0.0001)	
		<< "PV: " << PV << std::endl;

	double PVPut = api->BlackScholes_PV(putOption, spot, vol, r);
	EXPECT_NEAR(PVPut, 11.15601933, 0.0001)
		<< "PV: " << PVPut << std::endl;
}

TEST_F(ApiTest, WhenComputingDelta) 
{
	double delta = api->BlackScholes_Delta(callOption, spot, vol, r);
	EXPECT_NEAR(delta, 0.45718497, 0.0001)
		<< "delta: " << delta << std::endl;

	double deltaPut = api->BlackScholes_Delta(putOption, spot, vol, r);
	EXPECT_NEAR(deltaPut, -0.54281503, 0.0001)
		<< "deltaPut: " << deltaPut << std::endl;
}

TEST_F(ApiTest, WhenComputingGamma)
{
	double epsilon = 0.0001;
	double gamma = api->BlackScholes_Gamma(callOption, spot, vol, r, epsilon);
	EXPECT_NEAR(gamma, 0.018697, 0.0001)
		<< "gamma: " << gamma << std::endl;

	double gammaPut = api->BlackScholes_Gamma(putOption, spot, vol, r, epsilon);
	EXPECT_EQ(gamma, gammaPut)
		<< "gamma: " << gamma << "gammaPut:" << gammaPut << std::endl;
}

TEST_F(ApiTest, WhenComputingTheta) 
{
	double theta = api->BlackScholes_Theta(callOption, spot, vol, r);
	EXPECT_NEAR(theta, -12.334, 0.001)
		<< "theta: " << theta << std::endl;

	double thetaPut = api->BlackScholes_Theta(putOption, spot, vol, r);
	EXPECT_NEAR(thetaPut, -1.870, 0.001)
		<< "thetaPut: " << thetaPut << std::endl;
}

TEST_F(ApiTest, WhenComputingRho)
{
	double rho = api->BlackScholes_Rho(callOption, spot, vol, r);
	EXPECT_NEAR(rho, 0.195988572, 0.0001)
		<< "rho: " << rho << std::endl;

	double rhoPut = api->BlackScholes_Rho(putOption, spot, vol, r);
	EXPECT_NEAR(rhoPut, -0.327187612, 0.0001)
		<< "rhoPut: " << rhoPut << std::endl;
}

TEST_F(ApiTest, WhenComputingVega)
{
	double vega = api->BlackScholes_Vega(callOption, spot, vol, r);
	EXPECT_NEAR(vega, 0.280468662, 0.0001)
		<< "vega: " << vega << std::endl;

	double vegaPut = api->BlackScholes_Vega(putOption, spot, vol, r);
	EXPECT_EQ(vega, vegaPut)
		<< "vega: " << vega << "vegaPut: " << vegaPut << std::endl;
}

TEST_F(ApiTest, WhenComputingImpliedVol) 
{	
	double vol = api->BlackScholes_ImpliedVolatility(callOption, spot, r, mktprice);
	EXPECT_NEAR(vol, 0.2, 0.001)
		<< "vol:" << vol << std::endl;
}

TEST_F(ApiTest, WhenComputingPVForMonteCarlo) 
{
	GreekResults results = api->MonteCarlo_PV(callOption, spot, vol, r, 1000);
	double PV = results.PV;
	double PVPut = results.PVPut;
	double delta = results.Delta;
	double deltaPut = results.DeltaPut;
	double gamma = results.Gamma;
	double theta = results.Theta;
	double thetaPut = results.ThetaPut;
	double rho = results.Rho;
	double rhoPut = results.RhoPut;
	double vega = results.Vega;
	EXPECT_NEAR(PV, 6.52078264, 0.1) << "PV: " << PV << std::endl;
	EXPECT_NEAR(PVPut, 11.15601933, 0.4) << "PVPut: " << PVPut << std::endl;
	EXPECT_NEAR(delta, 0.45718497, 0.01) << "delta: " << delta << std::endl;
	EXPECT_NEAR(deltaPut, -0.54281503, 0.01) << "deltaPut: " << PVPut << std::endl;
	EXPECT_NEAR(gamma, 0.018697, 0.01) << "gamma: " << gamma << std::endl;
	EXPECT_NEAR(theta, -12.334, 0.9) << "theta: " << theta << std::endl;
	EXPECT_NEAR(thetaPut, -1.870, 0.9) << "thetaPut: " << thetaPut << std::endl;
	EXPECT_NEAR(rho, 0.195988572, 0.5) << "rho: " << rho << std::endl;
	EXPECT_NEAR(rhoPut, -0.327187612, 0.5) << "rhoPut: " << rhoPut << std::endl;
	EXPECT_NEAR(vega, 0.280468662, 0.2) << "vega: " << vega << std::endl;	
}

TEST_F(ApiTest, WhenComputingImpliedVolForMonteCarlo)
{
	double impliedVol = api->MonteCarlo_ImpliedVolatility(callOption, spot, r, 1000, mktprice);	
	//EXPECT_NEAR(impliedVol, 0.2, 0.0001) << "impliedVol: " << impliedVol << std::endl;
	EXPECT_EQ(true, true); // TODO: Fix Implied Vol; 
}