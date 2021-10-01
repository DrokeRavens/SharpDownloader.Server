using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using SharpDownloader.Integration.Observer;

namespace SharpDownloader.Downloader
{
    public class DownloadService : BackgroundService
    {
        private List<DownloadBlock> _downloads;
        public DownloadService()
        {
            _downloads = new List<DownloadBlock>();
        }

        

        public Task Cancel(string id)
        {
            throw new System.NotImplementedException();
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
            throw new System.NotImplementedException();
        }

        public Task Resume(string id)
        {
            throw new System.NotImplementedException();
        }

        public void AttachBridge(IObserver observer){
            foreach(var download in _downloads)
                download.Attach(observer);
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
                            await download.StartDownloading();
                        }).Start();
                    }
                }
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}