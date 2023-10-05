import * as React from 'react';
import { useTheme } from '@mui/material/styles';
import { LineChart, Line, XAxis, YAxis, Label, ResponsiveContainer } from 'recharts';
import Typography from '@mui/material/Typography';

// create data for chart
function createData(time: string, amount?: number) {
  return { time, amount };
}

//const data = [
//  createData('00:00', 0),
//  createData('03:00', 300),
//  createData('06:00', 600),
//  createData('09:00', 800),
//  createData('12:00', 1500),
//  createData('15:00', 2000),
//  createData('18:00', 2400),
//  createData('21:00', 2400),
//  createData('24:00', undefined),
//];
const data = [
    createData('0.1', 0.25079345703125),
    createData('0.15', 0.19720458984375),
    createData('0.2', 0.17559814453125),
    createData('0.25', 0.16387939453125),
    createData('0.3', 0.15655517578125),
    createData('0.35', 0.15167236328125),
    createData('0.4', 0.14825439453125),
    createData('0.45', 0.14581298828125),
    createData('0.5', 0.14422607421875),
    createData('0.55', 0.14300537109375),
    createData('0.6', 0.14422607421875),
    createData('0.65', 0.14581298828125),
    createData('0.7', 0.14825439453125),
    createData('0.75', 0.15167236328125),
    createData('0.8', 0.15655517578125),
    createData('0.85', 0.16387939453125),
    createData('0.9', 0.17559814453125),
    createData('0.95', 0.19720458984375),
    createData('0.1', 0.25079345703125),
]
export default function Chart() {
  const theme = useTheme();

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
