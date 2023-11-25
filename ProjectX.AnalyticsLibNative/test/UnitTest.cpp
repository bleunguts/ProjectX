#include <gtest/gtest.h>
#include "..\src\ProjectX.AnalyticsLibNative.h"

TEST(ApiTest, ExecuteAPI)
{
	API* api = new API();
	api->execute();
	EXPECT_EQ(true, true);
}