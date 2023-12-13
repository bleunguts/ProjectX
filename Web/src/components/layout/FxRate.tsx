import Button from '@mui/material/Button';
import Grid from '@mui/material/Grid';
import Typography from '@mui/material/Typography';
import * as React from 'react';
import { SyntheticEvent } from 'react';
import api from './api';

export interface CurrencyPairFormatted {
    bid: string
    ask: string
};
export interface CurrencyPair {
    bid: number
    ask: number
};
export interface FxRateProps {
    ccyName: string;
    ccyPair: CurrencyPairFormatted;    
}

export default function FxRate(this: any, props: FxRateProps) {    
    const { ccyName, ccyPair } = props;

    const handleSubscribeClick = (event: SyntheticEvent) => {
        const target = event.target;
        console.log(`Subscribe to currency requested: ${ccyName}`);       
        api.submitFxRateSubscribeRequest(ccyName);
    };

    const handleUnsubscribeClick = (event: SyntheticEvent) => {
        const target = event.target;
        console.log(`Unsubscribe to currency requested: ${ccyName}`);
        api.submitFxRateUnsubscribeRequest(ccyName);
    };

    return (       
        <>              
            <Grid container direction='row' spacing={0.5} alignItems="center" alignContent='center'>
                <Grid item xs={8}><Typography><b>{ccyName}: {ccyPair.bid}/{ccyPair.ask}</b></Typography></Grid>
                <Grid item xs={2}><Button onClick={handleSubscribeClick}>Pull</Button></Grid>
                <Grid item xs={2}><Button onClick={handleUnsubscribeClick}>Stop</Button></Grid>
            </Grid>        
        </>
    );
}