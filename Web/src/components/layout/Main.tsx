import * as React from 'react';
import Grid from '@mui/material/Grid';
import Typography from '@mui/material/Typography';
import Divider from '@mui/material/Divider';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import Paper from '@mui/material/Paper';
import Chart from './Chart';
import { FakeVolatilityData } from './DummyData';
import Chart3D from './Chart3D';
import { observer } from 'mobx-react-lite';
import { useRootStore } from '../../RootStoreProvider';

interface MainProps {
    posts: ReadonlyArray<string>;
    title: string;
}

export const Main = observer(function Main(props: MainProps) {
    const { posts, title } = props;
    const { tradingStrategyStore } = useRootStore();

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
                        {posts}
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
                            display: 'flex',
                            flexDirection: 'column',
                            height: 500,
                        }}
                    >              
                    <Typography>{tradingStrategyStore.symbol}</Typography>
                    <Chart label='Long & Short Strategy Pnl' data={tradingStrategyStore.data}/>                                   
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
                            display: 'flex',
                            flexDirection: 'column',
                            height: 400,        
                        }}
                    >   
                    <Typography style={{                                       
                        fontStyle: 'italic',
                    }}>
                    Market inferred volatility (implied black scholes calc against live option prices)
                    </Typography>   
                    <Chart3D label='Vol Smile' zData={FakeVolatilityData} />                                                    
                    </Paper>
                </CardContent>
            </Card>          
        </Grid>
    );
});
