#include "../pch.h"
#include "Debug.h"
#include <sstream>
#include <iostream>
#include <string>
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

Debug ProjectXAnalyticsCppLib::Debug::Build(int calls, int puts, const std::vector<double> rhos, const std::map<double, std::vector<double>> spots, int simulations)
{   
    std::string rhoString = CSV(rhos);
    std::string spotString = CSV(spots);
    
    char r[10000] = {};
    char s[10000] = {};
    size_t MAX = (10000 - 1); // maximum number of chars
    std::copy(rhoString.begin(), (rhoString.size() >= MAX ? rhoString.begin() + MAX : rhoString.end()), r);
    std::copy(spotString.begin(), (spotString.size() >= MAX ? spotString.begin() + MAX : spotString.end()), s);
    
	Debug dbg = Debug(calls, puts, r, s, simulations);
	return dbg;
}