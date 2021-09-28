using System;
using System.Net.WebSockets;

namespace SharpDownloader.API.Wss
{
    public class Client
    {
        public string Id {get;set;}
        public WebSocket Socket {get;set;}
        public DateTime Creation {get;set;} 
    }
}