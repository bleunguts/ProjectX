# CMake generated Testfile for 
# Source directory: C:/Dev/projects/GitHub/ProjectX/ProjectX.AnalyticsLibNative/test
# Build directory: C:/Dev/projects/GitHub/ProjectX/ProjectX.AnalyticsLibNative/build/test
# 
# This file includes the relevant testing commands required for 
# testing this directory and lists subdirectories to be tested as well.
if(CTEST_CONFIGURATION_TYPE MATCHES "^([Dd][Ee][Bb][Uu][Gg])$")
  add_test(ApiTest "C:/Dev/projects/GitHub/ProjectX/ProjectX.AnalyticsLibNative/build/bin/Debug/tests.exe")
  set_tests_properties(ApiTest PROPERTIES  WORKING_DIRECTORY "C:/Dev/projects/GitHub/ProjectX/ProjectX.AnalyticsLibNative/build/bin" _BACKTRACE_TRIPLES "C:/Dev/projects/GitHub/ProjectX/ProjectX.AnalyticsLibNative/test/CMakeLists.txt;34;add_test;C:/Dev/projects/GitHub/ProjectX/ProjectX.AnalyticsLibNative/test/CMakeLists.txt;0;")
elseif(CTEST_CONFIGURATION_TYPE MATCHES "^([Rr][Ee][Ll][Ee][Aa][Ss][Ee])$")
  add_test(ApiTest "C:/Dev/projects/GitHub/ProjectX/ProjectX.AnalyticsLibNative/build/bin/Release/tests.exe")
  set_tests_properties(ApiTest PROPERTIES  WORKING_DIRECTORY "C:/Dev/projects/GitHub/ProjectX/ProjectX.AnalyticsLibNative/build/bin" _BACKTRACE_TRIPLES "C:/Dev/projects/GitHub/ProjectX/ProjectX.AnalyticsLibNative/test/CMakeLists.txt;34;add_test;C:/Dev/projects/GitHub/ProjectX/ProjectX.AnalyticsLibNative/test/CMakeLists.txt;0;")
elseif(CTEST_CONFIGURATION_TYPE MATCHES "^([Mm][Ii][Nn][Ss][Ii][Zz][Ee][Rr][Ee][Ll])$")
  add_test(ApiTest "C:/Dev/projects/GitHub/ProjectX/ProjectX.AnalyticsLibNative/build/bin/MinSizeRel/tests.exe")
  set_tests_properties(ApiTest PROPERTIES  WORKING_DIRECTORY "C:/Dev/projects/GitHub/ProjectX/ProjectX.AnalyticsLibNative/build/bin" _BACKTRACE_TRIPLES "C:/Dev/projects/GitHub/ProjectX/ProjectX.AnalyticsLibNative/test/CMakeLists.txt;34;add_test;C:/Dev/projects/GitHub/ProjectX/ProjectX.AnalyticsLibNative/test/CMakeLists.txt;0;")
elseif(CTEST_CONFIGURATION_TYPE MATCHES "^([Rr][Ee][Ll][Ww][Ii][Tt][Hh][Dd][Ee][Bb][Ii][Nn][Ff][Oo])$")
  add_test(ApiTest "C:/Dev/projects/GitHub/ProjectX/ProjectX.AnalyticsLibNative/build/bin/RelWithDebInfo/tests.exe")
  set_tests_properties(ApiTest PROPERTIES  WORKING_DIRECTORY "C:/Dev/projects/GitHub/ProjectX/ProjectX.AnalyticsLibNative/build/bin" _BACKTRACE_TRIPLES "C:/Dev/projects/GitHub/ProjectX/ProjectX.AnalyticsLibNative/test/CMakeLists.txt;34;add_test;C:/Dev/projects/GitHub/ProjectX/ProjectX.AnalyticsLibNative/test/CMakeLists.txt;0;")
else()
  add_test(ApiTest NOT_AVAILABLE)
endif()
subdirs("../_deps/googletest-build")
