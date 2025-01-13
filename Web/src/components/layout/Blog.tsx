import * as React from 'react';
import Grid from '@mui/material/Grid';
import MainFeaturedPost from './MainFeaturedPost';
import FeaturedPost from './FeaturedPost';
import Sidebar from './Sidebar';
import { featuredPosts }  from './Posts'
import { Main } from './Main';

const mainFeaturedPost = {
  title: 'Innovative trading signals platform',
  description:
        "A backtest platform for algo trading strategies based on price trends signals and price predictions using AI.",
  image: 'https://source.unsplash.com/random?wallpapers',
  imageText: 'main image description',  
  linkText: '',
};

const bodyComponents = [
    'Backtesting enables a comprehensive understanding of a strategy by demonstrating its reproducibility by running it against past live data. Using the long-short method, the app computes PnL based on Moving Averages within a specified Moving Window (i.e. the number of prices/days). Trades are initiated only after exiting the current position, ensuring that no more than one trade is executed per trading day.',
];
export default function Blog() {
  return (    
      <div>
        <MainFeaturedPost post={mainFeaturedPost} />
        <Grid container spacing={4}>
          {featuredPosts.map((post) => (
            <FeaturedPost key={post.title} post={post} />
          ))}
        </Grid>
        <Grid container spacing={5} sx={{ mt: 3 }}>
          <Main title="Mean-reversion strategies" posts={bodyComponents} />
          <Sidebar/>
        </Grid>
      </div>    
  );
}
