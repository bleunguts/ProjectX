#include "pch.h"
#include "MonteCarloOptionsPricer.h"

// Function to calculate the cumulative distribution function of the standard normal distribution
static double cumulative_normal(double x) 
{
    return 0.5 * (1.0 + std::erf(x / (std::sqrt(2.0))));
}

// Function to generate a random number between 0 and 1
static double random_double() {
    return rand() / (RAND_MAX + 1.0);
}

void ProjectXAnalyticsCppLib::MonteCarloOptionsPricer::Execute(
	double S,
	double K,
	double r,
	double T,
	double sigma,
	int num_simulations
) 
{
    double dt = T / static_cast<double>(num_simulations);
    double sqrt_dt = std::sqrt(dt);

    double sum_payoff = 0.0;
    double sum_delta = 0.0;
    double sum_delta_squared = 0.0;
    double sum_vega = 0.0;

    for (int i = 0; i < num_simulations; i++) {
        double normal_random = 0.0;
        for (int j = 0; j < 12; j++) {
            normal_random += random_double();
        }
        normal_random -= 6.0;

        double ST = S * std::exp((r - 0.5 * sigma * sigma) * T + sigma * normal_random * sqrt_dt);
        double payoff = std::max(0.0, ST - K);
        sum_payoff += payoff;

        // Calculate the option price, delta, gamma, vega, and rho at each simulation step
        double d1 = (std::log(ST / K) + (r + 0.5 * sigma * sigma) * (T - i * dt)) / (sigma * std::sqrt((T - i * dt)));
        double d2 = d1 - sigma * std::sqrt((T - i * dt));

        double N_d1 = cumulative_normal(d1);
        double N_d2 = cumulative_normal(d2);

        double delta_i = N_d1;
        double gamma_i = std::exp(-r * (T - i * dt)) * N_d1 / (ST * sigma * sqrt_dt);
        double vega_i = ST * std::exp(-r * (T - i * dt)) * N_d1 * sqrt_dt;

        sum_delta += delta_i;
        sum_delta_squared += delta_i * delta_i;
        sum_vega += vega_i;
    }

    double option_price = std::exp(-r * T) * (sum_payoff / static_cast<double>(num_simulations));
    double delta = (sum_delta / static_cast<double>(num_simulations));
    double gamma = (sum_delta_squared / static_cast<double>(num_simulations));
    double theta = (-sum_payoff + S * sum_delta) * std::exp(-r * T) / static_cast<double>(num_simulations);
    double vega = (sum_vega / static_cast<double>(num_simulations));
    double rho = (K * T * std::exp(-r * T) * sum_payoff / static_cast<double>(num_simulations));

    m_PV = option_price;
    m_Delta = delta;
    m_Gamma = gamma;
    m_Theta = theta;
    m_Vega = vega;
    m_Rho = rho;
}