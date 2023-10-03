using Microsoft.AspNetCore.SignalR.Client;
using ProjectX.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shell
{
    public interface IHubConnector
    {
        Task Start();
        HubConnection Connection { get; }
    }

    public class HubConnector : IHubConnector
    {
        private readonly HubConnection _connection;

        public HubConnection Connection => _connection;

        public HubConnector(string baseUrl)
        {
            _connection = new HubConnectionBuilder()
                .WithUrl(new Uri($"{baseUrl}/streamHub"))
                .WithAutomaticReconnect()
                .Build();
        }

        public async Task Start()
        {
            await _connection.StartAsync();                                           
        }        
    }
}
