import * as React from 'react';
import Grid from '@mui/material/Grid';
import Paper from '@mui/material/Paper';
import Typography from '@mui/material/Typography';
import FxTicker from './FxTicker';
import StockNews from './StockNews';

interface SidebarProps {  
  description: string;  
  title: string;
}

export default function Sidebar(props: SidebarProps) {
  const { description, title } = props;

  return (
    <Grid item xs={12} md={4}>
      <Paper elevation={0} sx={{ p: 2, bgcolor: 'grey.200' }}>
        <Typography variant="h6" gutterBottom>
          {title}
        </Typography>
        <Typography>{description}</Typography>
      </Paper>
      <Typography variant="h6" gutterBottom sx={{ mt: 3 }}>
        Subscriptions
      </Typography>            
          <FxTicker />
          <StockNews/>
    </Grid>
  );
}
