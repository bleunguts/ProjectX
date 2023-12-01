import * as signalR from "@microsoft/signalr";
import { HubConnectionState } from "@microsoft/signalr";
import axios from 'axios';

const backendServer = `https://projectxgatewayapi-app-20231130.yellowfield-d8e525a6.uksouth.azurecontainerapps.io`;
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
    startHub: async () => {
        startHubConnection();
    },   
    connection: (): signalR.HubConnection => {
        return connection;
    }
}
export default api; 