#pragma once

enum class RandomAlgorithm 
{
	Summation,
	BoxMuller
};

class RandomWalk {
private: 
	RandomAlgorithm m_Algorithm;
public:
	RandomWalk(RandomAlgorithm algo)
	{
		m_Algorithm = algo;
	};	
	double GetOneGaussian();
private:
	double GetOneGaussianBySummation();
	double GetOneGaussianByBoxMuller();
};