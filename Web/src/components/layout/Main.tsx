import * as React from 'react';
import Grid from '@mui/material/Grid';
import Typography from '@mui/material/Typography';
import Divider from '@mui/material/Divider';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import Paper from '@mui/material/Paper';
import Chart from './Chart';
import { defineConfig } from 'vite';

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
                style={{
                    width: 800,
                }}>
                <CardContent>
                    <Typography style={{
                        marginBottom: 12,
                    }}>
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
                            height: 240,
                        }}
                    >
                        <Chart />
                    </Paper>
                </CardContent>
            </Card>
                    {/* Item 2*/}
                    <Card
                style={{
                    width: 800,
                }}>
                <CardContent>
                    <Typography style={{
                        marginBottom: 12,
                    }}>
                    Future Roadmap features:
                    </Typography>                
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
                </CardContent>
            </Card>
        </Grid>
    );
}
