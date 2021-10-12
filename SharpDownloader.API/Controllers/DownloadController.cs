using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SharpDownloader.API.Model;
using SharpDownloader.Downloader;

namespace SharpDownloader.API.Controllers
{
    [ApiController]
    [Route("/download")]
    public class DownloadController : ControllerBase
    {
        DownloadService _downloadService;
        public DownloadController(DownloadService downloadService)
        {
            _downloadService = downloadService;
        }

        [HttpPost("/download/new")]
        public async Task<IActionResult> Create(DownloadCreateDto downloadData){
            var id = await _downloadService.Create(downloadData.Url, string.IsNullOrEmpty(downloadData.Path) ? Environment.CurrentDirectory : downloadData.Path);
            return Ok(new {
                Id = id
            });
        }

        [HttpPut("/download/pause/{id}")]
        public async Task<IActionResult> Pause(string id){
            await _downloadService.Pause(id);
            return Ok();
        }
        [HttpPut("/download/resume/{id}")]
        public async Task<IActionResult> Resume(string id){
            await _downloadService.Resume(id);
            return Ok();
        }

        [HttpDelete("/download/cancel/{id}/{deleteFile}")]
        public async Task<IActionResult> Cancel(string id, bool deleteFile){
            await _downloadService.Cancel(id, deleteFile);
            return Ok();
        }
    }
}