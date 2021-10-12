using System.Net.WebSockets;
using System.Threading.Tasks;
using SharpDownloader.Wss.Enum;

namespace SharpDownloader.Wss.Models.Snd
{
    public class RcvPayload : PayloadHeader
    {
        public object PacketData {get;set;}
        public string Id {get;set;}
    }
}