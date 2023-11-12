import * as React from 'react';
import CssBaseline from '@mui/material/CssBaseline';
import Grid from '@mui/material/Grid';
import Container from '@mui/material/Container';
import { createTheme, ThemeProvider } from '@mui/material/styles';
import Header from './Header';
import MainFeaturedPost from './MainFeaturedPost';
import FeaturedPost from './FeaturedPost';
import Main from './Main';
import Sidebar from './Sidebar';
import Footer from './Footer';
import { featuredPosts }  from './Posts'

const sections = [
  { title: 'Home', url: '#' },
  { title: 'Backtesting', url: '#' },  
  { title: 'Market Analysis Intelligence', url: '#' },
];

const mainFeaturedPost = {
  title: 'Innovative trading signals platform',
  description:
        "A comprehensive tool suite that backtests trading strategies based on price trends analysis.",
  image: 'https://source.unsplash.com/random?wallpapers',
  imageText: 'main image description',  
};

const sidebar = {
  title: 'FX Rates',
  description:
    'Publishes FX prices using in-house pricing based on user customizabled spreads', 
};

const defaultTheme = createTheme();
const bodyComponents = [
    'The key to a profitable strategy is the optimization methods that have been carried out and this app provides the tools to visualize outcomes. Backtesting allows understanding the strategy completelty and can help prove that you can reproduce its results by seeing how it would have performend in the past using long-short method to compute PnL using Moving Average and a specified Moving Window (=number of prices/daily). We enter a new trade AFTER exiting the current position, enforces only one trade at most per trading day.',
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
            <Sidebar
              title={sidebar.title}
              description={sidebar.description}                           
            />
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
