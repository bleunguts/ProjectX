import Typography from '@mui/material/Typography';
import * as React from 'react';
import { gainers, mostActive } from './StockNewsData';
import Grid from '@mui/material/Grid';
export default function StockNews() {
    return (
        <>
            <Typography variant="h6" align="center" gutterBottom>
            Stock Market Highest Gainers
            </Typography>
            {gainers.slice(0, 5).map((gainer) =>
            (
                <Grid container direction='row' spacing={0.5} alignItems="flex-start" alignContent='center'>
                    <Grid item xs={8}><Typography><b>{gainer.symbol}</b></Typography></Grid>
                    <Grid item xs={2}><Typography>${gainer.price.toFixed(1)}</Typography></Grid>
                    <Grid item xs={2}><Typography>{gainer.changesPercentage.toFixed(1)}%</Typography></Grid>                                        
                </Grid>
            ))}
            <Typography variant="h6" align="center" gutterBottom>
                Stock Market Most Active Companies
            </Typography>
            {mostActive.slice(0, 5).map((stock) =>
            (
                <Grid container direction='row' spacing={0.5} alignItems="flex-start" alignContent='center'>
                    <Grid item xs={8}><Typography><b>{stock.symbol}</b></Typography></Grid>
                    <Grid item xs={2}><Typography>${stock.price.toFixed(1)}</Typography></Grid>
                    <Grid item xs={2}><Typography>{stock.changesPercentage.toFixed(1)}%</Typography></Grid>
                </Grid>
            ))}
        </>
    );
}