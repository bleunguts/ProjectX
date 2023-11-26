#include "../pch.h"
#include "Debug.h"
#include <sstream>
#include <iostream>
#include <string>
using namespace std;
using namespace ProjectXAnalyticsCppLib;

std::string CSV(vector<double> v)
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

std::string CSV(map<double, vector<double>> m)
{
    std::stringstream s;        // creating stringstream object

    for (auto it = m.begin(); it != m.end(); it++) {
        double key = it->first;        
        std::string csv = CSV(it->second);                
        s << key << ":(" << csv << ")";
    }
    return s.str();
}

Debug& ProjectXAnalyticsCppLib::Debug::Build(int calls, int puts, std::vector<double> rhos, std::map<double, std::vector<double>> spots, int simulations)
{   
    std::string rhoString = CSV(rhos);
    std::string spotString = CSV(spots);
    char r[256] = "";
    char s[256] = "";    
    strcpy_s(r, rhoString.c_str());
    strcpy_s(s, spotString.c_str());
	Debug dbg = Debug(calls, puts, r, s, simulations);
	return dbg;
}