using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SharpDownloader.Downloader;

namespace SharpDownloader.API.Controllers
{
    [Route("/download")]
    public class TesteController : Controller
    {
        DownloadService _downloadService;
        public TesteController(DownloadService downloadService)
        {
            _downloadService = downloadService;
        }
        [HttpPost]
        public async Task<IActionResult> Download(string url){
            await _downloadService.Create(url, Environment.CurrentDirectory);
            return Ok();
        }
    }
}