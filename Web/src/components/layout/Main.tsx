import * as React from 'react';
import Grid from '@mui/material/Grid';
import Typography from '@mui/material/Typography';
import Divider from '@mui/material/Divider';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import Paper from '@mui/material/Paper';
import Chart, { createData } from './Chart';
import { defineConfig } from 'vite';
import { ChartData } from './Chart';
import { BoltRounded } from '@mui/icons-material';
import { FakeStrategyChartData } from './DummyData';
import Chart3D from './Chart3D';

interface MainProps {
    posts: ReadonlyArray<string>;
    title: string;
}

export default function Main(props: MainProps) {
    const { posts, title } = props;

    return (
        <Grid
            item
            xs={12}
            md={8}
            sx={{
                '& .markdown': {
                    py: 3,
                },
            }}
        >
            <Typography variant="h6" gutterBottom>
                {title}
            </Typography>
            <Divider />
            {/* Item 1*/}
            <Card
                style={{width: 800,}}>
                <CardContent>
                    <Typography style={{}}>
                        'Single stock mean reversion signal backtesting, includes visual graphs and real market sourced historical pricing data',
                    </Typography>
                </CardContent>
            </Card>    
            {/* Item 3*/}
            <Card
                style={{
                    width: 800,
                }}>
                <CardContent>
                    <Paper
                        sx={{
                            p: 2,
                            display: 'flex',
                            flexDirection: 'column',
                            height: 200,
                        }}
                    >              
                    <Chart label='Long & Short Strategy Pnl' data={FakeStrategyChartData}/>                                   
                    </Paper>
                </CardContent>
            </Card>
            {/* Item 4*/}
            <Card
                style={{
                    width: 800,
                }}>
                <CardContent>
                    <Paper sx={{
                            p: 2,
                            display: 'flex',
                            flexDirection: 'column',                            
                        }}
                    >   
                    <Typography style={{                                       
                        fontStyle: 'italic',
                    }}>
                    Market inferred volatility (implied black scholes calc against live option prices)
                    </Typography>   
                    <Chart3D label='Vol Smile' 
                        data={[
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
                    ]}/>                                                    
                    </Paper>
                </CardContent>
            </Card>
            {/* Item 5*/}
            <Card
                style={{
                    width: 800,
                }}>
                <CardContent>
                    <Typography style={{
                        marginLeft: 12,
                        marginBottom: 12,                        
                    }} 
                    align='left'>
                    The Roadmap:
                    </Typography>       
                    <Typography style={{
                        marginLeft: 12,
                        marginBottom: 12,                        
                    }} 
                    align='left'>
                    {
                        [
                            "A way to rapid develop trading strategies and using back testing to validate it", 
                            "Show live FX prices from FX market data source as an additional column 'Reference Price'",
                            "All pricing model calcs are Azure-enabled (leveraging existing AspNetCore BackgroundServices)",
                            "Enhanced eTrader trade management screen shows PnL helps facilitate demo tradin",
                        ].map((x) =>
                        <li>
                        {x}
                        </li>
                        )                                        
                    }           
                    </Typography>                  
                </CardContent>
            </Card>
        </Grid>
    );
}
