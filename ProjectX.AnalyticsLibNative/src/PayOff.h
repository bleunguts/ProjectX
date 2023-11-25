#pragma once

namespace ProjectXAnalyticsCppLib
{
    class PayOff
	{
	public:
		PayOff() {};

		virtual double operator()(double Spot) const = 0;
		virtual ~PayOff() {}
		virtual PayOff* clone() const = 0;
	private:

	};

	class PayOffCall : public PayOff
	{
	public:
		PayOffCall(double Strike_);
		virtual double operator()(double Spot) const;
		virtual PayOff* clone() const;
		virtual ~PayOffCall() {}
	private:
		double Strike;
	};

	class PayOffPut : public PayOff
	{
	public:
		PayOffPut(double Strike_);
		virtual double operator()(double Spot) const;
		virtual PayOff* clone() const;
		virtual ~PayOffPut() {}
	private:
		double Strike;

	};

	class PayOffBridge
	{
	public:
		PayOffBridge(const PayOffBridge& original);
		PayOffBridge(const PayOff& innerPayOff);
		inline double operator()(double Spot) const {
			return ThePayOffPtr->operator()(Spot);
		}
		~PayOffBridge();
		PayOffBridge& operator=(const PayOffBridge& original);

	private:
		PayOff* ThePayOffPtr;
	};
}