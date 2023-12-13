import Typography from '@mui/material/Typography';
import * as React from 'react';
import { gainers, mostActive } from './StockNewsData';
import Grid from '@mui/material/Grid';
import Button from '@mui/material/Button';
import { SyntheticEvent, useState } from 'react';

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
                Stock Symbol: <b>{stockSymbol}</b>
            </Typography>
            <Typography variant="h6" align="center" gutterBottom>
            Stock Market Highest Gainers
            </Typography>
            {gainers.slice(0, 5).map((gainer) =>
            (
                <Grid container direction='row' spacing={0.5} alignItems="center" justifyContent="center" alignContent='center'>
                    <Grid item xs={5}><Typography><b>{gainer.symbol}</b></Typography></Grid>
                    <Grid item xs={2}><Typography>${gainer.price.toFixed(1)}</Typography></Grid>
                    <Grid item xs={2}><Typography>{gainer.changesPercentage.toFixed(1)}%</Typography></Grid> 
                    <Grid item xs={1}><Button onClick={() => handleApplyClick(gainer.symbol)}>APPLY</Button></Grid>                                       
                </Grid>
            ))}
            <Grid container direction='row-reverse' spacing={0.5}>
                <Grid item xs={2}><Button onClick={handleHighestGainersClick}>Refresh</Button></Grid>
            </Grid>
            <Typography variant="h6" align="center" gutterBottom>
                Stock Market Most Active Companies
            </Typography>
            {mostActive.slice(0, 5).map((stock) =>
            (
                <Grid container direction='row' spacing={0.5} alignItems="center" alignContent='center'>
                    <Grid item xs={8}><Typography><b>{stock.symbol}</b></Typography></Grid>
                    <Grid item xs={2}><Typography>${stock.price.toFixed(1)}</Typography></Grid>
                    <Grid item xs={2}><Typography>{stock.changesPercentage.toFixed(1)}%</Typography></Grid>
                    <Grid item xs={1}><Button onClick={() => handleApplyClick(gainer.symbol)}>APPLY</Button></Grid>   
                </Grid>
            ))}
            <Grid container direction='row-reverse' spacing={0.5}>
                <Grid item xs={2}><Button onClick={handleMostActiveClick}>Refresh</Button></Grid>
            </Grid>
        </>
    );
}