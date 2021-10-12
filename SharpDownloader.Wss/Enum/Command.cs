using System.Net.WebSockets;
using System.Threading.Tasks;

namespace SharpDownloader.Wss.Enum
{
    public enum Command
    {
        DownloadList = 1,
        DownloadProgress = 2,
        Identity = 3,
        NewDownload = 4
    }
}