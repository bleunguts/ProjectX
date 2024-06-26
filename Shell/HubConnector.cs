using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
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
        private readonly string _baseUriString;
        private HubConnection _connection;

        public HubConnection Connection => _connection;

        [ImportingConstructor]
        public HubConnector(IOptions<GatewayApiClientOptions> options)
        {
            _baseUriString = $"{options.Value.SignalRUrl}";
            if (options.Value.ForceDisableSignalR.HasValue)
            {
                _connection = Connect(_baseUriString);
            }           
        }

        public async Task Start()
        {            
            // starts connection back up
            if (_connection != null && _connection.State != HubConnectionState.Connected)
            {
                _connection = Connect(_baseUriString);

                await _connection.StartAsync();
            }
        }     

        private HubConnection Connect(string baseUri)
        {
            return new HubConnectionBuilder()                
                .WithUrl(new Uri(baseUri))
                .AddMessagePackProtocol()
                .WithAutomaticReconnect()
                .Build();
        }

        public async Task Stop()
        {
            if(_connection != null)
            {
                // disposes each time page loads
                await _connection.DisposeAsync();
            }            
        }
    }
}
