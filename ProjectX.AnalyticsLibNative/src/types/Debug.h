#pragma once
#include "..\pch.h"
#include <stdlib.h> 
#include <map>

using namespace std;

namespace ProjectXAnalyticsCppLib 
{
	

	class Debug
	{
	public:
		Debug() : callsCount(0), putsCount(0), totalSimulations(0) {};
		Debug(int calls, int puts, char r[10000], char s[10000], int simulations) : callsCount(0), putsCount(0), totalSimulations(0)
		{			
			size_t MAX = (10000 - 1);

			strncpy_s(this->rhos, _countof(this->rhos), r, MAX);
			strncpy_s(this->spotGraph, _countof(this->spotGraph), s, MAX);
		};
		static Debug Build(int calls, int puts, std::vector<double> rhos, std::map<double, std::vector<double>> spots, int simulations);
	public:
		int callsCount;
		int putsCount;
		char rhos[10000];		
		char spotGraph[10000];		
		int totalSimulations;	
	};
}