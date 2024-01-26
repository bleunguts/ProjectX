import {useEffect, useState} from 'react';
import Box from '@mui/material/Box';
import Container from '@mui/material/Container';
import Typography from '@mui/material/Typography';
import Link from '@mui/material/Link';
import axios, { AxiosResponse } from 'axios';
import * as signalR from "@microsoft/signalr";
import { HubConnectionState } from '@microsoft/signalr';
import api from '../../api';

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

interface CalcDetail {
    calcType: string;
    resultsCount: string;
    calcTime: string;
}
export default function Footer(props: FooterProps) {  
    const { description, title } = props;
    const [backendData, setBackendData] = useState<any>("Ciao!");
    const [lastCalcDetail, setCalcDetail] = useState<CalcDetail | null>(null);    

    useEffect(() => {
        connectToSignalR();        
        getBackendData();
    }, []);

    const connectToSignalR = () => {
        api
            .startHub();
    }

    const getBackendData = () => {
        api
            .fetchHealthCheck()
            .then((res) => setBackendData((res as AxiosResponse<any, any>).data))
            .catch((error) => console.log(`Error occured in fetching data from backend: ${error}`));
    }; 
   
    // suscribe to signalR events
    api.connection().on("PricingResults", (pricingResults) => {
        console.log("Client recieved a PricingResult" + JSON.stringify(pricingResults));
        let calcType = "unknown";
        switch (pricingResults["auditTrail"]["calculatorType"]) {
            case 0:
                calcType = "BlackScholesCSharpPricer";
                break;
            case 1:
                calcType = "BlackScholes C++ Pricer";
                break;
            case 2: 
                calcType = "MonteCarlo C++ Pricer 1e^6 paths";
                break;
        }
        setCalcDetail({
            calcType: calcType,
            resultsCount: pricingResults["resultsCount"],
            calcTime: pricingResults["auditTrail"]["elapsedMilliseconds"]
        });        
    });
    api.connection().on("PlotResults", (plotResults) => {
        console.log("Client received a PlotResult" + JSON.stringify(plotResults));
    });

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
          GatewayAPI Backend says: {JSON.stringify(backendData)}         
        </Typography>
        <Typography
            variant="subtitle1"
            align="center"
            color="text.secondary"
            component="p"
              >
            GatewayAPI Last Calc Performed: {lastCalcDetail?.resultsCount} sets of valuationResults completed in {lastCalcDetail?.calcTime} ms using the calculator {lastCalcDetail?.calcType}
        </Typography>              
      </Container>
    </Box>
  );
}
