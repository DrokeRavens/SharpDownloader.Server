using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Threading.Tasks;
using SharpDownloader.Downloader.Exceptions;
using SharpDownloader.Integration.Observer;

namespace SharpDownloader.Downloader
{
    public class DownloadBlock : ISubject
    {
        private DownloadService _downloadService;
        private List<IObserver> _observers = new List<IObserver>();
        public string Id {get;set;}
        public string FileName {get; private set;}
        public long FileSize {get;private set;}
        public long CurrentSize {get;private set;}
        public string Path {get;private set;}
        public string Url {get;private set;}
        public int ChunkSize {get;private set;}
        public double Progress {get;private set;}
        public DownloadState State {get; private set;}

        public DownloadBlock(DownloadService downloadService)
        {
            _downloadService = downloadService;
        }
        public void Attach(IObserver observer)
        {
            _observers.Add(observer);
        }
        public void Detach(IObserver observer)
        {
            _observers.Remove(observer);

        }
        public async Task Notify()
        {
            foreach(var observer in _observers)
                await observer.Update(this);
        }
        //This is wrong... need to find a better way and refactor. Creating new task during a controller request may be dumb
        public async Task<string> Create(string url, string destination)
        {
            Url = url;
            Path = destination;
            GenerateId();
            await SetInitialInfo();
            State = DownloadState.Sleeping;
            return Id;
        }

        public async Task StartDownloading(){
            State = DownloadState.Running;
            var request = (HttpWebRequest)WebRequest.Create(Url);
            request.Method = "GET";
            request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)";
            using (var response = await request.GetResponseAsync())
            {
                using (var responseStream = response.GetResponseStream())
                {
                    using (var fs = new FileStream(Path , FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                    {
                        while (State != DownloadState.Paused)
                        {
                            var buffer = new byte[ChunkSize];
                            var bytesRead = await responseStream.ReadAsync(buffer, 0, buffer.Length);
                            if (bytesRead == 0) break;
                            await fs.WriteAsync(buffer, 0, bytesRead);
                            CurrentSize += bytesRead;
                            Progress = Math.Round((CurrentSize * 100d)/ FileSize, 1);
                            await Notify();
                        }
                        await fs.FlushAsync();
                        await _downloadService.End(this);
                    }
                }
            }
        }

        private async Task SetInitialInfo(){

            var request = (HttpWebRequest)WebRequest.Create(Url);
            request.Method = "HEAD";


            using (var response = await request.GetResponseAsync())
            {
                FileSize = response.ContentLength;
                Progress = 0;
                FileName = response.Headers.AllKeys.Contains("Content-Disposition") ? 
                response.Headers["Content-Disposition"].Replace("attachment; filename=", String.Empty).Replace("\"", String.Empty) : Url.Split('/').LastOrDefault();
                ChunkSize = 1000000; // 10mb
                Path = $"{Path}/{FileName}";
                Url = Url;
            }
        }

        private void GenerateId(){
            Id = Guid.NewGuid().ToString();
        }

        
    }
}