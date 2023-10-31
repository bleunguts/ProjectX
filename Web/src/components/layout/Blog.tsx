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
        "The purpose of Trading Signals is provide a platform for designing and optimization of trading strategies, and being able to exercise back testing on the strategies targeted to the personal investor.  My ethos is simple, clutter-free effective tooling to assist in personal trading strategies development.",
  image: 'https://source.unsplash.com/random?wallpapers',
  imageText: 'main image description',  
};

const sidebar = {
  title: 'FX Rates',
  description:
    'Publishes FX prices using in-house pricing based on spreads sourced from the market or configurable by the user', 
};

// TODO remove, this demo shouldn't need to reset the theme.
const defaultTheme = createTheme();
const bodyComponents = [
    'The key for a profitable strategy is the optimization methods that have been carried out and this app provides the toolkit to support this endeavour.',    
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
