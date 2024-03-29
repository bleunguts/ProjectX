import * as React from 'react';
import { LineChart, Line, XAxis, YAxis, ResponsiveContainer, Tooltip, CartesianGrid, Legend } from 'recharts';
import Typography from '@mui/material/Typography';
import Grid from '@mui/material/Grid';

// create data for chart
export interface ChartProps {
  data : unknown[],
  label: string,
}

export default function Chart(props: ChartProps) {  
  const data = props.data;
  const label = props.label;
  
  return (
    <React.Fragment>
      <Grid container direction="row" spacing={1.5} alignItems="right" justifyContent="flex-end" alignContent='center'>
        <Grid item> 
        <select>
          <option value="LongShortMean">Long-Short (Bollinger)</option>
          <option value="LongShortMomentum" disabled={true}>Long-Short (RSI)</option>
          <option value="PairsMean" disabled={true}>Correlated Pairs (Bollinger)</option>
          <option value="PairsMomentum" disabled={true}>Correlated Pairs (WilliamsR)</option>
        </select>
        </Grid>
      </Grid>
      <Typography component="h6" variant="body1" color="primary" gutterBottom>
      {label}
      </Typography>
      <ResponsiveContainer>
      <LineChart  data={data}>
        <YAxis yAxisId={0} orientation='left'/>
        <YAxis yAxisId={1} orientation='right'/>
        <XAxis dataKey="time" />
        <Tooltip />
        <CartesianGrid stroke="#f5f5f5" />
        <Line type="monotone" dataKey="amount" stroke="#8884d8" activeDot={{r:8}} strokeWidth='6' yAxisId={0} name='Strategy Pnl($)' />
        <Line type="monotone" dataKey="amountHold" stroke="#82ca9d" strokeWidth='2' yAxisId={1} name='Buy & Hold Benchmark Pnl($)' />
        <Tooltip/>
        <Legend/>
      </LineChart>
      </ResponsiveContainer>
    </React.Fragment>
  );
}