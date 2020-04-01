using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace XpathDataCrawler
{
    internal class DownloadData
    {
        public Uri Uri { get; set; }
        public string Path { get; set; }
        public NameValueCollection PostData { get; set; }
        public CookieCollection PostCookies { get; set; }
    }
}
