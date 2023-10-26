import * as React from 'react';
import { useTheme } from '@mui/material/styles';
import { LineChart, Line, XAxis, YAxis, Label, ResponsiveContainer, Tooltip, CartesianGrid, Legend } from 'recharts';
import Typography from '@mui/material/Typography';

export interface ChartData {
  time: string,
  amount: number
}

export function createData(time: string, amount: number) : ChartData {
  return { time, amount };
}

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
      <Typography component="h2" variant="body1" color="primary" gutterBottom>
      {label}
      </Typography>
      <ResponsiveContainer>
      <LineChart width={400} height={400} data={data} margin={{ top: 5, right: 20, left: 10, bottom: 5 }}>
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