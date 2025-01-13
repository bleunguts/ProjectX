import * as react from "react";
import { Grid, Typography, Divider, Card, CardContent, Paper } from "@mui/material";

interface MainProps {
    posts: ReadonlyArray<string>;
    title: string;
}

export default function MarketPrediction(props: MainProps) {
    const {posts, title} = props;
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
                    ML model applied.
                    </Typography>                                                                    
                    </Paper>
                </CardContent>
            </Card>          
        </Grid>
    );
}