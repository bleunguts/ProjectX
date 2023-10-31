ProjectX🌊 is a technical analysis centric app that can backtest popular trading strategies such as the Long-Short trading strategy designed for the personal trader. 
I wrote ProjectX to collate my projects that I have built in github over the years option risk pricing C++, back testing strategies into a concise platform deliverable that showcases a decade plus of industry expertise in risk management and trading & execution systems.

We explore long-short strategy by using trend based mean reversion stock indicators; in this case the moving average stock indicator; a lagging indicator that can identify trends and reversals. 
Finding similar well-known trading strategies from public sources is easily accessible, the key for a profitable strategy is the optimization methods that have been carried out and this app provides the toolkit to support this endeavour.

The core compute engine is running on cloud-compatabile ASP.NETCore with a Desktop UI (WPF) and a Web Portal front end (React).

Successful trading strategy development requires backtesting on historical real price data, 
ProjectX provides the following features to assist the optimization process:
* ability to design trading strategies for demo trade purposes and backtest said strategies by running a simulation on financial stocks like GOOGL, AAPL.
* captures live market data from reliable third party data sources such as FMP, Quandl

_Desktop App Backtesting:_
<img src="./Backtesting.jpg" width="800">

The supporting components relates to providing pricers to help explore arbitrage or advantagous opportunities, fine-tune pricing models, and understand market sentiment.
ProjectX provies the following highly customizable and optimizable toolkit
* ProjectX implements pricing models for FX products based on spreaded commisions, Options Pricing, Bonds Pricing, CDS Pricing
* Vanilla Options Black Scholes Pricer (C#)
* Implied Vol Pricer to help understand market sentiment
* Simple Bonds Pricer
* CDS Pricer
* Vanilla Options BS Pricer C++ better RNGs and normal distribution for stochastic component  (future support based on https://github.com/bleunguts/OptionsPricerCpp) 
* QL.NET product pricer (future support experiment with popular opensource pricing library)
* FX Pricing & Order management System

_Desktop App BS Option Pricing:_
<img src="./Options.jpg" width="50">

_Desktop App FX OMS:_
<img src="./FXOMS.jpg" width="50">

------------------------------------------------------------------------------------------------------------------------------------------------------------------------
_Tech Stack:_
* C# 11, .NET 7, WPF, Caliburn.Micro, System.ComponentModel.Composition.Hosting.CompositionContainer (IoC), ReactiveX 6, Microsft Chart Controls,
* ASPNET Core WebApi 6, SignalR, .NET Json Serialization, Background Services (Azure Ready), LiveCharts Skia
* React 8, Bootstrap,  Typescript, recharts,
* StockIndicators Skender API,  NinjaTrader StockInidicator API, FinancialModellingPrep MarketData API, Quandl MarketData API
* Trend indicator methods: Bollinger Bands, Moving Averages
 