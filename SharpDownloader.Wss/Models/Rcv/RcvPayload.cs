using System.Net.WebSockets;
using System.Threading.Tasks;
using SharpDownloader.Wss.Enum;

namespace SharpDownloader.Wss.Models.Snd
{
    public class RcvPayload
    {
        public RcvCommands PacketType {get;set;}
        public object PacketData {get;set;}
        public string Id {get;set;}
    }
}