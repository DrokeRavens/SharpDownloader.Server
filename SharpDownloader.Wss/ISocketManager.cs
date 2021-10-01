using System.Net.WebSockets;
using System.Threading.Tasks;
using SharpDownloader.Wss.Enum;
using SharpDownloader.Wss.Models;
using SharpDownloader.Wss.Models.Snd;

namespace SharpDownloader.Wss
{
    public interface ISocketManager
    {
        WssClient Add(WebSocket socket);
        Task Remove(WebSocket socket);
        Task Disconnect(string id);
        Task SendMessage(string id, object message, SndPayload type);
        Task ProcessMessage(RcvPayload message);
    }
}