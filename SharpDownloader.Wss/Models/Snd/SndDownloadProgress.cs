using System.Net.WebSockets;
using System.Threading.Tasks;
using SharpDownloader.Wss.Enum;

namespace SharpDownloader.Wss.Models.Snd
{
    public class SndDownloadProgress : PayloadHeader
    {
        public string Id {get;set;}
        public double Progress {get;set;}
        public string SizeProgress { get; set; }
        public string RemainingTime { get; set; }
    }
}