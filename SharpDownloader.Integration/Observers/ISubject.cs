using System.Threading.Tasks;

namespace SharpDownloader.Integration.Observer
{
    public interface ISubject{
        void Attach(IObserver observer);

        void Detach(IObserver observer);

        Task NotifyProgress(string id, double progress, string size, string remainingTime);
    }
}