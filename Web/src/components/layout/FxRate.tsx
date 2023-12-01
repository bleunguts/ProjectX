import Button from '@mui/material/Button';
import Grid from '@mui/material/Grid';
import Typography from '@mui/material/Typography';
import * as React from 'react';

export interface CurrencyPair {
    bid: number
    ask: number
};
export interface FxRateProps {
    ccyName: string;
    ccyPair: CurrencyPair;    
}

export default function FxRate(props: FxRateProps) {
    console.log(props);
    const { ccyName, ccyPair } = props;

    console.log(ccyName);
    return (       
        <>              
            <Grid container direction='row' spacing={0.5} alignItems="flex-start" alignContent='center'>
                <Grid item xs={8}><Typography><b>{ccyName}: {ccyPair.bid}/{ccyPair.ask}</b></Typography></Grid>
                <Grid item xs={2}><Button>Pull</Button></Grid>
                <Grid item xs={2}><Button>Stop</Button></Grid>
            </Grid>        
        </>
    );
}