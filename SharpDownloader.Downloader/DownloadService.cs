using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using SharpDownloader.Integration.Observer;

namespace SharpDownloader.Downloader
{
    public class DownloadService : BackgroundService, ISubject
    {
        private List<DownloadBlock> _downloads;
        private List<IObserver> _observers;
        public DownloadService()
        {
            _downloads = new List<DownloadBlock>();
            _observers = new List<IObserver>();
        }

        

        

        public async Task <string> Create(string url, string path){
            var download = new DownloadBlock(this);
            await download.Create(url, path);

            _downloads.Add(download);
            return download.Id;
        }

        

        public Task End(DownloadBlock toEnd)
        {
            var download = _downloads.SingleOrDefault(c => c == toEnd);
            
            _downloads.Remove(download);
            
            return Task.CompletedTask;
        }

        public Task Get(string id)
        {
            throw new System.NotImplementedException();
        }

        public List<DownloadBlock> GetAll(string id)
        {
            return _downloads;
        }

        public Task Pause(string id)
        {
            var download = _downloads.SingleOrDefault(c => c.Id.Equals(id));
            download.Pause();
            return Task.CompletedTask;
        }

        public Task Resume(string id)
        {
             var download = _downloads.SingleOrDefault(c => c.Id.Equals(id));
            download.Resume();
            return Task.CompletedTask;
        }
        public Task Cancel(string id, bool deleteFile)
        {
            var download = _downloads.SingleOrDefault(c => c.Id.Equals(id));
            download.Cancel(deleteFile);
            _downloads.Remove(download);
            
            return Task.CompletedTask;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                foreach(var download in _downloads){
                    if(download.State == DownloadState.Sleeping)
                    {
                        new Thread(async () => 
                        {
                            Thread.CurrentThread.IsBackground = true; 
                            await NotifyNewDownload(download);
                            await download.StartDownloading();
                        }).Start();
                    }
                }
                await Task.Delay(5000, stoppingToken);
            }
        }

        public void Attach(IObserver observer)
        {
            _observers.Add(observer);
        }

        public void Detach(IObserver observer)
        {
            _observers.Remove(observer);
        }

        public async Task NotifyProgress(string id, double progress, string size, string remainingTime)
        {
            foreach(var observer in _observers){
                await observer.UpdateProgress(id, progress, size, remainingTime);
            }
        }

        public async Task NotifyNewDownload(object downloadBlock)
        {
            foreach(var observer in _observers){
                await observer.UpdateNewDownload(downloadBlock);
            }
        }
    }
}