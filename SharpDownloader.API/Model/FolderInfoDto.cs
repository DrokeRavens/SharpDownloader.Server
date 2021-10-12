using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SharpDownloader.Downloader;

namespace SharpDownloader.API.Model
{
    public class FolderInfoDto {
        public string Name {get;set;}
        public string Path {get; set;}
    }
}