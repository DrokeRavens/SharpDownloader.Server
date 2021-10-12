using System;
using System.Net.WebSockets;
using System.Threading.Tasks;
using SharpDownloader.Integration.Observer;
using SharpDownloader.Wss.Enum;
using SharpDownloader.Wss.Models.Snd;

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
        public async Task UpdateProgress(string id, double progress, string size, string remainingTime)
        {
            if(Socket.State != WebSocketState.Open)
                return; 
            var downProgress = new SndDownloadProgress {
                Id = id,
                Progress = progress,
                SizeProgress = size,
                RemainingTime = remainingTime,
                PacketType = Command.DownloadProgress
            };
            
            await _manager.SendMessage(this.Socket, downProgress);
        }

        public async Task UpdateNewDownload(object downloadBlock)
        {
            if(Socket.State != WebSocketState.Open)
                return; 

            var downProgress = new SndPayload {
                PacketType = Command.NewDownload,
                PacketData = downloadBlock
            };
            
            await _manager.SendMessage(this.Socket, downProgress);
        }
    }
}