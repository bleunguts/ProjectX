import * as React from 'react';
import Typography from '@mui/material/Typography';
import Grid from '@mui/material/Grid';
import Button from '@mui/material/Button';
import Grid2 from '@mui/material/Unstable_Grid2/Grid2';

export default function FxTicker() {
    return (
        <>
            <Grid container direction='row' spacing={0.5} alignItems="flex-start" alignContent='center'>
                <Grid item xs={8}><Typography><b>EURUSD: 1.05123/1.05095</b></Typography></Grid>
                <Grid item xs={2}><Button>Pull</Button></Grid>
                <Grid item xs={2}><Button>Stop</Button></Grid>
            </Grid>       
            <Grid container direction='row' spacing={0.5} alignItems="flex-start" alignContent='center'>
                <Grid item xs={8}><Typography><b>GBPUSD: 1.255009/1.21093</b></Typography></Grid>
                <Grid item xs={2}><Button>Pull</Button></Grid>
                <Grid item xs={2}><Button>Stop</Button></Grid>
            </Grid>  
        </>
    );
}