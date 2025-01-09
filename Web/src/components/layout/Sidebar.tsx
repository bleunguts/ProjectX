import * as React from 'react';
import Grid from '@mui/material/Grid';
import Typography from '@mui/material/Typography';
import FxTicker from './FxTicker';
import StockNews from './StockNews';

export default function Sidebar() {  

  return (
    <Grid item xs={12} md={4}>      
      <Typography variant="h6" gutterBottom sx={{ mt: 3 }}>
        FX Rates
      </Typography>            
      <Typography variant="caption" align="center" gutterBottom>
        Publishes FX prices using in-house pricing based on user customizabled spreads
       </Typography>
       <FxTicker />
       <StockNews/>
    </Grid>
  );
}
