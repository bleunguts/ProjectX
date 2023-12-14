import Typography from '@mui/material/Typography';
import * as React from 'react';
import { cannedGainers, cannedMostActive } from './StockNewsData';
import Grid from '@mui/material/Grid';
import Button from '@mui/material/Button';
import { SyntheticEvent, useEffect, useState } from 'react';
import IconButton from '@mui/material/IconButton';
import SyncIcon from '@mui/icons-material/Sync';
import { AxiosResponse } from 'axios';
import api from './api';

export interface StockMarketSymbol {
    ticker: string,
    changes: number,
    price: string,
    changesPercentage: string,
    companyName: string
};

export default function StockNews() {
    const [stockSymbol, setStockSymbol] = useState<string>("AAPL")
    const [highestGainerStocks, setHighestGainerStocks] = useState<StockMarketSymbol[]>(cannedGainers);
    const [mostActiveStocks, setMostActiveStocks] = useState<StockMarketSymbol[]>(cannedMostActive);
    
    useEffect(() => {
        getHighestGainerStocks(cannedGainers);
        getMostActiveStocks(cannedMostActive);
    }, []);

    const getHighestGainerStocks = (defaultsOnFail : any[]) => {
        api        
            .fetchHighestGainerStocks(6)
            .then((res) => setHighestGainerStocks((res as AxiosResponse<any, any>).data))
            .catch((error) => {    
                console.log(`Error occured in fetching data from backend: ${error}`);
                setHighestGainerStocks(defaultsOnFail);
            });
    };

    const getMostActiveStocks = (defaultsOnFail : any[]) => {
        api        
            .fetchMostActiveStocks(6)
            .then((res) => setMostActiveStocks((res as AxiosResponse<any, any>).data))
            .catch((error) => {   
                console.log(`Error occured in fetching data from backend: ${error}`);
                setMostActiveStocks(defaultsOnFail);
            });
    };

    const handleStockSymbolChange = (event) => {
        setStockSymbol(event.target.value);
    };
 
    const handleHighestGainersClick = (event: SyntheticEvent) => {
        console.log(`Highest Gainers refresh clicked.`);       
        getHighestGainerStocks([]);
    };
    const handleMostActiveClick = (event: SyntheticEvent) => {
        console.log(`Most Active refresh clicked.`);       
        getMostActiveStocks([]);
    };
    const handleStrategizeFrom = (symbol: string) => {
        setStockSymbol(symbol);   
        handleStrategize();
    };
    const handleStrategize = () => {
        console.log(`Strategizing target: ${stockSymbol} ...`);    
        // TODO: invoke strategize     
    };

    function prettify(changesPercentage: string): string {
        return changesPercentage.includes('.') ?
                changesPercentage.substring(0, changesPercentage.indexOf('.') + 2)
                :
                changesPercentage;
    }

    return (
        <>
            <Grid container direction="row" spacing={1.5} alignItems="right" justifyContent="flex-end" alignContent='center'>
                <Grid item><Typography variant="h6" align="center" gutterBottom>Stock Symbol:</Typography></Grid>
                <Grid item><input id="stockSymbol" type="text" size={5} disabled={true} value={stockSymbol} onChange={handleStockSymbolChange}/></Grid>
                <Grid item><Button disabled={true} onClick={handleStrategize}>STRATEGIZE</Button></Grid>
            </Grid>
            <Typography variant="h6" align="center" gutterBottom>
            Stock Market Highest Gainers <IconButton onClick={handleHighestGainersClick}><SyncIcon></SyncIcon></IconButton>
            </Typography>
            {highestGainerStocks.map((gainer) =>
            (
                <Grid key={JSON.stringify(gainer)} container direction='row' spacing={0.5} alignItems="center" justifyContent="center" alignContent='center'>
                    <Grid item xs={2}><Typography><b>{gainer.ticker}</b></Typography></Grid>
                    <Grid item xs={2}><Typography>${gainer.price}</Typography></Grid>
                    <Grid item xs={2}><Typography>{prettify(gainer.changesPercentage)}%</Typography></Grid> 
                    <Grid item xs={1}><Button disabled={true} onClick={() => handleStrategizeFrom(gainer.ticker)}>STRATEGIZE</Button></Grid>                                       
                </Grid>
            ))}
            <Typography variant="h6" align="center" gutterBottom>
                Stock Market Most Active Players <IconButton onClick={handleMostActiveClick}><SyncIcon></SyncIcon></IconButton>
            </Typography>
            {mostActiveStocks.map((stock) =>
            (
                <Grid key={JSON.stringify(stock)} container direction='row' spacing={0.5} alignItems="center" justifyContent="center" alignContent='center'>
                    <Grid item xs={2}><Typography><b>{stock.ticker}</b></Typography></Grid>
                    <Grid item xs={2}><Typography>${stock.price}</Typography></Grid>
                    <Grid item xs={2}><Typography>{prettify(stock.changesPercentage)}%</Typography></Grid>
                    <Grid item xs={1}><Button disabled={true} onClick={() => handleStrategizeFrom(stock.ticker)}>STRATEGIZE</Button></Grid>   
                </Grid>
            ))}
        </>
    );
}