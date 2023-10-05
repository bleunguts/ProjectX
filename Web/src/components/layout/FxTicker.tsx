import * as React from 'react';
import Typography from '@mui/material/Typography';
import Grid from '@mui/material/Grid';
import Button from '@mui/material/Button';
import FxRate from './FxRate';

export default function FxTicker() {    
    return (
        <>
            <FxRate ccyPairs={["EURUSD: 1.05123/1.05095","GBPUSD: 1.255009/1.21093"]} />            
        </>
    );
}