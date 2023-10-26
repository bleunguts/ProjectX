import * as React from 'react';
import { useTheme } from '@mui/material/styles';
import { LineChart, Line, XAxis, YAxis, Label, ResponsiveContainer } from 'recharts';
import Typography from '@mui/material/Typography';

export interface ChartData {
  time: string,
  amount?: number
}
// create data for chart
export interface ChartProps {
  data : ChartData[];  
}

export function createData(time: string, amount?: number) : ChartData {
  return { time, amount };
}

export default function Chart(props: ChartProps) {
  const theme = useTheme();
  const data = props.data;

  return (
    <React.Fragment>
      <Typography component="h2" variant="h6" color="primary" gutterBottom>
              Volatility Chart
      </Typography>
      <ResponsiveContainer>
        <LineChart
          data={data}
          margin={{
            top: 16,
            right: 16,
            bottom: 0,
            left: 24,
          }}
        >
          <XAxis
            dataKey="time"
            stroke={theme.palette.text.secondary}
            style={theme.typography.body2}                      
          />
          <YAxis
            stroke={theme.palette.text.secondary}
            style={theme.typography.body2}            
          >
            <Label
              angle={270}
              position="left"
              style={{
                textAnchor: 'middle',
                fill: theme.palette.text.primary,
                ...theme.typography.body1,
              }}
            >
              Volatility
            </Label>
          </YAxis>
          <Line
            isAnimationActive={false}
            type="monotone"
            dataKey="amount"
            stroke={theme.palette.primary.main}
            dot={false}
          />
        </LineChart>
      </ResponsiveContainer>
    </React.Fragment>
  );
}
