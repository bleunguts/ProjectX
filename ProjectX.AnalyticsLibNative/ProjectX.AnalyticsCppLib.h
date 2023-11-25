#pragma once
#include <map>
using namespace std;

namespace ProjectXAnalyticsCppLib {			
	class Debug 
	{
	public:
		Debug() {};
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

	class GreekResults
	{
	public:
		GreekResults()
		{
		};
		GreekResults(double pv, double pvPut, Debug debug)
		{
			this->PV = pv;
			this->PVPut = pvPut;
			this->Debug = debug;
		};
		GreekResults(double pv, double pvPut, double delta, double deltaPut, double gamma, double vega, double rho, double rhoPut, double theta, double thetaPut)
		{
			this->PV = pv;
			this->PVPut = pvPut;
			this->Delta = delta;
			this->DeltaPut = deltaPut;
			this->Gamma = gamma;
			this->Vega = vega;
			this->Rho = rho;
			this->RhoPut = rhoPut;
			this->Theta = theta;
			this->ThetaPut = thetaPut;
		};
	public:
		double PV;
	 	double PVPut;
		double Delta;
		double DeltaPut;
		double Gamma;
		double Vega;
		double Rho;
		double RhoPut;
		double Theta;
		double ThetaPut;
		Debug Debug;		
	};

	enum OptionType
	{
		Call, Put
	};

	class VanillaOptionParameters 
	{
	private:
		OptionType m_optionType;
		double m_strike;
		double m_expiry;		

	public:
		VanillaOptionParameters(OptionType optionType, double strike, double expiry)
		{
			m_optionType = optionType;
			m_strike = strike;
			m_expiry = expiry;			
		};
		inline OptionType OptionType() { return m_optionType; };
		inline double Strike() { return m_strike; };
		inline double Expiry() { return m_expiry; };		
	};

	class IMonteCarloCppPricer
	{
		virtual GreekResults MCValue(
			VanillaOptionParameters& TheOption,
			double Spot,
			double Vol,
			double r,
			uint64_t NumberOfPaths) = 0;
		virtual double ImpliedVolatilityMC(
			VanillaOptionParameters& TheOption,
			double Spot,
			double r,
			uint64_t NumberOfPaths,
			double optionPrice
		) = 0;
	};
}
