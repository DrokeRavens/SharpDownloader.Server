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
using SharpDownloader.Downloader.Extensions;

namespace SharpDownloader.Downloader
{
    public class DownloadBlock
    {
        private DownloadService _downloadService;
        public string Id {get;set;}
        public string FileName {get; private set;}
        public long FileSize {get;private set;}
        public long CurrentSize {get;private set;}
        public string Path {get;private set;}
        public string Url {get;private set;}
        public int ChunkSize {get;private set;}
        public double Progress {get;private set;}
        public string RemainingTime {get;set;}
        private DateTime StartDate {get;set;}
        public DownloadState State {get; private set;}
        public string SizeProgress {get;private set;}

        public DownloadBlock(DownloadService downloadService)
        {
            _downloadService = downloadService;
        }

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
            StartDate = DateTime.Now;
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
                        while (true)
                        {
                            if(State == DownloadState.Paused)
                            {
                                await Task.Delay(1000);
                                continue;
                            }

                            if(State == DownloadState.Cancel)
                                break;

                            var buffer = new byte[ChunkSize];
                            var bytesRead = await responseStream.ReadAsync(buffer, 0, buffer.Length);
                            if (bytesRead == 0) break;
                            await fs.WriteAsync(buffer, 0, bytesRead);
                            CurrentSize += bytesRead;
                            Progress = Math.Round((CurrentSize * 100d)/ FileSize, 1);
                            RemainingTime = CalculateRemainingTime();
                            SizeProgress = CalculateSizeProgress();
                            NotifyProgress(this);

                        }
                        await fs.FlushAsync();
                        await _downloadService.End(this);
                    }
                }
            }
        }

        public void Pause(){
            State = DownloadState.Paused;
        }

        public void Resume(){
            State = DownloadState.Running;
        }

        public void Cancel(bool deleteFile){
            State = DownloadState.Cancel;

            if(deleteFile){
                if(File.Exists(Path))
                    File.Delete(Path);
            }
        }

        private string CalculateSizeProgress()
        {
            var currentStrSize = "";
            var fileStrSize = "";
            var sizeUnit = "";
            if(FileSize < 1000000000)
            {
                currentStrSize = CurrentSize.ToSize(SizeUnitExtensions.SizeUnits.MB);
                fileStrSize = FileSize.ToSize(SizeUnitExtensions.SizeUnits.MB);
                sizeUnit = "MB";
            }
            else if(FileSize >= 1000000000){
                currentStrSize = CurrentSize.ToSize(SizeUnitExtensions.SizeUnits.GB);
                fileStrSize = FileSize.ToSize(SizeUnitExtensions.SizeUnits.GB);
                sizeUnit = "GB";
            }
            return $"{currentStrSize} of {fileStrSize} {sizeUnit}";
        }

        

        private string CalculateRemainingTime()
        {
            var timeDiff = (DateTime.Now - StartDate).TotalMinutes * FileSize;
            var remainingTime = Math.Round(timeDiff / (double) CurrentSize, 2);

            var outputStr = remainingTime < 60 ? $"{remainingTime} minutes" : $"{remainingTime / 60} hours"; 
            return outputStr;
        }

        private async void NotifyProgress(DownloadBlock blockNode){
            await _downloadService.NotifyProgress(Id, Progress, SizeProgress, RemainingTime);
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