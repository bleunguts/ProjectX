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

const sections = [
  { title: 'Home', url: '#' },
  { title: 'Backtesting', url: '#' },  
  { title: 'Market Data', url: '#' },
];

const mainFeaturedPost = {
  title: 'Innovative trading signals engineering platform',
  description:
        "Trading strategies employed mainly consists of mean reversion, momentum, arbitrage strategies. This app allows conceptualization of trading strategies and offers a backtesting platform to simulate the strategy based on historical market sourced prices",
  image: 'https://source.unsplash.com/random?wallpapers',
  imageText: 'main image description',  
};

const featuredPosts = [
  {
    title: 'Mean Reversion Trading signals',
    date: 'Nov 12',
    description:
      'The main purpose is to generate trading signals such as mean reversion and provide backtesting',
    image: './signals.PNG',
    imageLabel: 'Image Text',
  },
  {
    title: 'Arbitrage Trading signals',
    date: 'Nov 11',
    description:
      'This is a more involved component of the system. The app provides various pricers that I have developed for options, fixed income, FX products and will show actual market prices to compare with.  This provides analytics tools on mismatches in function inputs such as (vol, spot prices) that can expose potential arbitrage ops',
      image: './signals.PNG',
    imageLabel: 'Image Text',
  },
];

const sidebar = {
  title: 'FX Rates',
  description:
    'Publishes FX prices using in-house pricing based on spreads sourced from the market or configurable by the user', 
};

// TODO remove, this demo shouldn't need to reset the theme.
const defaultTheme = createTheme();
const bodyComponents = [
    'Single stock mean reversion signal backtesting, includes visual graphs and real market sourced historical pricing data',
    'Pair stock mean reversion signal backtesting, includes visual graphs and real market sourced historical pricing data'
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
            <Main title="Features" posts={bodyComponents} />
            <Sidebar
              title={sidebar.title}
              description={sidebar.description}                           
            />
          </Grid>
        </main>
      </Container>
      <Footer
        title="Footer"
        description="Think,Change,Do."
      />
    </ThemeProvider>
  );
}
