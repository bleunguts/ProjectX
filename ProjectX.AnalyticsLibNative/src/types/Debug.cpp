#include "../pch.h"
#include "Debug.h"
#include <sstream>
#include <iostream>
#include <string>
#include <cstring>
using namespace std;
using namespace ProjectXAnalyticsCppLib;

std::string CSV(const vector<double> v)
{
    std::stringstream s;        // creating stringstream object

    for (auto it = v.begin(); it != v.end(); it++) {
        if (it != v.end() - 1)
            s << *it << ",";
        else
            s << *it;
    }
    return s.str();
}

std::string CSV(const map<double, vector<double>> m)
{
    std::stringstream s;        // creating stringstream object

    for (auto it = m.begin(); it != m.end(); it++) {
        double key = it->first;        
        std::string csv = CSV(it->second);                
        s << key << ":(" << csv << ")";
    }
    return s.str();
}

ProjectXAnalyticsCppLib::Debug::Debug(int calls, int puts, char r[10000], char s[10000], int simulations) : callsCount(0), putsCount(0), totalSimulations(0), rhos(), spotGraph()
{       
    strncpy(this->rhos, r, MAX);
    strncpy(this->spotGraph, s, MAX);
};

Debug ProjectXAnalyticsCppLib::Debug::Build(int calls, int puts, const std::vector<double> rhos, const std::map<double, std::vector<double>> spots, int simulations)
{   
    std::string rhoString = CSV(rhos);
    std::string spotString = CSV(spots);
    
    char r[SIZE] = {};
    char s[SIZE] = {};

    std::copy(rhoString.begin(), (rhoString.size() >= MAX ? rhoString.begin() + MAX : rhoString.end()), r);
    std::copy(spotString.begin(), (spotString.size() >= MAX ? spotString.begin() + MAX : spotString.end()), s);
    
	Debug dbg = Debug(calls, puts, r, s, simulations);
	return dbg;
}