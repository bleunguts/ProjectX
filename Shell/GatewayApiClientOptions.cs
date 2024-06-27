using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;

namespace Shell
{
    public class GatewayApiClientOptions
    {
        [Required, Url]
        public string BaseUrl { get; set; }
        [Required, Url]
        public string SignalRUrl { get; set; }        
        public bool? ForceDisableSignalR { get; set; }
    }
}
