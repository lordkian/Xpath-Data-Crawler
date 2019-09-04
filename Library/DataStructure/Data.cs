﻿using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace com.MovieAssistant.core.DataStructure
{
    public class Data
    {
        List<SubDataPackage> Current = new List<SubDataPackage>();
        private List<LeafModel> Filters = new List<LeafModel>();
        private string SearchKey;
        private LeafModel TmpFilterKey = null;
        private int TmpIndex = -1;
        bool IsFilterd = false;
        public Model Model { get; set; }
        internal SubDataPackage Root { get; set; }
        public Action<string[]> onFilter { get; set; }
        public Action onFinish { get; set; }
        public Data(Model model, string searchKey)
        {
            Model = model;
            Root = new SubDataPackage() { SubDatas = new SubData[] { new SubData() { NodeModel = model.Root } } };
            Root.SubDatas[0].Data = string.Format(model.SearchEng, searchKey);
            SearchKey = searchKey;
        }
        public void Start(bool newThread = true)
        {
            if (newThread)
            {
                new Thread(() => { Start(false); }).Start();
                return;
            }
            Current.Add(Root);
            Continue();
        }
        public void Continue()
        {
            while (Current.Count > 0)
            {
                if (!IsFilterd)
                    foreach (var item in Filters)
                        if ((from i in Current[0].SubDatas select i.NodeModel).Contains(item))
                        {
                            TmpFilterKey = item;
                            var sd = (from i in Current[0].SubDatas where i.NodeModel == item select i).First();
                            TmpIndex = Array.IndexOf(Current[0].SubDatas, sd);
                            onFilter((from i in Current select i.SubDatas[TmpIndex].Data).Distinct().ToArray());
                            return;
                        }
                var next = new List<SubDataPackage>();
                foreach (var item in Current)
                {
                    foreach (var item2 in (from i in item.SubDatas where i.NodeModel is BrancheModle select i))
                    {
                        var bm = item2.NodeModel as BrancheModle;
                        NameValueCollection nameValue = null;

                        if (bm.PostDataXpath != null && bm.PostDataXpath.Length != 0)
                            nameValue = GenerateNameValue(bm.PostDataXpath);

                        var xpathes = (from i in bm.Next orderby Array.IndexOf(bm.Next, i) select i.Xpath).ToArray();//orderby for security
                        List<List<string>> res;
                        if (nameValue == null)
                            res = LoadData(item2.Data, xpathes);
                        else
                            res = LoadData(item2.Data, nameValue, xpathes);

                        foreach (var item3 in res)
                        {
                            var subDataPackage = new SubDataPackage() { SubDatas = new SubData[bm.Next.Length] };
                            for (int i = 0; i < bm.Next.Length; i++)
                                subDataPackage.SubDatas[i] = new SubData() { Data = item3[i], NodeModel = bm.Next[i] };
                            for (int i = 0; i < bm.Next.Length; i++)
                                if (subDataPackage.SubDatas[i].NodeModel is BrancheModle && subDataPackage.SubDatas[i].NodeModel.IsURLRelative)
                                    subDataPackage.SubDatas[i].Data = Model.BaseURL + subDataPackage.SubDatas[i].Data;

                            item.NextSubDataPackage.Add(subDataPackage);
                            next.Add(subDataPackage);
                        }
                    }
                }
                Current.Clear();
                Current.AddRange(next);
                IsFilterd = false;
            }
            onFinish();
        }
        public void SetFilter(params string[] xpathes)
        {
            foreach (var item in xpathes)
            {
                NodeModel nodeModel;
                if (Model.NodeModelToXpath.TryGetBySecond(item, out nodeModel) && nodeModel is LeafModel)
                    Filters.Add(nodeModel as LeafModel);
            }
        }
        public void SetFilter(params Guid[] guids)
        {
            foreach (var item in guids)
            {
                NodeModel nodeModel;
                if (Model.NodeModelToGuid.TryGetBySecond(item, out nodeModel) && nodeModel is LeafModel)
                    Filters.Add(nodeModel as LeafModel);
            }
        }
        public void Filter(params string[] data)
        {
            if (TmpFilterKey == null || TmpIndex == -1)
                return; //exeption
            var cp = Current.ToArray().ToList();
            foreach (var item in cp)
            {
                if (!data.Contains(item.SubDatas[TmpIndex].Data))
                    Current.Remove(item);
            }
            TmpFilterKey = null;
            TmpIndex = -1;
            IsFilterd = true;
        }
        public List<NameValueCollection> ToList(ValueType outputType)
        {
            var MainList = new List<List<SubDataPackage>>() { new List<SubDataPackage>() { Root } };
            var newList = new List<List<SubDataPackage>>();
            while (true)
            {
                newList.Clear();
                foreach (var item in MainList)
                {
                    var sdp = item.Last();
                    if (sdp.NextSubDataPackage != null && sdp.NextSubDataPackage.Count != 0)
                        foreach (var item2 in sdp.NextSubDataPackage)
                        {
                            var tmp = new List<SubDataPackage>();
                            tmp.AddRange(item);
                            tmp.Add(item2);
                            newList.Add(tmp);
                        }
                }
                if (newList.Count == 0)
                    break;
                MainList.Clear();
                MainList.AddRange(newList);
            }
            Func<SubData, string> func;
            switch (outputType)
            {
                case ValueType.Guid:
                    func = sd => { return sd.NodeModel.Guid.ToString(); };
                    break;
                case ValueType.Xpath:
                    func = sd => { return sd.NodeModel.Xpath; };
                    break;
                case ValueType.Name:
                    func = sd => { return (sd.NodeModel as LeafModel).Name; };
                    break;
                default:
                    func = sd => { return sd.NodeModel.Xpath; };
                    break;
            }
            var res = new List<NameValueCollection>();
            foreach (var item in MainList)
            {
                var allSD = new List<SubData>();
                foreach (var item2 in item)
                    allSD.AddRange((from i in item2.SubDatas where i.NodeModel is LeafModel select i));
                var nvc = new NameValueCollection();
                foreach (var item2 in allSD)
                    nvc.Add(func(item2), item2.Data);
                res.Add(nvc);
            }
            return res;
        }
        private NameValueCollection GenerateNameValue(string[] data)
        {
            var nameValue = new NameValueCollection();

            for (int i = 0; i < data.Length; i++)
                data[i] = data[i].Replace("@SearchWord", SearchKey);
            for (int i = 0; i < data.Length / 2; i++)
                nameValue.Add(data[i], data[i + 1]);

            return nameValue;
        }
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
    internal class SubData
    {
        public NodeModel NodeModel { get; set; }
        public string Data { get; set; }
    }
    internal class SubDataPackage
    {
        public SubData[] SubDatas { get; set; }
        public List<SubDataPackage> NextSubDataPackage { get; set; } = new List<SubDataPackage>();
    }
    public enum ValueType
    {
        Guid, Xpath, Name
    }
}