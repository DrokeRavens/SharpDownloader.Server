using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SharpDownloader.API.Wss
{
    public abstract class WsHandler
    {
        protected ConnectionManager WebSocketConnectionManager { get; set; }

        public WsHandler(ConnectionManager webSocketConnectionManager)
        {
            WebSocketConnectionManager = webSocketConnectionManager;
        }

        public virtual async Task OnConnected(WebSocket socket)
        {
            WebSocketConnectionManager.AddClient(socket);
        }

        public virtual async Task OnDisconnected(WebSocket socket)
        {
            await WebSocketConnectionManager.RemoveSocket(socket);
        }

        public async Task SendMessageAsync(WebSocket socket, string message)
        {
            if(socket.State != WebSocketState.Open)
                return;

            await socket.SendAsync(buffer: new ArraySegment<byte>(array: Encoding.ASCII.GetBytes(message),
                                                                    offset: 0,
                                                                    count: message.Length),
                                    messageType: WebSocketMessageType.Text,
                                    endOfMessage: true,
                                    cancellationToken: CancellationToken.None);
        }

        public async Task SendMessageAsync(string socketId, string message)
        {
            await SendMessageAsync(WebSocketConnectionManager.GetClientById(socketId).Socket, message);
        }

        public async Task SendMessageToAllAsync(string message)
        {
            foreach(var client in WebSocketConnectionManager.GetAll())
            {
                if(client.Socket.State == WebSocketState.Open)
                    await SendMessageAsync(client.Socket, message);
            }
        }

        public abstract Task ReceiveAsync(WebSocket socket, WebSocketReceiveResult result, byte[] buffer);
    }
}