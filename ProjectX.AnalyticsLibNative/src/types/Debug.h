#pragma once
#include "..\pch.h"
#include <map>

using namespace std;

namespace ProjectXAnalyticsCppLib {
	class Debug
	{
	public:
		Debug()
			: callsCount(0), putsCount(0), totalSimulations(0)
		{
			rhos.clear();
			spotGraph.clear();
		};
		Debug(int calls, int puts, std::vector<double> rhos, std::map<double, std::vector<double>> spots, int simulations)
		{
			this->callsCount = calls;
			this->putsCount = puts;
			this->rhos = rhos;
			this->spotGraph = spots;
			this->totalSimulations = simulations;
		};
	public:
		int callsCount;
		int putsCount;
		std::vector<double> rhos;
		std::map<double, std::vector<double>> spotGraph;
		int totalSimulations;
	};
}