#pragma once
#include "..\pch.h"
#include <stdlib.h> 
#include <map>

using namespace std;

namespace ProjectXAnalyticsCppLib 
{
	class Debug
	{
	private:
		const static unsigned int SIZE = 10000;
		const static size_t MAX = (SIZE - 1); // maximum number of chars
	public:
		Debug() : callsCount(0), putsCount(0), totalSimulations(0), rhos(), spotGraph() {};
		Debug(int calls, int puts, char r[SIZE], char s[SIZE], int simulations);
		static Debug Build(int calls, int puts, std::vector<double> rhos, std::map<double, std::vector<double>> spots, int simulations);
	public:
		int callsCount;
		int putsCount;
		char rhos[SIZE];
		char spotGraph[SIZE];
		int totalSimulations;		
	};
}