using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SharpDownloader.Downloader;

namespace SharpDownloader.API.Model
{
    public class DownloadCreateDto {
        public string Url {get;set;}
        public string Path {get; set;}
    }
}