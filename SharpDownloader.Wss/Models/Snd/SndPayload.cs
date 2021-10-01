using System.Net.WebSockets;
using System.Threading.Tasks;
using SharpDownloader.Wss.Enum;

namespace SharpDownloader.Wss.Models.Snd
{
    public class SndPayload
    {
        public SndCommands PacketType {get;set;}
        public object PacketData {get;set;}
    }
}