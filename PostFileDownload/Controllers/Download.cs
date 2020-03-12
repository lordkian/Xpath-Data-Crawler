using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace PostFileDownload.Controllers
{
    public class Download : Controller
    {
        public IActionResult DownloadFile()
        {
            return View();
        }
        [HttpPost]
        public IActionResult DownloadFile(string fileName)
        {
            if (fileName == null)
                return DownloadFile();
            Stream stream = System.IO.File.Open($"wwwroot/test.zip", FileMode.Open);
            return new FileStreamResult(stream, "application/octet-stream") ;
        }
    }
}
