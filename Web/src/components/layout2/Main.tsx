import * as React from 'react';
import Grid from '@mui/material/Grid';
import Typography from '@mui/material/Typography';
import Divider from '@mui/material/Divider';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';

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
    {posts.map((post) => (
        <Card
            style={{
                width: 800,                      
            }}>
            <CardContent>
                <Typography style={{
                    marginBottom: 12,
                }}>
                    {post}
                </Typography>                
            </CardContent>
        </Card>
    ))}      
    </Grid>
  );
}
