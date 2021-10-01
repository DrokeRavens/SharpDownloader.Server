using System.Net.WebSockets;
using System.Threading.Tasks;

namespace SharpDownloader.Wss.Enum
{
    public enum RcvCommands
    {
        DownloadList = 1,
        DownloadProgress = 2
    }
}