import Typography from '@mui/material/Typography';
import * as React from 'react';
import { gainers, mostActive } from './StockNewsData';
import Grid from '@mui/material/Grid';
import Button from '@mui/material/Button';
import { SyntheticEvent, useState } from 'react';
import IconButton from '@mui/material/IconButton';
import SyncIcon from '@mui/icons-material/Sync';

export default function StockNews() {
    const [stockSymbol, setStockSymbol] = useState<string>("AAPL")
    const handleHighestGainersClick = (event: SyntheticEvent) => {
        const target = event.target;
        console.log(`Highest Gainers refresh clicked.`);       
        // do something
    };
    const handleMostActiveClick = (event: SyntheticEvent) => {
        const target = event.target;
        console.log(`Most Active refresh clicked.`);       
        // do something
    };
    const handleApplyClick = (symbol: string) => {
        console.log(`Applied clicked on target: ${symbol}`);    
        // do something
        
    };
    return (
        <>
            <Typography variant="h6" align="center" gutterBottom>
                Stock Symbol Used: <b>{stockSymbol}</b>
            </Typography>
            <Typography variant="h6" align="center" gutterBottom>
            Stock Market Highest Gainers <IconButton onClick={handleHighestGainersClick}><SyncIcon></SyncIcon></IconButton>
            </Typography>
            {gainers.slice(0, 5).map((gainer) =>
            (
                <Grid container direction='row' spacing={0.5} alignItems="center" justifyContent="center" alignContent='center'>
                    <Grid item xs={2}><Typography><b>{gainer.symbol}</b></Typography></Grid>
                    <Grid item xs={2}><Typography>${gainer.price.toFixed(1)}</Typography></Grid>
                    <Grid item xs={2}><Typography>{gainer.changesPercentage.toFixed(1)}%</Typography></Grid> 
                    <Grid item xs={1}><Button onClick={() => handleApplyClick(gainer.symbol)}>APPLY STRATEGY</Button></Grid>                                       
                </Grid>
            ))}
            <Typography variant="h6" align="center" gutterBottom>
                Stock Market Most Active Symbols <IconButton onClick={handleMostActiveClick}><SyncIcon></SyncIcon></IconButton>
            </Typography>
            {mostActive.slice(0, 5).map((stock) =>
            (
                <Grid container direction='row' spacing={0.5} alignItems="center" justifyContent="center" alignContent='center'>
                    <Grid item xs={2}><Typography><b>{stock.symbol}</b></Typography></Grid>
                    <Grid item xs={2}><Typography>${stock.price.toFixed(1)}</Typography></Grid>
                    <Grid item xs={2}><Typography>{stock.changesPercentage.toFixed(1)}%</Typography></Grid>
                    <Grid item xs={1}><Button onClick={() => handleApplyClick(stock.symbol)}>APPLY STRATEGY</Button></Grid>   
                </Grid>
            ))}
        </>
    );
}