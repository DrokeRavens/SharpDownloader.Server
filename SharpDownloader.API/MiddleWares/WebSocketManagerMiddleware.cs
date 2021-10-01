using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SharpDownloader.Wss;

namespace SharpDownloader.API.MiddleWares
{
    public class WebSocketManagerMiddleware
{
    private readonly RequestDelegate _next;
    private ISocketHandler _webSocketHandler { get; set; }

    public WebSocketManagerMiddleware(RequestDelegate next,
                                        ISocketHandler webSocketHandler)
    {
        _next = next;
        _webSocketHandler = webSocketHandler;
    }

    public async Task Invoke(HttpContext context)
    {
        if(!context.WebSockets.IsWebSocketRequest)
            return;

        var socket = await context.WebSockets.AcceptWebSocketAsync();
        await _webSocketHandler.OnConnected(socket);

        await Receive(socket, async(result, buffer) =>
        {
            if(result.MessageType == WebSocketMessageType.Text)
            {
                string msg = Encoding.UTF8.GetString(buffer);
                await _webSocketHandler.OnMessage(socket, msg);
                return;
            }

            else if(result.MessageType == WebSocketMessageType.Close)
            {
                await _webSocketHandler.OnDisconnect(socket);
                return;
            }

        });
    }

    private async Task Receive(WebSocket socket, Action<WebSocketReceiveResult, byte[]> handleMessage)
    {
        var buffer = new byte[1024 * 4];

        while(socket.State == WebSocketState.Open)
        {
            var result = await socket.ReceiveAsync(buffer: new ArraySegment<byte>(buffer),
                                                    cancellationToken: CancellationToken.None);

            handleMessage(result, buffer);
        }
    }
}
}