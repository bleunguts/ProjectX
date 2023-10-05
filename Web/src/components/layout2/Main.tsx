import * as React from 'react';
import Grid from '@mui/material/Grid';
import Typography from '@mui/material/Typography';
import Divider from '@mui/material/Divider';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import Paper from '@mui/material/Paper';
import Chart from './Chart';

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
            {/* Item 2*/}
            <Card
                style={{
                    width: 800,
                }}>
                <CardContent>
                    <Typography style={{
                        marginBottom: 12,
                    }}>
                        'Pair stock mean reversion signal backtesting, includes visual graphs and real market sourced historical pricing data'
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
        </Grid>
    );
}
