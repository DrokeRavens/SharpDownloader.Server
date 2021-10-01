using System.Threading.Tasks;

namespace SharpDownloader.Integration.Observer
{
    public interface IObserver{
        Task Update(ISubject subject);
    }
}