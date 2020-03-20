using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace XpathDataCrawler
{
    public class DownloadManager
    {
        public enum DownloadStatus
        {
            Ready, InProgress, Done
        }

        private List<DownloadData> DownloadItems = new List<DownloadData>();

        public void Add_DownloadItem(DownloadData downloadData)
        {
            DownloadItems.Add(downloadData);
        }

        public int DownloadsInProgress { get; set; }

        public DownloadStatus Status { get; set; }

        public void Start()
        {
            foreach (DownloadData item in DownloadItems)
            {

            }
        }

        public void RecursiveFunction()
        {
            for (int i = 0; i < DownloadsInProgress; i++)
            {
                Uri googleUri = new Uri("https://www.google.com/");
                Task t = new Task(new Action(() => { Download(googleUri, i.ToString()); }));
            }
        }

        public void Download(Uri uri, string fileName)
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
