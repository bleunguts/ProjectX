﻿using Microsoft.Extensions.Options;

namespace Shell
{
    public class GatewayApiClientOptions
    {
        public string BaseUrl { get; set; }   
        public bool ForceDisableSignalR { get; set; }
    }
}
