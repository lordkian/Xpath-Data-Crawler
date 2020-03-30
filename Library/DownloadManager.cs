using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
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
                    Task.Run(() => { Download(downloadData.Uri, downloadData.Path); });
                }
            }
        }

        private void Download(Uri uri, string fileName)
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(uri);
            HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();

            Stream fileStream = File.Open(fileName, FileMode.Create);
            Stream responseStream = httpWebResponse.GetResponseStream();
            responseStream.CopyTo(fileStream);

            fileStream.Close();
            responseStream.Close();
            httpWebResponse.Close();
        }
    }
}
