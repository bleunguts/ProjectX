Successful Trading System involves trading strategy development, backtesting, and risk management.
Finding similar well-known trading strategies from public sources is easily accessible, the key for a profitable strategy is the optimization methods that have been carried out and this app provides the toolkit to support this endeavour.

There are two broad categories of trading strategies, mean-Reversion and momentum.

Mean reversion exploits the fact that stock prices eventually return to long-term avverage price, we buy low and sell high (long-short strategy).  
Let's take IBM 2012-2013 stock prices as an example,
Stock prices moved between 180 to 215.
Profitable strategies include buying when prices move below 40-day moving average, and selling when prices move above 40-day moving average.

Momentum exploits the fact that strong moves in the market (either direction) will follow higher highs and lower lows, buy stocks that are showing upward trend, and selling stock on a downward trend... most common
Lets take GS 2012-2015 stock prices as an example.  
Profitable strategies include buying when price goes above 300-day moving average and selling once price is below 300-day moving average

Rule of thumb for trend-folowing strategies is to buy on the upwswing and sell on the downswing 
Momentum exploits the fact that accelaration of prices are due to earnings, market sentiment, news, greed or fear.  
Momentum traders ignore predictions and believe any changes in fundaments (earnings, sales) will evenutally reflect on the price.

Small moving window size and small SignalIn and SignalOut is tighter, leading to shorter holding period and more trade executions.
SignalOut = SignalOut times standard deviations within a moving window to exit the position.
SignalOut = 0 means wait for price return back to mean before exiting.

Bollinger Band graph (MovingAverage), shows four lines of the graph (Original,Predicted,Upper,Lower) where PredictedPrice is just the moving average.  
Z-Score is easier to interpret, it is just the signal graphed, it is calculated by how close (sd) you are to the mean.
Graph modelling using moving average (z-score), linear regression.

Other tchnical indicators technical analysis include: 
* Linear Regression where we project linearly a forecast of price movements based on moving window
* Relative Strength Index (momentum indicator) oscilattes between 0 and 100, mocking volatility, > 70 is overbought and < 30 is oversold
* Williams %R (momentum indicator) oscillates between -100 to 0, mocking volatility, > -20 is overbought and < 80 is oversold
* machine learning and neural networks 

Here are some of the most popular trend indicators used by traders today: Moving Average Stock Indicators, Parabolic Stop and Reverse (SAR), Moving Average Convergence Divergence (MACD), Stochastic Oscilalator, Commodity Channel Index (CCI), Relative Strength Index (RS)
https://tinyurl.com/4uayf4xd

Backtesting allows understanding the strategy completelty and can help prove that you can reproduce its results by seeing how it would have performend in the past.
Backtesting is based on long-short method to compute PnL.
Backtesting uses Moving Average for a specified Moving Window (=number of prices/daily, e.g 3,14)
Backtesting will only enter a new trade AFTER exiting the current position, enforces only one trade at most per tading day (tradeType, isTrade is used to implement this).

Backtesting for prices between Start End.
Backtesting computes Pnl with inputs Signal In, Signal Out based on a strategy
Backtesting methods include Mean Reversion, Momentum
Backtesting includes hold stocks strategy to compare as a baseline (PnlCumHold), simply buying and holding the stock position for tbe duration. 
Backtesting features Re-Invest, whether use fixed initial notional or reinvest earnings from trades back into the strategy.

Risk measure indicators, Sharpe Ratio and Maximum drawdown.  
Sharpe = Excess of daily return  / stddev returns 
i.e. winnings above the risk-free rate.
Annualized Sharpe Ratio (sp0) comparing pnl from the strategy and buying-and-holding (sp1), this allows you to see how strategy has performed every year.

Maximum drawdown = A percentage of largest peak to trough drop, this allows you to see future drawdown performance of the strategy.
Prices of drawdown contracts would be significant when market is in a bubble, and prices of drawdown is cheap in a stable mean-reverting market.

Optimization performs sensitivity study by varying SignalIn, SignalOut and resulting PnL/Sharpe Ratio in a most profitable ranking table.
We are identifying the optimal buy/sell parameters, you should always perform senstivity study on your trading strategies.

Sharpe Ratio guide:
Usually, any Sharpe ratio greater than 1.0 is considered acceptable to good by investors.
A ratio higher than 2.0 is rated as very good.
A ratio of 3.0 or higher is considered excellent.
A ratio under 1.0 is considered sub-optimal.

Implemented Hurst to identify regime (mean reverting, or momentum) to decide which strategy because applying mean reversion to IBM yields positive results, yet momentum yield negative pnl.
Alternatively, (Markov-transition modelling)

Coming Soon: 
* Stop loss control and transaction costs PnL calculations.
* Pairs uses Pair Type by Price Ratio correlation or via Spread, its a market neutral mean-reverting strategy uncorrelated to market direction.  
	* A steady strategy to hedge sector and market risk
	* Involves going long on the under-performer while going short on the over performer when spread is away from mean, when spreadd reverts to mea we close the position
	* Mean reversion we are betting that prices will eventually revert to their historical trends
	* Self-funding strategy, since short sale proceeds are reinvested to tcreate long position.
	e.g. QQQ and SOY, Coke and Peps
