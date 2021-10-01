using System;
using System.Net.WebSockets;
using System.Threading.Tasks;
using SharpDownloader.Integration.Observer;
using SharpDownloader.Wss.Enum;

namespace SharpDownloader.Wss.Models
{
    public class WssClient : IObserver
    {
        public WebSocket Socket {get;set;}
        public string Id {get;set;} = Guid.NewGuid().ToString();

        private SocketManager _manager;
        public WssClient(SocketManager manager)
        {
            _manager = manager;
        }
        public async Task Update(ISubject subject)
        {
            await _manager.SendMessage(Id, subject, SndCommands.DownloadProgress);
        }
    }
}