#pragma once
#include "..\pch.h"
#include <map>

using namespace std;

namespace ProjectXAnalyticsCppLib 
{
	class Debug
	{
	public:
		Debug(): callsCount(0), putsCount(0), totalSimulations(0) {};
		Debug(int calls, int puts, char rhos[256], char spots[256], int simulations) : callsCount(0), putsCount(0), totalSimulations(0)
		{
			strcpy(this->rhos, rhos);
			strcpy(this->spotGraph, spots);
		};
		static Debug& Build(int calls, int puts, std::vector<double> rhos, std::map<double, std::vector<double>> spots, int simulations);
	public:
		int callsCount;
		int putsCount;
		char rhos[256];
		char spotGraph[256];
		int totalSimulations;	
	};
}