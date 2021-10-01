using System.Threading.Tasks;

namespace SharpDownloader.Integration.Observer
{
    public interface ISubject{
        void Attach(IObserver observer);

        void Detach(IObserver observer);

        Task Notify();
    }
}