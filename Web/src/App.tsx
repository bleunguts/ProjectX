import { Route, Routes, BrowserRouter as Router} from 'react-router-dom';
import { createTheme, ThemeProvider } from '@mui/material/styles';
import { CssBaseline, Container } from '@mui/material'
import './App.css'
import Blog from './components/layout/Blog'
import Footer from './components/layout/Footer'
import Header from './components/layout/Header'
import MarketPredictionBlog from './components/layout/MarketPredictionBlog';

const sections = [
  { title: 'Home', url: '/' },
  { title: 'Algo Trading Backtesting', url: '/' },  
  { title: 'Market Prediction Intelligence (AI models)', url: '/Market' },
];

const defaultTheme = createTheme();
function App() {  
    return (       
    <>               
    <Router>     
      <ThemeProvider theme={defaultTheme}>
        <CssBaseline />      
        <Container maxWidth="lg">
          <Header title="Trading Signals" sections={sections} />         
          <main>
            <Routes>
              <Route path="/" element = {<Blog/>}/>
              <Route path="/Market" element = {<MarketPredictionBlog/>}/>
            </Routes>
          </main>               
        </Container>
        <Footer
          title=""
          description="Think,Change,Do."
        />
      </ThemeProvider>
    </Router>       
    </>
  )
}

export default App
