import {useEffect, useState} from 'react';
import Box from '@mui/material/Box';
import Container from '@mui/material/Container';
import Typography from '@mui/material/Typography';
import Link from '@mui/material/Link';
import axios from 'axios';
import * as signalR from "@microsoft/signalr";
import { HubConnectionState } from '@microsoft/signalr';

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

const backendServer = `https://projectxgatewayapi-app-20231130.yellowfield-d8e525a6.uksouth.azurecontainerapps.io`;
const connection = new signalR.HubConnectionBuilder()
    .withUrl(`${backendServer}/streamHub`)
    .configureLogging(signalR.LogLevel.Information)
    .build();

async function start() {
    if (connection.state != HubConnectionState.Disconnected) {
        return;
    }
    try {
        await connection.start();
        console.log("SignalR Connected.");
    } catch (err) {
        console.log(err);
        setTimeout(start, 5000);
    }
};

connection.onclose(async () => {
    await start();
});

export default function Footer(props: FooterProps) {  
    useEffect(() => {
        start();        
        getData();
    }, []);

  const { description, title } = props;    
    const [serverData, setServerData] = useState<any>([]);
    const [lastCalcType, setLastCalcType] = useState<any>('');
    const [lastResultsCount, setLastResultsCount] = useState<any>('');
    const [lastCalcTime, setLastCalcTime] = useState<any>('');

  const getData = async () => {    
      setServerData("Ciao!");     
      await axios.get(backendServer).then((response: { data: any; }) => {
      setServerData(response.data);
    })
    .catch((error: string) => {
      console.log("Backend communication error: " + error);
    });    
    };

    // suscribe to signalR events
    connection.on("PricingResults", (pricingResults) => {
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
        setLastCalcType(calcType);
        setLastResultsCount(pricingResults["resultsCount"]);
        setLastCalcTime(pricingResults["auditTrail"]["elapsedMilliseconds"])
    });
    connection.on("PlotResults", (plotResults) => {
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
          GatewayAPI Backend says: {JSON.stringify(serverData)}         
        </Typography>
        <Typography
            variant="subtitle1"
            align="center"
            color="text.secondary"
            component="p"
              >
                  GatewayAPI Last Calc Performed: {lastResultsCount} sets of valuationResults completed in {lastCalcTime} ms using the calculator {lastCalcType}
        </Typography>
              
      </Container>
    </Box>
  );
}
