#pragma once
#include "..\pch.h"

namespace ProjectXAnalyticsCppLib {
	class GreekResults
	{
	public:
		GreekResults()
			: PV(0), PVPut(0), Delta(0), DeltaPut(0), Gamma(0), Vega(0), Rho(0), RhoPut(0), Theta(0), ThetaPut(0)
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
}