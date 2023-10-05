import Button from '@mui/material/Button';
import Grid from '@mui/material/Grid';
import Typography from '@mui/material/Typography';
import * as React from 'react';

const ccyPairs = ["EURUSD: 1.05123/1.05095", "GBPUSD: 1.255009/1.21093"]

export default function FxRate() {
    
    return (       
        <>
        {ccyPairs.map((ccyPair) =>
        (
            <Grid container direction='row' spacing={0.5} alignItems="flex-start" alignContent='center'>      
                <Grid item xs={8}><Typography><b>{ccyPair}</b></Typography></Grid>
                <Grid item xs={2}><Button>Pull</Button></Grid>
                <Grid item xs={2}><Button>Stop</Button></Grid>
            </Grid>
        ))}
        </>
    );
}