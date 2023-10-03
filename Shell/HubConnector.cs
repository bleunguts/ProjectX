using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Options;
using ProjectX.Core.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shell
{
    public interface IHubConnector
    {
        Task Start();
        Task Stop();

        HubConnection Connection { get; }
    }

    [Export(typeof(IHubConnector)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class HubConnector : IHubConnector
    {
        private readonly HubConnection _connection;

        public HubConnection Connection => _connection;

        [ImportingConstructor]
        public HubConnector(IOptions<GatewayApiClientOptions> options)
        {
            _connection = new HubConnectionBuilder()
                .WithUrl(new Uri($"{options.Value.BaseUrl}/streamHub"))
                .WithAutomaticReconnect()
                .Build();
        }

        public async Task Start()
        {
            if (_connection.State != HubConnectionState.Connected)
            {                
                await _connection.StartAsync();
            }
        }

        public async Task Stop()
        {
            // dont dispose connection each time the page loads
            // rather think about disposing when application finishes
            //await _connection.DisposeAsync();
        }
    }
}
