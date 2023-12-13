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

export default function StockNews() {
    const [stockSymbol, setStockSymbol] = useState<string>("AAPL")
    const [highestGainerStocks, setHighestGainerStocks] = useState<any[]>(cannedGainers);
    const [mostActiveStocks, setMostActiveStocks] = useState<any[]>(cannedMostActive);
    
    useEffect(() => {
        getHighestGainerStocks(cannedGainers);
        getMostActiveStocks(cannedMostActive);
    }, []);

    const getHighestGainerStocks = (defaultsOnFail : any[]) => {
        api        
            .fetchHighestGainerStocks(5)
            .then((res) => setHighestGainerStocks((res as AxiosResponse<any, any>).data))
            .catch((error) => {    
                console.log(`Error occured in fetching data from backend: ${error}`);
                setHighestGainerStocks(defaultsOnFail);
            });
    };

    const getMostActiveStocks = (defaultsOnFail : any[]) => {
        api        
            .fetchMostActiveStocks(5)
            .then((res) => setMostActiveStocks((res as AxiosResponse<any, any>).data))
            .catch((error) => {   
                console.log(`Error occured in fetching data from backend: ${error}`);
                setMostActiveStocks(defaultsOnFail);
            });
    };
 
    const handleHighestGainersClick = (event: SyntheticEvent) => {
        console.log(`Highest Gainers refresh clicked.`);       
        getHighestGainerStocks([]);
    };
    const handleMostActiveClick = (event: SyntheticEvent) => {
        const target = event.target;
        console.log(`Most Active refresh clicked.`);       
        getMostActiveStocks([]);
    };
    const handleApplyClick = (symbol: string) => {
        console.log(`Applied clicked on target: ${symbol}`);    
        // do something
        
    };
    return (
        <>
            <Typography variant="h6" align="center" gutterBottom>
                Stock Symbol Applied: <b>{stockSymbol}</b>
            </Typography>
            <Typography variant="h6" align="center" gutterBottom>
            Stock Market Highest Gainers <IconButton onClick={handleHighestGainersClick}><SyncIcon></SyncIcon></IconButton>
            </Typography>
            {highestGainerStocks.map((gainer) =>
            (
                <Grid container direction='row' spacing={0.5} alignItems="center" justifyContent="center" alignContent='center'>
                    <Grid item xs={2}><Typography><b>{gainer.symbol}</b></Typography></Grid>
                    <Grid item xs={2}><Typography>${gainer.price.toFixed(1)}</Typography></Grid>
                    <Grid item xs={2}><Typography>{gainer.changesPercentage.toFixed(1)}%</Typography></Grid> 
                    <Grid item xs={1}><Button disabled={true} onClick={() => handleApplyClick(gainer.symbol)}>APPLY STRATEGY</Button></Grid>                                       
                </Grid>
            ))}
            <Typography variant="h6" align="center" gutterBottom>
                Stock Market Most Active Players <IconButton onClick={handleMostActiveClick}><SyncIcon></SyncIcon></IconButton>
            </Typography>
            {mostActiveStocks.map((stock) =>
            (
                <Grid container direction='row' spacing={0.5} alignItems="center" justifyContent="center" alignContent='center'>
                    <Grid item xs={2}><Typography><b>{stock.symbol}</b></Typography></Grid>
                    <Grid item xs={2}><Typography>${stock.price.toFixed(1)}</Typography></Grid>
                    <Grid item xs={2}><Typography>{stock.changesPercentage.toFixed(1)}%</Typography></Grid>
                    <Grid item xs={1}><Button disabled={true} onClick={() => handleApplyClick(stock.symbol)}>APPLY STRATEGY</Button></Grid>   
                </Grid>
            ))}
        </>
    );
}