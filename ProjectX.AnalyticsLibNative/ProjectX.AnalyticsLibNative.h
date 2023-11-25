// ProjectX.AnalyticsLibNative.h : Include file for standard system include files,
// or project specific include files.

#pragma once

#include <iostream>

#ifdef BUILD_DLL
#define PROJECT_API __declspec(dllexport)
#else
#define PROJECT_API __declspec(dllimport)
#endif

class PROJECT_API API
{
public:
    API() {};
    virtual ~API() {};
    void execute(void);
};
