# CMakeList.txt : CMake project for ProjectX.AnalyticsLibNative, include source and define
# project specific logic here.
#
cmake_minimum_required (VERSION 3.8)

set(CMAKE_RUNTIME_OUTPUT_DIRECTORY ${CMAKE_BINARY_DIR}/bin)
project ("ProjectX.AnalyticsLibNative")

# Add source to this project's dll.
set(Headers 
"API.h"
#"pch.h"
"Utility.h" 
"types/Debug.h"
"types/GreekResults.h"
"types/OptionType.h"
"types/VanillaOptionsParameters.h"
"types/HestonStochasticVolatilityParameters.h"
"Parameters.h"
"Payoff.h"
"VanillaOption.h"
"RandomWalk.h"
"BlackScholesFunctions.h"
"MonteCarloCppPricer.h"
"BlackScholesCppPricer.h"
"MonteCarloHestonCppPricer.h"
"MonteCarloHestonCppPricer2.h"
)
set(Sources 
"API.cpp"
#"pch.cpp"
"types/Debug.cpp"
"Parameters.cpp"
"Payoff.cpp"
"VanillaOption.cpp"
"RandomWalk.cpp"
"BlackScholesFunctions.cpp"
"MonteCarloCppPricer.cpp"
"BlackScholesCppPricer.cpp"
"MonteCarloHestonCppPricer.cpp"
"MonteCarloHestonCppPricer2.cpp"
)
add_library (ProjectX.AnalyticsLibNative SHARED ${Sources} ${Headers} )