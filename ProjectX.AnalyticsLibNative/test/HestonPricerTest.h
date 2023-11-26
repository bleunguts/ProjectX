#pragma once
#include "../src/MonteCarloHestonCppPricer.h"
#include "../src/MonteCarloHestonCppPricer2.h"
#include <gtest/gtest.h>

using namespace ProjectXAnalyticsCppLib;

class HestonPricerTest : public testing::Test {
protected:
    // You can remove any or all of the following functions if their bodies would
    // be empty.
    MonteCarloHestonCppPricer* api1;
    MonteCarloHestonCppPricer2* api2;

    HestonPricerTest() {
        // You can do set-up work for each test here.
        api1 = NULL;
        api2 = NULL;
    }

    ~HestonPricerTest() override {
        // You can do clean-up work that doesn't throw exceptions here.
    }

    // If the constructor and destructor are not enough for setting up
    // and cleaning up each test, you can define the following methods:

    void SetUp() override {
        // Code here will be called immediately after the constructor (right
        // before each test).
        api1 = new MonteCarloHestonCppPricer();
        api2 = new MonteCarloHestonCppPricer2();
    }

    void TearDown() override {
        // Code here will be called immediately after each test (right
        // before the destructor).
        if (api1 == NULL)
        {
            delete api1;
        }
        if (api2 == NULL)
        {
            delete api2;
        }
    }

    // Class members declared here can be used by all tests in the test suite
    // for Foo.
};

