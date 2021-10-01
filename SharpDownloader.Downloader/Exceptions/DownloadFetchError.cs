using System;

namespace SharpDownloader.Downloader.Exceptions
{
    public class DownloadFetchError : Exception
    {
        public DownloadFetchError() : base("Getting file info failed.")
        {
            
        }
    }
}