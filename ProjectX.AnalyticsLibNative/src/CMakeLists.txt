# CMakeList.txt : CMake project for ProjectX.AnalyticsLibNative, include source and define
# project specific logic here.
#
cmake_minimum_required (VERSION 3.8)

set(CMAKE_RUNTIME_OUTPUT_DIRECTORY ${CMAKE_BINARY_DIR}/bin)
set(CMAKE_LIBRARY_OUTPUT_DIRECTORY ${CMAKE_BINARY_DIR}/bin)
project ("ProjectX.AnalyticsLibNative")

# Add source to this project's dll.
set(Headers 
"API.h"
"pch.h"
"Utility.h" 
"types/Debug.h"
"types/GreekResults.h"
"types/OptionType.h"
"types/VanillaOptionsParameters.h"
"types/HestonStochasticVolatilityParameters.h"
"Parameters.h"
"PayOff.h"
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
"pch.cpp"
"types/Debug.cpp"
"Parameters.cpp"
"PayOff.cpp"
"VanillaOption.cpp"
"RandomWalk.cpp"
"BlackScholesFunctions.cpp"
"MonteCarloCppPricer.cpp"
"BlackScholesCppPricer.cpp"
"MonteCarloHestonCppPricer.cpp"
"MonteCarloHestonCppPricer2.cpp"
)
add_library (ProjectX.AnalyticsLibNative SHARED ${Sources} ${Headers} )

add_custom_command(TARGET ProjectX.AnalyticsLibNative POST_BUILD
    COMMAND ${CMAKE_COMMAND} -E copy
        "$<TARGET_FILE:ProjectX.AnalyticsLibNative>"
        "${CMAKE_SOURCE_DIR}/../3rdParty")