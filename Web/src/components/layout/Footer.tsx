import {useEffect, useState} from 'react';
import Box from '@mui/material/Box';
import Container from '@mui/material/Container';
import Typography from '@mui/material/Typography';
import Link from '@mui/material/Link';
import axios from 'axios';

function Copyright() {
  return (
    <Typography variant="body2" color="text.secondary" align="center">
      {'Copyright © '}
      <Link color="inherit" href="https://bleunguts.github.io/bleunguts/">
        bleunguts
      </Link>{' '}
      {new Date().getFullYear()}
      {'.'}
    </Typography>
  );
}

interface FooterProps {
  description: string;
  title: string;
}

export default function Footer(props: FooterProps) {  
  useEffect(() => { getData(); }, []);

  const { description, title } = props;    
  const [serverData, setServerData] = useState<any>([]);

  const getData = async () => {    
    setServerData("Ciao!");
    await axios.get(`https://projectxgatewayapi-app-20231130.yellowfield-d8e525a6.uksouth.azurecontainerapps.io`).then(response => {
      setServerData(response.data);
    })
    .catch(error => {
      console.log("Backend communication error: " + error);
    });    
  };

  return (
    <Box component="footer" sx={{ bgcolor: 'background.paper', py: 6 }}>
      <Container maxWidth="lg">
        <Typography variant="h6" align="center" gutterBottom>
          {title}
        </Typography>        
        <Typography
          variant="subtitle1"
          align="center"
          color="text.secondary"
          component="p"
        >          
          {description}
        </Typography>
        <Copyright />
        <Typography  
          variant="subtitle1"
          align="center"
          color="text.secondary"
          component="p"
        >
          GatewayAPI Backend says: {JSON.stringify(serverData)}
        </Typography>
      </Container>
    </Box>
  );
}
