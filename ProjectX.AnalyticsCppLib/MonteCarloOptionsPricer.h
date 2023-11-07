#pragma once
using namespace System;

namespace ProjectXAnalyticsCppLib 
{
	public ref class MonteCarloOptionsPricer 
	{
	public:
		void Execute(
			double S, 
			double K, 
			double r, 
			double T, 
			double sigma, 
			int num_simulations
		);
		inline double PV() { return m_PV; }
		inline double Delta() { return m_Delta; }
		inline double Gamma() { return m_Gamma; }
		inline double Theta() { return m_Theta; }
		inline double Rho() { return m_Rho; }		
		inline double Vega() { return m_Vega; }		
	private:
		double m_PV;
		double m_Delta;
		double m_Gamma;
		double m_Theta;
		double m_Rho;
		double m_Vega;
	};
}