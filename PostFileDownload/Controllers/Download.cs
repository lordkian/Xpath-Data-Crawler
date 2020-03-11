﻿using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace PostFileDownload.Controllers
{
    public class Download : Controller
    {

        [HttpPost]
        public IActionResult DownloadFile([FromBody] string fileName)
        {
            if (fileName == null)
            {
                return null;
            }
            else
            {
                Stream stream = System.IO.File.Open($"wwwroot/{fileName}", FileMode.Open);
                return new FileStreamResult(stream, "application/octet-stream") { FileDownloadName = fileName };
            }

        }
    }
}
