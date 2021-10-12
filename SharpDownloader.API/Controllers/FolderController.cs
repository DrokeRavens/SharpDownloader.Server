using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SharpDownloader.API.Model;
using SharpDownloader.Downloader;

namespace SharpDownloader.API.Controllers
{
    [ApiController]
    [Route("/folder")]
    public class FolderController : ControllerBase {
        public FolderController()
        {

        }
        [HttpGet("/folder/{folderRoot?}")]
        public RootFolderDto Folders(string folderRoot){
            
            if(string.IsNullOrEmpty(folderRoot)){
                return new RootFolderDto{
                    Folders  = Directory
                        .EnumerateDirectories(Environment.CurrentDirectory)
                        .Select(f => new FolderInfoDto{
                            Name = new DirectoryInfo(f).Name,
                            Path = f,
                        }),
                    CurrentFolder = Environment.CurrentDirectory
                } ;
            }
            else{
                return new RootFolderDto{
                    Folders = Directory
                        .EnumerateDirectories(folderRoot)
                        .Select(f => new FolderInfoDto{
                            Name = new DirectoryInfo(f).Name,
                            Path = f,
                        }),
                    CurrentFolder = folderRoot
                };
            }
        }

        [HttpGet("/folder/parent/{folderRoot?}")]
        public RootFolderDto ParentFolder(string folderRoot){
            var path = Path.Combine(folderRoot, "..");
            return new RootFolderDto{
                    Folders = Directory
                        .EnumerateDirectories(path)
                        .Select(f => new FolderInfoDto{
                            Name = new DirectoryInfo(f).Name,
                            Path = f
                        }),
                    CurrentFolder = path
                };
        }
    }
}