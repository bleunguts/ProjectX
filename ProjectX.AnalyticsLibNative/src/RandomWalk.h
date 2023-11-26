#pragma once

enum class RandomAlgorithm 
{
	Summation,
	BoxMuller
};

class RandomWalk {
public:
	RandomWalk() : m_Algorithm(RandomAlgorithm::BoxMuller) {}
	RandomWalk(RandomAlgorithm algo) : m_Algorithm(algo) {}
	double GetOneGaussian();
private:
	double GetOneGaussianBySummation();
	double GetOneGaussianByBoxMuller();
private:
	RandomAlgorithm m_Algorithm;
};