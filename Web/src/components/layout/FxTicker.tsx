import * as React from 'react';
import Typography from '@mui/material/Typography';
import Grid from '@mui/material/Grid';
import Button from '@mui/material/Button';
import FxRate from './FxRate';

export default function FxTicker() {    
    return (
        <>
            <FxRate ccyPairs={[
                "EURUSD: 1.0512/1.0509",
                "GBPUSD: 1.2550/1.2109",
                "USDJPY: 149.94/149.95",
                "USDCHF: 0.8952/0.8955",
                "AUDUSD: 0.6330/0.6331",
                "USDCAD: 1.3776/1.3779",
                "NZDUSD: 0.5821/0.5822",
                "BTCUSD: 34,223/34,222"                
                ]} />            
        </>
    );
}