import * as React from 'react';
import CssBaseline from '@mui/material/CssBaseline';
import Grid from '@mui/material/Grid';
import Container from '@mui/material/Container';
import { createTheme, ThemeProvider } from '@mui/material/styles';
import Header from './Header';
import MainFeaturedPost from './MainFeaturedPost';
import FeaturedPost from './FeaturedPost';
import Sidebar from './Sidebar';
import Footer from './Footer';
import { featuredPosts }  from './Posts'
import { Main } from './Main';

const sections = [
  { title: 'Home', url: '#' },
  { title: 'Algo Trading Backtesting', url: '#' },  
  { title: 'Market Prediction Intelligence (AI models)', url: '#' },
];

const mainFeaturedPost = {
  title: 'Innovative trading signals platform',
  description:
        "A comprehensive backtest platform for algorithmic trading strategies based on price trends signals and price predictions using AI.",
  image: 'https://source.unsplash.com/random?wallpapers',
  imageText: 'main image description',  
  linkText: '',
};

const sidebar = {
  title: '',
  description: '', 
};

const defaultTheme = createTheme();
const bodyComponents = [
    'The key to a profitable trading strategy lies in the optimization methods applied, and this app provides the tools to visualize the outcomes effectively. Backtesting enables a comprehensive understanding of the strategy and demonstrates its reproducibility by showing how it would have performed in the past. Using the long-short method, the app computes PnL based on Moving Averages within a specified Moving Window (i.e. the number of prices/days). Trades are initiated only after exiting the current position, ensuring that no more than one trade is executed per trading day.',
];
export default function Blog() {
  return (
    <ThemeProvider theme={defaultTheme}>
      <CssBaseline />
      <Container maxWidth="lg">
        <Header title="Trading Signals" sections={sections} />        
        <main>
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
        </main>
      </Container>
      <Footer
        title=""
        description="Think,Change,Do."
      />
    </ThemeProvider>
  );
}
