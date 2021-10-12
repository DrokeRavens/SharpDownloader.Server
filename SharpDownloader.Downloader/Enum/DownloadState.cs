namespace SharpDownloader.Downloader
{
    public enum DownloadState
    {
        Running = 0,
        Paused = 1,
        Error = 2,
        Finished = 3,
        Sleeping = 4,
        Cancel = 5
    }
}