import * as signalR from "@microsoft/signalr";
import { HubConnectionState } from "@microsoft/signalr";
import axios from 'axios';
import { v1 as uuidv1 } from 'uuid';

const backendServer =  process.env.BACKEND_SERVER || `https://projectxgatewayapi-app-20231130--x2a6t10.icybay-6c4fad7d.westus2.azurecontainerapps.io`;
//const backendServer = `https://localhost:8081`;

const connection = new signalR.HubConnectionBuilder()
    .withUrl(`${backendServer}/streamHub`)
    .configureLogging(signalR.LogLevel.Information)
    .build();

async function startHubConnection() {
    if (connection.state != HubConnectionState.Disconnected) {
        return;
    }
    try {
        await connection.start();
        console.log("SignalR Connected.");
    } catch (err) {
        console.log(err);
        setTimeout(startHubConnection, 5000);
    }
};

connection.onclose(async () => {
    await startHubConnection();
});

const api = {    
    fetchHealthCheck: async () => {
        return await axios.get(backendServer)
            .then((response) => response)
            .catch((error: string) => error);
    },
    fetchHighestGainerStocks: async (limitRows: number) => {
        const endpoint =`${backendServer}/StockMarketInsights/HighestGainerStocks/${limitRows}`;
        console.log(`Fetching from ${endpoint}`);
        return await axios.get(endpoint);
    },
    fetchMostActiveStocks: async (limitRows: number) => {
        const endpoint = `${backendServer}/StockMarketInsights/MostActiveStocks/${limitRows}`;
        console.log(`Fetching from ${endpoint}`);
        return await axios.get(endpoint);
    },
    submitFxRateSubscribeRequest: async (ccyName: string) => {
        let requestId = uuidv1();
        let body = {
            "currencyPair": ccyName,
            "clientName": "Web",
            "mode": 0, // subscribe
            "request": requestId,
        };
        return await axios.post(`${backendServer}/FXPricing`, body);
    },
    submitFxRateUnsubscribeRequest: async (ccyName: string) => {
        let requestId = uuidv1();
        let body = {
            "currencyPair": ccyName,
            "clientName": "Web",
            "mode": 1, // unsubscribe
            "request": requestId,
        };
        return await axios.post(`${backendServer}/FXPricing`, body);
    },
    startHub: async () => {
        startHubConnection();
    },   
    connection: (): signalR.HubConnection => {
        return connection;
    }
}
export default api; 

function uuidv4() {
    throw new Error("Function not implemented.");
}
