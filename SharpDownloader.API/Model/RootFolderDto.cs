using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SharpDownloader.Downloader;

namespace SharpDownloader.API.Model
{
    public class RootFolderDto {
        public IEnumerable<FolderInfoDto> Folders {get;set;}
        public string CurrentFolder { get; set; }
    }
}