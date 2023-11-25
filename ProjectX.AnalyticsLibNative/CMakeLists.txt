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

add_subdirectory(src)
add_subdirectory(test)