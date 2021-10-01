using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SharpDownloader.Downloader;
using SharpDownloader.Integration.Observer;
using SharpDownloader.Wss.Enum;
using SharpDownloader.Wss.Models;
using SharpDownloader.Wss.Models.Snd;

namespace SharpDownloader.Wss
{
    public class SocketManager : ISocketManager
    {
        private List<WssClient> _clients;
        private DownloadService _downloadService;
        public SocketManager(DownloadService downloadService) 
        {
            _clients = new List<WssClient>();
            _downloadService = downloadService;
        }
        public WssClient Add(WebSocket socket){
            var client = new WssClient(this){
                Socket = socket
            };

            _clients.Add(client);

            return client;
        }
        public Task Remove(WebSocket socket)
        {
            _clients.RemoveAll(c => c.Socket == socket);
            return Task.CompletedTask;
        }
        public async Task Disconnect(string id){
            var client = _clients.SingleOrDefault(c => c.Id.Equals(id));
            await client.Socket.CloseAsync(WebSocketCloseStatus.Empty, null, CancellationToken.None);
            _clients.Remove(client);
        }

        public async Task ProcessMessage(RcvPayload message)
        {
            var sender = _clients.SingleOrDefault(c => c.Id.Equals(message.Id));

            if(sender == null)
                return;

            switch (message.PacketType)
            {
                case RcvCommands.DownloadList:
                    await SendDownloadList(sender);
                    break;
                case RcvCommands.DownloadProgress:
                    SubscribeDownloads(sender);
                    break;
                default:
                    break;
            }
            
        }

        private void SubscribeDownloads(WssClient sender)
        {
            _downloadService.AttachBridge(sender);
        }

        private async Task SendDownloadList(WssClient sender)
        {
            var allDownloads = _downloadService.GetAll(sender.Id);

            await SendMessage(sender.Id, allDownloads, SndCommands.DownloadList);
        }

        public async Task SendMessage(string id, object message, SndCommands type){
            var payload = new SndPayload {
                PacketType = type,
                PacketData = message
            };
            var client = _clients.SingleOrDefault(c => c.Id.Equals(id));

            var messageJson = JsonConvert.SerializeObject(payload);
            await client.Socket.SendAsync(buffer: new ArraySegment<byte>(array: Encoding.ASCII.GetBytes(messageJson),
                                                                    offset: 0,
                                                                    count: messageJson.Length),
                                    messageType: WebSocketMessageType.Text,
                                    endOfMessage: true,
                                    cancellationToken: CancellationToken.None);
        }

        public Task SendMessage(string id, object message, SndPayload type)
        {
            throw new NotImplementedException();
        }

        public void Update(ISubject subject)
        {
            throw new NotImplementedException();
        }
    }
}