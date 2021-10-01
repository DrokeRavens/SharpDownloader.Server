using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace SharpDownloader.Downloader
{
    public interface IsDownloadService{
        Task<string> Create(string url, string path);
        Task Pause(string id);
        Task Resume(string id);
        Task Cancel(string id);
        Task Get(string id);
        List<DownloadBlock> GetAll(string id);
        Task End(DownloadBlock toEnd);
    }
    
}