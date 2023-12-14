import * as React from 'react';
import api from './api';
import FxRate, { CurrencyPair, CurrencyPairFormatted } from './FxRate';
import { useState } from 'react';

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
    const [eurUsd, setEurUsd] = useState<CurrencyPairFormatted>(To({ bid: 1.0512, ask: 1.0509 }, 4));
    const [gbpUsd, setGbpUsd] = useState<CurrencyPairFormatted>(To({ bid: 1.2550, ask: 1.2109 }, 4));
    const [usdJpy, setUsdJpy] = useState<CurrencyPairFormatted>(To({ bid: 149.94, ask: 149.95 }, 2));
    const [usdChf, setUsdChf] = useState<CurrencyPairFormatted>(To({ bid: 0.8952, ask: 0.8955 }, 4));
    const [audusd, setAudUsd] = useState<CurrencyPairFormatted>(To({ bid: 0.6330, ask: 1.0509 }, 4));
    const [usdcad, setUsdCad] = useState<CurrencyPairFormatted>(To({ bid: 1.3776, ask: 1.3779 }, 4));
    const [nzdusd, setNzdUsd] = useState<CurrencyPairFormatted>(To({ bid: 0.5821, ask: 0.5822 }, 4));
    const [btcusd, setBtcUsd] = useState<CurrencyPairFormatted>(To({ bid: 34223, ask: 34222 }, 0));
    
    api.connection().on("PushFxRate", (spotPriceResult) => {
        const timestamp = spotPriceResult.timestamp;
        const spotPriceResponse = spotPriceResult["spotPriceResponse"];
        const ccyName = spotPriceResponse.spotPrice.currencyPair
        const bidPrice = spotPriceResponse.spotPrice.bidPrice;
        const askPrice = spotPriceResponse.spotPrice.askPrice;
        let c: CurrencyPair = { "bid": bidPrice, "ask": askPrice };
        switch (ccyName) {
            case 'EURUSD':                
                setEurUsd(To(c, 4));
                break;
            case 'GBPUSD':
                setGbpUsd(To(c, 4));
                break;            
            case 'USDJPY':
                setUsdJpy(To(c, 2));
                break;
            case 'USDCHF':
                setUsdChf(To(c, 4));
                break;
            case 'AUDUSD':
                setAudUsd(To(c, 4));
                break;
            case 'USDCAD':
                setUsdCad(To(c, 4));
                break;
            case 'NZDUSD':
                setNzdUsd(To(c, 4));
                break;
            case 'BTCUSD':
                setBtcUsd(To(c, 0));
                break;
        }
        console.log(`FXRate recvd ${ccyName} @ ${timestamp}.`);        
    }); 
    api.connection().on("StopFxRate", (ccyName) => {        
        console.log(ccyName);
        console.log(`Stop FxRate recvd for ccy name ${ccyName}`);
    });   

    return (
        <>
            <FxRate ccyName='EURUSD' ccyPair={eurUsd} />            
            <FxRate ccyName='GBPUSD' ccyPair={gbpUsd} />            
            <FxRate ccyName='USDJPY' ccyPair={usdJpy} />            
            <FxRate ccyName='USDCHF' ccyPair={usdChf} />
            <FxRate ccyName='AUDUSD' ccyPair={audusd} />            
            <FxRate ccyName='USDCAD' ccyPair={usdcad} />
            <FxRate ccyName='NZDUSD' ccyPair={nzdusd} />
            <FxRate ccyName='BTCUSD' ccyPair={btcusd} />                        
        </>
    );
}

function To(currencyPair: CurrencyPair, dp: number): CurrencyPairFormatted {
    return {
        bid: currencyPair.bid.toFixed(dp),
        ask: currencyPair.ask.toFixed(dp),
    };
}
