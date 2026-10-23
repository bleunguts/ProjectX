using System.ComponentModel.DataAnnotations;

namespace ShellX.Services
{
    public class GatewayApiClientOptions
    {
        [Required, Url]
        public string BaseUrl { get; set; } = string.Empty;
        [Required, Url]
        public string SignalRUrl { get; set; } = string.Empty;        
        public bool? ForceDisableSignalR { get; set; }
    }
}
