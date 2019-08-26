using com.MovieAssistant.core;
using com.MovieAssistant.core.Exceptions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;

namespace Library.DataStructure
{
    public enum SaveType { XML, JSON, Binary };
    [DataContract]
    public class Model
    {
        [DataMember]
        BrancheModle Root;
        [DataMember]
        public string SiteNmae { get; set; }
        [DataMember]
        public string BaseURL { get; set; }
        public string SearchEng { get; set; }
        public Guid RootGuid { get { return Root.Guid; } }
        public string[] RooPostDataXpath
        {
            get { return Root.PostDataXpath; }
            set { Root.PostDataXpath = value; }
        }
        internal readonly BiDictionary<NodeModel, string> NodeModelToXpath = new BiDictionary<NodeModel, string>();
        internal readonly BiDictionary<NodeModel, Guid> NodeModelToGuid = new BiDictionary<NodeModel, Guid>();
        public void SetRoot(int ChildNumber)
        {
            Root = new BrancheModle(ChildNumber) { Xpath = "", Guid = Guid.NewGuid() };
            NodeModelToXpath.Add(Root, Root.Xpath);
            NodeModelToGuid.Add(Root, Root.Guid);
        }
        public void AddItem(Guid fatherGuid, string xpath, string name, LeafType type, bool isUnique, bool isURLRelative = true)
        {
            NodeModel father;
            if (!NodeModelToGuid.TryGetBySecond(fatherGuid, out father))
                throw new GuidNotFoundException();
            if (father is LeafModel)
                throw new BadGuidException();
            var lm = new LeafModel() { Guid = Guid.NewGuid(), Xpath = xpath, Name = name, Type = type, IsUniqe = isUnique, IsURLRelative = isURLRelative };
            (father as BrancheModle).AddToNext(lm);
            NodeModelToXpath.Add(lm, lm.Xpath);
            NodeModelToGuid.Add(lm, lm.Guid);
        }
        public void AddItem(string fatherXpath, string xpath, string name, LeafType type, bool isUnique, bool isURLRelative = true)
        {
            NodeModel father;
            if (!NodeModelToXpath.TryGetBySecond(fatherXpath, out father))
                throw new XpathNotFoundException();
            if (father is LeafModel)
                throw new BadXpathException();
            var lm = new LeafModel() { Xpath = xpath, Type = type, Guid = Guid.NewGuid(), IsUniqe = isUnique, Name = name, IsURLRelative = isURLRelative };
            (father as BrancheModle).AddToNext(lm);
            NodeModelToXpath.Add(lm, lm.Xpath);
            NodeModelToGuid.Add(lm, lm.Guid);
        }
        public void AddXpath(Guid fatherGuid, string xpath, int childNumber, bool isURLRelative = true, string[] postDataXpath = null)
        {
            NodeModel father;
            if (!NodeModelToGuid.TryGetBySecond(fatherGuid, out father))
                throw new GuidNotFoundException();
            if (father is LeafModel)
                throw new BadGuidException();
            var bm = new BrancheModle(childNumber) { Guid = Guid.NewGuid(), Xpath = xpath, IsURLRelative = isURLRelative, PostDataXpath = postDataXpath };
            (father as BrancheModle).AddToNext(bm);
            NodeModelToXpath.Add(bm, bm.Xpath);
            NodeModelToGuid.Add(bm, bm.Guid);
        }
        public void AddXpath(string fatherXpath, string xpath, int childNumber, bool isURLRelative = true, string[] postDataXpath = null)
        {
            NodeModel father;
            if (!NodeModelToXpath.TryGetBySecond(fatherXpath, out father))
                throw new XpathNotFoundException();
            if (father is LeafModel)
                throw new BadXpathException();
            var bm = new BrancheModle(childNumber) { Guid = Guid.NewGuid(), Xpath = xpath, IsURLRelative = isURLRelative, PostDataXpath = postDataXpath };
            (father as BrancheModle).AddToNext(bm);
            NodeModelToXpath.Add(bm, bm.Xpath);
            NodeModelToGuid.Add(bm, bm.Guid);
        }
        public void Save(string path, SaveType saveType)
        {
            switch (saveType)
            {
                case SaveType.XML:
                    break;
                case SaveType.JSON:
                    var sw = new StreamWriter(path);
                    sw.WriteLine(JsonConvert.SerializeObject(this, Formatting.Indented));
                    sw.Close();
                    break;
                case SaveType.Binary:
                    break;
            }
        }
        public static Model Load(string path)
        {
            var sr = new StreamReader(path);
            var res = JsonConvert.DeserializeObject<Model>(sr.ReadToEnd());
            res.NodeModelToXpath.Add(res.Root, res.Root.Xpath);
            res.NodeModelToGuid.Add(res.Root, res.Root.Guid);
            return res;
        }
    }
    public enum LeafType { Downloadable, Data, FinalData }
}
