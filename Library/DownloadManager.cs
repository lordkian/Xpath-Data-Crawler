using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

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


        private string ToQueryString(this NameValueCollection collection)
        {
            var array = (from key in collection.AllKeys
                         from value in collection.GetValues(key)
                         select string.Format("{0}={1}", HttpUtility.UrlEncode(key), HttpUtility.UrlEncode(value))).ToArray();
            return string.Join("&", array);
        }

        private void Download(DownloadData downloadData)
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(downloadData.Uri);
            HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            string filename = new ContentDisposition(httpWebResponse.Headers["content-disposition"]).FileName;
            Stream responseStream = httpWebResponse.GetResponseStream();

            if (downloadData.PostData != null)
            {
                byte[] postData = Encoding.ASCII.GetBytes(ToQueryString(downloadData.PostData));
                httpWebRequest.Method = "POST";
                httpWebRequest.ContentType = "application/x-www-form-urlencoded";
                httpWebRequest.ContentLength = postData.Length;
                responseStream.Write(postData, 0, postData.Length);
            }

            Stream fileStream = File.Open(downloadData.Path + DirSprator + filename, FileMode.Create);
            responseStream.CopyTo(fileStream);
            responseStream.CopyTo(fileStream);

            fileStream.Close();
            responseStream.Close();
            httpWebResponse.Close();
        }
    }
}
