using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Net.Http;

namespace PostFileDownload.Controllers
{
    public class Download : Controller
    {
        public IActionResult DownloadFile()
        {
            return View();
        }
        [HttpPost]
        public IActionResult DownloadFile(HttpRequestMessage request)
        {
            Stream stream = System.IO.File.Open($"wwwroot/test.zip", FileMode.Open);
            return new FileStreamResult(stream, "application/octet-stream") { FileDownloadName = "test.zip" };
        }
    }
}
