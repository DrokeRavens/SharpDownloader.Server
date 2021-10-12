using System.Threading.Tasks;

namespace SharpDownloader.Integration.Observer
{
    public interface IObserver{
        Task UpdateProgress(string id, double progress, string size, string remainingTime);
        Task UpdateNewDownload(object downloadBlock);
    }
}