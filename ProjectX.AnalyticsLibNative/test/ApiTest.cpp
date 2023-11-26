#include "ApiTest.h"
#include <gtest/gtest.h>

double spot = 100;
double strike = 110;
double expiry = 0.5;
double vol = 0.3;
double r = 0.1;
VanillaOptionParameters callOption = VanillaOptionParameters(OptionType::Call, strike, expiry);
VanillaOptionParameters putOption = VanillaOptionParameters(OptionType::Put, strike, expiry);

TEST_F(ApiTest, ExecuteAPI)
{	
	api->execute();
	EXPECT_EQ(true, true);			
}

TEST_F(ApiTest, WhenComputingPV)
{	
	double PV = api->Value(callOption, spot, vol, r);
	EXPECT_NEAR(PV, 6.52078264, 0.0001)	
		<< "PV: " << PV << std::endl;

	double PVPut = api->Value(putOption, spot, vol, r);
	EXPECT_NEAR(PVPut, 11.15601933, 0.0001)
		<< "PV: " << PVPut << std::endl;
}

TEST_F(ApiTest, WhenComputingDelta) 
{
	double delta = api->Delta(callOption, spot, vol, r);
	EXPECT_NEAR(delta, 0.45718497, 0.0001)
		<< "delta: " << delta << std::endl;

	double deltaPut = api->Delta(putOption, spot, vol, r);
	EXPECT_NEAR(deltaPut, -0.54281503, 0.0001)
		<< "deltaPut: " << deltaPut << std::endl;
}

TEST_F(ApiTest, WhenComputingGamma)
{
	double epsilon = 0.0001;
	double gamma = api->Gamma(callOption, spot, vol, r, epsilon);
	EXPECT_NEAR(gamma, 0.018697, 0.0001)
		<< "gamma: " << gamma << std::endl;

	double gammaPut = api->Gamma(putOption, spot, vol, r, epsilon);
	EXPECT_EQ(gamma, gammaPut)
		<< "gamma: " << gamma << "gammaPut:" << gammaPut << std::endl;
}

TEST_F(ApiTest, WhenComputingTheta) 
{
	double theta = api->Theta(callOption, spot, vol, r);
	EXPECT_NEAR(theta, -12.334, 0.001)
		<< "theta: " << theta << std::endl;

	double thetaPut = api->Theta(putOption, spot, vol, r);
	EXPECT_NEAR(thetaPut, -1.870, 0.001)
		<< "thetaPut: " << thetaPut << std::endl;
}

TEST_F(ApiTest, WhenComputingRho)
{
	double rho = api->Rho(callOption, spot, vol, r);
	EXPECT_NEAR(rho, 0.195988572, 0.0001)
		<< "rho: " << rho << std::endl;

	double rhoPut = api->Rho(putOption, spot, vol, r);
	EXPECT_NEAR(rhoPut, -0.327187612, 0.0001)
		<< "rhoPut: " << rhoPut << std::endl;
}

TEST_F(ApiTest, WhenComputingVega)
{
	double vega = api->Vega(callOption, spot, vol, r);
	EXPECT_NEAR(vega, 0.280468662, 0.0001)
		<< "vega: " << vega << std::endl;

	double vegaPut = api->Vega(putOption, spot, vol, r);
	EXPECT_EQ(vega, vegaPut)
		<< "vega: " << vega << "vegaPut: " << vegaPut << std::endl;
}

TEST_F(ApiTest, WhenComputingImpliedVol) 
{
	double mktprice = 3.7432065872239662;
	double vol = api->ImpliedVolatility(callOption, spot, r, mktprice);
	EXPECT_NEAR(vol, 0.2, 0.001)
		<< "vol:" << vol << std::endl;
}