using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace SharpDownloader.API.Wss
{
    public class WsMessageHandler : WsHandler
    {
        public WsMessageHandler(ConnectionManager webSocketConnectionManager) : base(webSocketConnectionManager)
        {
        }

        public override async Task OnConnected(WebSocket socket)
        {
            await base.OnConnected(socket);

            var client = WebSocketConnectionManager.GetIdBySocket(socket);
            Console.WriteLine($"{client.Id} is now connected");
        }

        public override async Task ReceiveAsync(WebSocket socket, WebSocketReceiveResult result, byte[] buffer)
        {
            var client = WebSocketConnectionManager.GetIdBySocket(socket);
            var message = $"{client.Id} said: {Encoding.UTF8.GetString(buffer, 0, result.Count)}";
            Console.WriteLine(message);
        }
    }
}