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
            RecursiveFunction();
        }

        private int downloadingTask = 0;
        private int downloadTaskIndex = 0;


        private void RecursiveFunction()
        {
            if (downloadTaskIndex < DownloadItems.Count)
            {
                if (downloadingTask < DownloadsInProgress)
                {
                    int current_downloadTaskIndex = downloadTaskIndex;
                    int current_downloadingTask = downloadingTask;

                    Task.Run(new Func<int>(() =>
                    {
                        Uri toDownload = DownloadItems[current_downloadTaskIndex].Uri;
                        string fileName = current_downloadTaskIndex.ToString();

                        Download(toDownload, fileName);

                        downloadTaskIndex++;
                        downloadingTask++;
                        return current_downloadTaskIndex;
                    })).ContinueWith(new Action<Task>((Task completedTask) =>
                    {
                        downloadingTask--;
                    }));
                }
                RecursiveFunction();
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
