cmake_minimum_required (VERSION 3.8)
set(CMAKE_CXX_STANDARD 11)
set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -std=c++11")

set(CMAKE_RUNTIME_OUTPUT_DIRECTORY ${CMAKE_BINARY_DIR}/bin)
set(CMAKE_LIBRARY_OUTPUT_DIRECTORY ${CMAKE_BINARY_DIR}/bin)
set(PROJECT_TEST_NAME tests)

set( gtest_force_shared_crt ON CACHE BOOL "Always use msvcrt.dll" FORCE)
include(FetchContent)
FetchContent_Declare(
  googletest
  GIT_REPOSITORY https://github.com/google/googletest.git
  GIT_TAG        release-1.12.1
)
FetchContent_MakeAvailable(googletest)
enable_testing()

add_library(GTest::GTest INTERFACE IMPORTED)
target_link_libraries(GTest::GTest INTERFACE gtest_main)

set(sources
	ApiTest.cpp
	ApiTest.h
	HestonPricerTest.cpp
	HestonPricerTest.h
)

add_executable(${PROJECT_TEST_NAME} ${sources})
target_link_libraries(${PROJECT_TEST_NAME} 
	PUBLIC 
	gtest_main
	ProjectX.AnalyticsLibNativeDebug
)

add_test(
	NAME ApiTest
	COMMAND ${PROJECT_TEST_NAME}
	WORKING_DIRECTORY ${CMAKE_RUNTIME_OUTPUT_DIRECTORY}
)