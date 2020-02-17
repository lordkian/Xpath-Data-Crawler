using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace Library.DataStructure.DataGrab
{
    public class DataGrab
    {
        readonly Model.Model model;
        readonly Tree<DataNode> tree = new Tree<DataNode>();
        List<string> filterXpaths = new List<string>();
        List<Guid> filterIds = new List<Guid>();
        public Action<Guid, string, string[]> onFilter { get; set; }
        public Action<DataGrab> onFinish { get; set; }
        public DataGrab(Model.Model model)
        {
            this.model = model;
            var root = new DataNode();
            root.ModelNodes.Add(model.Root);
            tree.Add(root, null);
        }
        public void SetFilter(params Guid[] guids)
        {
            filterIds.Clear();
            filterIds.AddRange(guids);
        }
        public void SetFilter(params string[] xpathes)
        {
            filterXpaths.Clear();
            filterXpaths.AddRange(xpathes);
        }
        public void Start()
        {

        }
        private void GrabData(DataNode dataNode)
        { }
        private static List<List<string>> LoadData(string URL, NameValueCollection data, params string[] xpathes)
        {
            if (URL == null || URL.Length == 0)
                throw new Exception("URL Cannot be null or empty");
            var client = new WebClient();
            client.Encoding = Encoding.UTF8;
            var res = client.UploadValues(URL, "post", data);
            // var HTML = client.DownloadString(URL);
            return LoadDataFromHTML(Encoding.UTF8.GetString(res), xpathes);
        }
        private static List<List<string>> LoadData(string URL, params string[] xpathes)
        {
            if (URL == null || URL.Length == 0)
                throw new Exception("URL Cannot be null or empty");
            var client = new WebClient();
            client.Encoding = Encoding.UTF8;
            return LoadDataFromHTML(client.DownloadString(URL), xpathes);
        }
        private static List<List<string>> LoadDataFromHTML(string HTML, params string[] xPathes)
        {
            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(HTML);
            var firstDraft = new List<List<string>>();
            int rowsCount = xPathes.Length;
            HtmlNodeCollection nodes = null;
            var regex = new Regex(@"/@\w+$");

            for (int i = 0; i < rowsCount; i++)
            {
                var list = new List<string>();
                var m = regex.Match(xPathes[i]);
                nodes = doc.DocumentNode.SelectNodes(xPathes[i]);
                if (nodes == null)
                    list.Add("");
                else
                {
                    if (!m.Success)
                        foreach (var node in nodes)
                            list.Add(node.InnerText.Trim());

                    else
                    {
                        string attribute = m.Value.Substring(2);
                        foreach (var node in nodes)
                            list.Add(node.GetAttributeValue(attribute, "Attribute not found !"));
                    }
                }
                firstDraft.Add(list);
            }

            //Transpose
            var grouped = new List<List<string>>();
            var len = firstDraft.First().Count;
            for (int i = 0; i < len; i++)
            {
                var list = new List<string>();
                foreach (var item in firstDraft)
                    if (item.Count > 0)
                    {
                        list.Add(item.First());
                        item.RemoveAt(0);
                    }
                    else
                        list.Add("");
                grouped.Add(list);
            }
            return grouped;
        }
    }
}
