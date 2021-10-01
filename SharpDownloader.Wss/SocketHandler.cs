using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SharpDownloader.Wss.Enum;
using SharpDownloader.Wss.Models;
using SharpDownloader.Wss.Models.Snd;

namespace SharpDownloader.Wss
{
    public class SocketHandler : ISocketHandler
    {

        ISocketManager _socketManager;
        public SocketHandler(ISocketManager socketManager)
        {
            _socketManager = socketManager;
        }

        public async Task OnConnected(WebSocket socket)
        {
            Console.WriteLine("Connected.");
            var client = _socketManager.Add(socket);

            await SendMessage(socket, new {
                Id = client.Id
            }, SndCommands.Identity);
        }

        public Task OnDisconnect(WebSocket socket)
        {
            Console.WriteLine("Disconnected.");
            _socketManager.Remove(socket);
            return Task.CompletedTask;
        }

        public Task OnMessage(WebSocket socket, string message)
        {
            Console.WriteLine("Message: " + message);
            var messageObject = JsonConvert.DeserializeObject<RcvPayload>(message);

            _socketManager.ProcessMessage(messageObject);
            
            return Task.CompletedTask;
        }

        public async Task SendMessage(WebSocket socket, object message, SndCommands type){
            var payload = new SndPayload {
                PacketType = type,
                PacketData = message
            };
            var messageJson = JsonConvert.SerializeObject(payload);
            await socket.SendAsync(buffer: new ArraySegment<byte>(array: Encoding.ASCII.GetBytes(messageJson),
                                                                    offset: 0,
                                                                    count: messageJson.Length),
                                    messageType: WebSocketMessageType.Text,
                                    endOfMessage: true,
                                    cancellationToken: CancellationToken.None);
        }
    }
}