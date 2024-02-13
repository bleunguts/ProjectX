import * as signalR from "@microsoft/signalr";
import { HubConnectionState } from "@microsoft/signalr";
import axios, { AxiosResponse } from 'axios';
import { v1 as uuidv1 } from 'uuid';

const config = () => {
    console.log(process.env);
    return {
        BackendServer : process.env.BACKEND_SERVER
    }
}

const backendServer = config().BackendServer ??  `https://projectxgatewayapi-app-20231130.mangorock-d77aee79.uksouth.azurecontainerapps.io`;

const connection = new signalR.HubConnectionBuilder()
    .withUrl(`${backendServer}/streamHub`,
    {
        accessTokenFactory: () => 'zpq+oW5R6HtWi0Or1XAI0BJcLHTaXOlQHLhRHo7A8IQ=',
        withCredentials: false
    })
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
        console.log(`SignalR Connection error: {err}`);
        setTimeout(startHubConnection, 5000);
    }
}

connection.onclose(async () => {
    await startHubConnection();
});

export interface BackenedApi {
    fetchHealthCheck(): Promise<string | AxiosResponse<unknown,unknown>>,
    fetchHighestGainerStocks(limitRows: number): Promise<AxiosResponse<unknown,unknown>>,
    fetchMostActiveStocks(limitRows: number): Promise<AxiosResponse<unknown,unknown>>,
    fetchLongShortStrategy(ticker: string): Promise<AxiosResponse<unknown,unknown>>,
    submitFxRateSubscribeRequest(ccyName: string): Promise<AxiosResponse<unknown,unknown>>,
    submitFxRateUnsubscribeRequest(ccyName: string): Promise<AxiosResponse<unknown,unknown>>,
    startHub(): void,
    connection(): signalR.HubConnection
}

const api: BackenedApi = {    
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
    fetchLongShortStrategy: async (ticker: string) => {
        const endpoint = `${backendServer}/BacktestService/ComputeLongShortPnlStrategyChartData?ticker=${ticker}&fromDate=2023-05-01&toDate=2023-09-25&notional=10000`;
        console.log(`Fetching from ${endpoint}`);
        return await axios.get(endpoint);
    },
    submitFxRateSubscribeRequest: async (ccyName: string) => {
        const requestId = uuidv1();
        const body = {
            "currencyPair": ccyName,
            "clientName": "Web",
            "mode": 0, // subscribe
            "request": requestId,
        };
        return await axios.post(`${backendServer}/FXPricing`, body);
    },
    submitFxRateUnsubscribeRequest: async (ccyName: string) => {
        const requestId = uuidv1();
        const body = {
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
