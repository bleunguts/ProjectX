# CMakeList.txt : CMake project for ProjectX.AnalyticsLibNative, include source and define
# project specific logic here.
#
cmake_minimum_required (VERSION 3.8)
add_definitions(-DBUILD_DLL)

# Enable Hot Reload for MSVC compilers if supported.
if (POLICY CMP0141)
  cmake_policy(SET CMP0141 NEW)
  set(CMAKE_MSVC_DEBUG_INFORMATION_FORMAT "$<IF:$<AND:$<C_COMPILER_ID:MSVC>,$<CXX_COMPILER_ID:MSVC>>,$<$<CONFIG:Debug,RelWithDebInfo>:EditAndContinue>,$<$<CONFIG:Debug,RelWithDebInfo>:ProgramDatabase>>")
endif()

set(CMAKE_RUNTIME_OUTPUT_DIRECTORY ${CMAKE_BINARY_DIR}/bin)
project ("ProjectX.AnalyticsLibNative")

enable_testing()

# Add source to this project's dll.
set(Headers 
"ProjectX.AnalyticsLibNative.h"
"pch.h"
"Utility.h" 
"Parameters.h"
"Payoff.h"
"VanillaOption.h"
"RandomWalk.h"
"BlackScholesFunctions.h"
"MonteCarloCppPricer.h"
)
set(Sources 
"ProjectX.AnalyticsLibNative.cpp"
"pch.cpp"
"Parameters.cpp"
"Payoff.cpp"
"VanillaOption.cpp"
"RandomWalk.cpp"
"BlackScholesFunctions.cpp"
"MonteCarloCppPricer.cpp"
)
add_library (ProjectX.AnalyticsLibNative SHARED ${Sources} ${Headers})

if (CMAKE_VERSION VERSION_GREATER 3.12)
  set_property(TARGET ProjectX.AnalyticsLibNative PROPERTY CXX_STANDARD 20)
endif()

# TODO: Add tests and install targets if needed.
add_subdirectory(test)