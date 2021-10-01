using System.Net.WebSockets;
using System.Threading.Tasks;

namespace SharpDownloader.Wss
{
    public interface ISocketHandler
    {
        Task OnConnected(WebSocket socket);
        Task OnMessage(WebSocket socket, string message);
        Task OnDisconnect(WebSocket socket);
    }
}