using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using XpathDataCrawler.DataStructure.Model;
using XpathDataCrawler.DataStructure;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace XpathDataCrawler.DataGrab
{
    public class DataGrab
    {
        readonly Model model;
        readonly Tree<DataNode> tree = new Tree<DataNode>();
        readonly string keyword;
        List<string> filterXpaths = new List<string>();
        List<Guid> filterIds = new List<Guid>();
        Dictionary<string, List<DataNode>> filterXpathsDic = new Dictionary<string, List<DataNode>>();
        Dictionary<Guid, List<DataNode>> filterIdsDic = new Dictionary<Guid, List<DataNode>>();
        bool FilterOn = false;
        DownloadManager downloadManager = new DownloadManager();
        public Action<Guid, string, string[]> onFilter { get; set; }
        public Action<DataGrab> onFinish { get; set; }

        public DataGrab(Model model, string keyword)
        {
            this.model = model;
            this.keyword = keyword;
        }
        public void Filter(Guid id, bool keep, params string[] datas)
        {
            var modelnode = model.GuidToModelNode[id];
            var all = tree.GetAll().Where(d => d.ModelNodes.Contains(modelnode)).ToList();

            int index = all[0].ModelNodes.IndexOf(modelnode);
            foreach (var item in all)
            {
                if (datas.Contains(item.Datas[index]) ^ keep)
                    RemoveDataNode(item);
            }
            filterIds.Remove(id);
            if (filterIdsDic.ContainsKey(id))
                filterIdsDic.Remove(id);
            FilterOff();
        }
        public void Filter(string xpath, bool keep, params string[] datas)
        {
            var modelnode = model.XpathToModelNode[xpath];
            var all = tree.GetAll().Where(d => d.ModelNodes.Contains(modelnode)).ToList();

            int index = all[0].ModelNodes.IndexOf(modelnode);
            foreach (var item in all)
            {
                if (datas.Contains(item.Datas[index]) ^ keep)
                    RemoveDataNode(item);
            }
            filterXpaths.Remove(xpath);
            if (filterXpathsDic.ContainsKey(xpath))
                filterXpathsDic.Remove(xpath);
            FilterOff();
        }
        private void RemoveDataNode(DataNode dataNode)
        {
            tree.Remove(dataNode);
            if (list.Contains(dataNode))
                list.Remove(dataNode);
            if (list2.Contains(dataNode))
                list2.Remove(dataNode);
        }
        private void FilterOff()
        {
            foreach (var item in filterIds)
                if (filterIdsDic[item].Count > 0)
                    return;
            foreach (var item in filterXpaths)
                if (filterXpathsDic[item].Count > 0)
                    return;
            FilterOn = false;
        }
        public void SetFilter(params Guid[] guids)
        {
            filterIds.Clear();
            filterIds.AddRange(guids);
            filterIdsDic.Clear();
            foreach (var item in guids)
                filterIdsDic.Add(item, new List<DataNode>());
        }
        public void SetFilter(params string[] xpathes)
        {
            filterXpaths.Clear();
            filterXpaths.AddRange(xpathes);
            filterXpathsDic.Clear();
            foreach (var item in xpathes)
                filterXpathsDic.Add(item, new List<DataNode>());
        }
        public void Download(string path)
        {
            var tasks = new List<Task>();
            var modelnodes = model.GetDownloadableNodes();
            foreach (var item in tree.GetAll())
            {
                var mn = item.ModelNodes.Intersect(modelnodes).ToList();
                if (mn.Count() > 0)
                    foreach (var item2 in mn)
                    {
                        int index = mn.IndexOf(item2);
                        tasks.Add(Task.Run(() => { MethodProcessDownload(item2.URLGrabMethode, item.Datas[index], path); }));
                    }
            }
            tasks.ForEach((t) => { t.Wait(); });
            downloadManager.Start();
        }
        List<DataNode> list = new List<DataNode>();
        List<DataNode> list2 = new List<DataNode>();
        public void Start()
        {
            var root = RootGrabData();
            list.Add(root);
            Continue();
        }
        public void Continue()
        {
            if (FilterOn)
                return;

            while (list.Count > 0)
            {
                foreach (var item in list)
                {
                    foreach (var item2 in item.ModelNodes)
                    {
                        if (filterXpaths.Contains(item2.Xpath))
                        {
                            FilterOn = true;
                            filterXpathsDic[item2.Xpath].Add(item);
                        }
                        if (filterIds.Contains(item2.Id))
                        {
                            FilterOn = true;
                            filterIdsDic[item2.Id].Add(item);
                        }
                    }
                }

                if (FilterOn)
                {
                    StratFilterAction();
                    return;
                }
                var tasks = new List<Task>();
                foreach (var item in list)
                    tasks.Add(Task.Run(() =>
                    {
                        var res = GrabData(item);
                        lock (list2)
                        {
                            list2.AddRange(res);
                        }
                    }));
                tasks.ForEach((t) => { t.Wait(); });

                list.Clear();
                list.AddRange(list2);
                list2.Clear();
            }
            onFinish(this);
        }
        private void StratFilterAction()
        {
            if (!FilterOn)
                return;

            foreach (var item in filterXpaths)
                if (filterXpathsDic[item].Count > 0)
                {
                    var guid = model.XpathToModelNode[item].Id;
                    int i = filterXpathsDic[item][0].ModelNodes.IndexOf(model.XpathToModelNode[item]);
                    var datas = new List<string>();
                    foreach (var item2 in filterXpathsDic[item])
                        datas.Add(item2.Datas[i]);
                    onFilter(guid, item, datas.ToArray());
                    return;
                }
            foreach (var item in filterIds)
                if (filterIdsDic[item].Count > 0)
                {
                    var xpath = model.GuidToModelNode[item].Xpath;
                    int i = filterIdsDic[item][0].ModelNodes.IndexOf(model.GuidToModelNode[item]);
                    var datas = new List<string>();
                    foreach (var item2 in filterIdsDic[item])
                        datas.Add(item2.Datas[i]);
                    onFilter(item, xpath, datas.ToArray());
                    return;
                }
        }
        private DataNode RootGrabData()
        {
            var dn = new DataNode();
            dn.Datas.Add(keyword);
            dn.ModelNodes.Add(model.Root);
            tree.Add(dn, null);
            return dn;
        }
        private List<DataNode> GrabData(DataNode dataNode)
        {
            var res = new List<DataNode>();

            for (int i = 0; i < dataNode.ModelNodes.Count; i++)
            {
                if (dataNode.ModelNodes[i] is Leaf)
                    continue;

                var html = MethodProcess((dataNode.ModelNodes[i] as Branche).URLGrabMethode, dataNode.Datas[i]);
                var children = model.GetChildren(dataNode.ModelNodes[i]);
                var xpaths = new List<string>();
                foreach (var item in children)
                    xpaths.Add(item.Xpath);
                var res2 = LoadDataFromHTML(html, xpaths.ToArray());
                var dns = ParsData(res2, children);
                res.AddRange(dns);
                foreach (var item in dns)
                    tree.Add(item, dataNode);
            }

            return res;
        }
        private List<DataNode> ParsData(List<List<string>> data, List<ModelNode> children)
        {
            var res = new List<DataNode>();
            foreach (var item in data)
            {
                var dn = new DataNode();
                dn.Datas.AddRange(item);
                dn.ModelNodes.AddRange(children);
                res.Add(dn);
            }
            return res;
        }
        private string MethodProcess(Method method, string xpathResult)
        {
            if (method == null)
                return LoadData(model.BaseURL + "/" + xpathResult);
            var url = BuildString(method.URL, xpathResult);
            var c = method.Keys.Count;
            if (c == 0)
                return LoadData(url);
            var nvc = new NameValueCollection();
            for (int i = 0; i < c; i++)
                nvc.Add(BuildString(method.Keys[i], xpathResult), BuildString(method.Values[i], xpathResult));
            return LoadData(url, nvc);
        }
        private void MethodProcessDownload(Method method, string xpathResult, string path)
        {
            if (method == null)
            {
                downloadManager.AddDownloadItem(path, model.BaseURL + "/" + xpathResult);
                return;
            }
            var url = BuildString(method.URL, xpathResult);
            var c = method.Keys.Count;
            if (c == 0)
            {
                downloadManager.AddDownloadItem(path, url);
                return;
            }
            var nvc = new NameValueCollection();
            for (int i = 0; i < c; i++)
                nvc.Add(BuildString(method.Keys[i], xpathResult), BuildString(method.Values[i], xpathResult));
            downloadManager.AddDownloadItem(path, url, nvc);
        }
        private string BuildString(List<string> list, string xpathResult)
        {
            var res = "";
            foreach (var item in list)
            {
                var value = item.Remove(0, 3);
                switch (item.Substring(0, 3))
                {
                    case "str":
                        res += value;
                        break;
                    case "xpa":
                        res += xpathResult;
                        break;
                    case "bur":
                        res += model.BaseURL;
                        break;
                    default:
                        break;
                }
            }
            return res;
        }
        public static string LoadData(string URL, NameValueCollection data)
        {
            if (URL == null || URL.Length == 0)
                throw new Exception("URL Cannot be null or empty");
            var client = new WebClient();
            client.Encoding = Encoding.UTF8;
            var res = client.UploadValues(URL, "post", data);
            return Encoding.UTF8.GetString(res);
        }
        public static string LoadData(string URL)
        {
            if (URL == null || URL.Length == 0)
                throw new Exception("URL Cannot be null or empty");
            var client = new WebClient();
            client.Encoding = Encoding.UTF8;
            return client.DownloadString(URL);
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
