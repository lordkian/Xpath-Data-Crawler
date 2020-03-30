using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Net.Mime;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace XpathDataCrawler
{
    public class DownloadManager
    {
        private List<DownloadData> downloadItems = new List<DownloadData>();
        private Task task;
        private ManualResetEvent manualResetEvent = new ManualResetEvent(false);
        private object downloadItemsLock = new object();
        private bool downloading = false;
        public static readonly string DirSprator;
        static DownloadManager()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                DirSprator = "\\";
            else
                DirSprator = "/";
        }
        public DownloadManager()
        {
            task = Task.Run(DoWork);
        }
        public void AddDownloadItem(string path, string url, NameValueCollection postData = null, CookieCollection postCookies = null)
        {
            lock (downloadItemsLock)
            {
                downloadItems.Add(new DownloadData() { Path = path, Uri = new Uri(url), PostData = postData, PostCookies = postCookies });
            }
            if (downloading)
                manualResetEvent.Set();
        }

        public int DownloadsInProgress { get; set; }

        public void Start()
        {
            manualResetEvent.Set();
            downloading = true;
        }
        public void stop()
        {
            manualResetEvent.Reset();
            downloading = false;
        }
        private void DoWork()
        {
            int c;
            while (true)
            {
                manualResetEvent.WaitOne();
                lock (downloadItemsLock)
                {
                    c = downloadItems.Count;
                }
                if (c == 0)
                    stop();
                else
                {
                    DownloadData downloadData;
                    lock (downloadItemsLock)
                    {
                        downloadData = downloadItems[0];
                        downloadItems.Remove(downloadData);
                    }
                    Task.Run(() => { Download(downloadData); });
                }
            }
        }

        private void Download(DownloadData downloadData)
        {//https://stackoverflow.com/questions/4015324/how-to-make-http-post-web-request
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(downloadData.Uri);
            HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            string filename = new ContentDisposition(httpWebResponse.Headers["content-disposition"]).FileName;

            Stream fileStream = File.Open(downloadData.Path + DirSprator + filename, FileMode.Create);
            Stream responseStream = httpWebResponse.GetResponseStream();
            responseStream.CopyTo(fileStream);

            fileStream.Close();
            responseStream.Close();
            httpWebResponse.Close();
        }
    }
}
