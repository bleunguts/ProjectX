ProjectX🌊 App that does technical analysis to shoot two birds with one stone for personal investment management

**Market Signals Analytics (MSA)**
* MSA monitors market prices, capture pricing signals,
* MSA provides ability to design trading strategies for demo trade purposes
* MSA provides backtesting of said trading strategies by running a simulation against the strategy from sourced historical data on fincnacial products like GOOGL, AAPL

**Quant Model Pricing (QMP)** 		
* QMP implements pricing models for FX products (commisions), Options Pricing, Bonds Pricing, CDS Pricing
* Options includes Vanilla Options Black Scholes Pricer (C# & CPP), Implied Vol Pricer, Barrier Options
* Fixed Income includes Simple Bonds Pricer, CDS Pricer    
------------------------------------------------------------------------------------------------------------------------------------------------------------------------

_Roadmap_
* React web front end capturing Market Signals Analytics which shows back testing results
* A way to rapid develop trading strategies and using back testing to validate it
* Show live FX prices from FX market data source as an additional column 'Reference Price'
* All pricing model calcs are Azure-enabled (using AspNetCore BackgroundServices)
* Enhanced eTrader trade management screen shows PnL helps facilitate demo trading

_Tech Stack:_
* C# 11, .NET 7, WPF, Caliburn.Micro, System.ComponentModel.Composition.Hosting.CompositionContainer (IoC), ReactiveX 6,
* ASPNET Core WebApi 6, SignalR, .NET Json Serialization, Background Services (Azure Ready)
* React 8, Bootstrap
  
_Market data sources:_
* Financial Modelling Prep (https://site.financialmodelingprep.com/developer/docs/)
* Quandl download GOOGL stock data it seems to have data from 2018 to 2014 (http://www.quandl.com/api/v3/datasets/WIKI/GOOGL.csv?auth_token=gp_z7rn26KEP3uJFuuiw&collapse=daily&transformation=none&sort_order=asc&exclude_headers=False)
* Rapid API (https://rapidapi.com/blog/best-stock-api/)
* Free Finance Data Api
* Twelve Data

