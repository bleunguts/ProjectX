// pch.h: This is a precompiled header file.
// Files listed below are compiled only once, improving build performance for future builds.
// This also affects IntelliSense performance, including code completion and many code browsing features.
// However, files listed here are ALL re-compiled if any one of them is updated between builds.
// Do not add files here that you will be updating frequently as this negates the performance advantage.

#ifndef PCH_H
#define PCH_H

// add headers that you want to pre-compile here
#include "Utility.h"
#include "types/Debug.h"
#include "types/GreekResults.h"
#include "types/OptionType.h"
#include "types/VanillaOptionsParameters.h"
#include "types/HestonStochasticVolatilityParameters.h"

#endif //PCH_H

#ifdef BUILD_DLL
#define PROJECT_API __declspec(dllexport)
#else
#define PROJECT_API __declspec(dllimport)
#endif
