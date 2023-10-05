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
        "The purpose of Trading Signals is provide a platform for coming up with trading strategies, and being able to exercise back testing on the strategies for personal stock investments.  My ethos is simple, easy to use, and without clutter effective tooling for small scale personal home trading.",
  image: 'https://source.unsplash.com/random?wallpapers',
  imageText: 'main image description',  
};

const featuredPosts = [
  {
        title: 'Financial Quant Development',
        date: 'Jul 23',
        description:
            `As the main purpose is to design a trading signals app when I was looking at my personal trading books on HSBC, 
                it also acts as a show case in my keen interest in financial quant development in C#/C++, a decade of experience in risk management systems and trading execution.
                Employing cutting edge tech stack for a scalable solution and cloud ready C# 11,.NET 7,WPF,Caliburn.Micro,ComponentModel.CompositionContainer (IoC), AspNet Core 6, ReactiveX, Background Services.
                I wrote the Quant models in c# (black-scholes, barrier options, bonds pricing, CDS pricing) and some options C++ (bs, pseudo random generators)`,
        image: './signals.PNG',
        imageLabel: 'Image Text',
  },
  {
    title: 'Mean Reversion Trading signals',
    date: 'Sep 23',
    description:
      'The main purpose is to generate trading signals such as mean reversion and provide backtesting',
    image: './signals.PNG',
    imageLabel: 'Image Text',
  },
  {
    title: 'Arbitrage Trading signals',
    date: 'Oct 23',
    description:
          `I have come up with my own Arbitrage Trading signals which is quite complex. The app provides various pricers that I have developed for options, fixed income, FX products and will show actual market prices to compare with.  
            This provides analytics tools on mismatches in function inputs such as (vol, spot prices) that can expose potential arbitrage ops`,
      image: './signals.PNG',
    imageLabel: 'Image Text',
    },
    {
        title: 'Market Data',
        date: 'Oct 23',
        description:
            `As I have been using Quandl to source market data over the internet, I found a great api which provides not only stock information but financial summaries and economic news.  I will be trialing this, stay tuned!`,
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
        title=""
        description="Think,Change,Do."
      />
    </ThemeProvider>
  );
}
