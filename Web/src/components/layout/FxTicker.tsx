import * as React from 'react';
import api from './api';
import FxRate, { CurrencyPair } from './FxRate';

export default function FxTicker() {    
    const ccyPairs: Map<string, CurrencyPair> = new Map<string, CurrencyPair>([
        ["EURUSD", { bid: 1.0512, ask: 1.0509 }],
        ["GBPUSD", { bid: 1.2550, ask: 1.2109 }],
        ["USDJPY", { bid: 149.94, ask: 149.95 }],
        ["USDCHF", { bid: 0.8952, ask: 0.8955 }],
        ["AUDUSD", { bid: 0.6330, ask: 1.0509 }],
        ["USDCAD", { bid: 1.3776, ask: 1.3779 }],
        ["NZDUSD", { bid: 0.5821, ask: 0.5822 }],
        ["BTCUSD", { bid: 34223, ask: 34222 }],
    ]);             

    //api.connection().on("FXRate", () => {

    //});        
    return (
        <>     
            <FxRate ccyName='EURUSD' ccyPair={ccyPairs.get('EURUSD') as CurrencyPair} />            
            <FxRate ccyName='GBPUSD' ccyPair={ccyPairs.get('GBPUSD') as CurrencyPair} />            
            <FxRate ccyName='USDJPY' ccyPair={ccyPairs.get('USDJPY') as CurrencyPair} />            
            <FxRate ccyName='USDCHF' ccyPair={ccyPairs.get('USDCHF') as CurrencyPair} />            
            <FxRate ccyName='AUDUSD' ccyPair={ccyPairs.get('AUDUSD') as CurrencyPair} />            
            <FxRate ccyName='USDCAD' ccyPair={ccyPairs.get('USDCAD') as CurrencyPair} />            
            <FxRate ccyName='NZDUSD' ccyPair={ccyPairs.get('NZDUSD') as CurrencyPair} />            
            <FxRate ccyName='BTCUSD' ccyPair={ccyPairs.get('BTCUSD') as CurrencyPair} />                        
        </>
    );
}