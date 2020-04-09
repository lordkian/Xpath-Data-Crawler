using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;

namespace XpathDataCrawler.DataStructure.Model
{
    public enum LeafType { Downloadable, Data, FinalData }
    public enum SaveType { XML, JSON, Binary, CleanJSON };
    [DataContract]
    public class Model
    {
        [DataMember]
        private Tree<ModelNode> Tree = new Tree<ModelNode>();
        [DataMember]
        public string SiteNmae { get; set; }
        [DataMember]
        public string BaseURL { get; set; }
        internal Branche Root { get { return Tree.TreeRoot as Branche; } }
        internal readonly Dictionary<string, ModelNode> XpathToModelNode = new Dictionary<string, ModelNode>();
        internal readonly Dictionary<Guid, ModelNode> GuidToModelNode = new Dictionary<Guid, ModelNode>();
        internal List<ModelNode> GetChildren(ModelNode modelNode)
        { return Tree.GetChildren(modelNode); }
        internal List<ModelNode> GetDownloadableNodes()
        {
            var res = new List<ModelNode>();
            foreach (var item in Tree.GetAll())
                if (item is Leaf && (item as Leaf).Type == LeafType.Downloadable)
                    res.Add(item);
            return res;
        }
        public Guid SetRoot(Method grabMethode, bool isURLRelative = true)
        {
            var guid = Guid.NewGuid();
            Tree.Clear();
            var br = new Branche() { URLGrabMethode = grabMethode, Id = guid, Xpath = " ", IsURLRelative = isURLRelative };
            Tree.Add(br, null);
            XpathToModelNode.Add(" ", br);
            GuidToModelNode.Add(guid, br);
            return guid;
        }
        public Guid AddXpath(Guid fatherGuid, string xpath, Method URLGrabMethode = null, bool isURLRelative = true)
        {
            var guid = Guid.NewGuid();
            var br = new Branche() { URLGrabMethode = URLGrabMethode, Id = guid, Xpath = xpath, IsURLRelative = isURLRelative };
            Tree.Add(br, GuidToModelNode[fatherGuid]);
            XpathToModelNode.Add(xpath, br);
            GuidToModelNode.Add(guid, br);
            return guid;
        }
        public Guid AddXpath(string fatherXpath, string xpath, Method URLGrabMethode = null, bool isURLRelative = true)
        {
            var guid = Guid.NewGuid();
            var br = new Branche() { URLGrabMethode = URLGrabMethode, Id = guid, Xpath = xpath, IsURLRelative = isURLRelative };
            Tree.Add(br, XpathToModelNode[fatherXpath]);
            XpathToModelNode.Add(xpath, br);
            GuidToModelNode.Add(guid, br);
            return guid;
        }
        public Guid AddItem(Guid fatherGuid, string xpath, string name, LeafType type, bool isUnique, Method URLGrabMethode = null, bool isURLRelative = true)
        {
            var guid = Guid.NewGuid();
            var l = new Leaf() { Id = guid, Xpath = xpath, IsURLRelative = isURLRelative, IsUniqe = isUnique, Type = type, Name = name, URLGrabMethode = URLGrabMethode };
            Tree.Add(l, GuidToModelNode[fatherGuid]);
            XpathToModelNode.Add(xpath, l);
            GuidToModelNode.Add(guid, l);
            return guid;
        }
        public Guid AddItem(string fatherXpath, string xpath, string name, LeafType type, bool isUnique, Method URLGrabMethode = null, bool isURLRelative = true)
        {
            var guid = Guid.NewGuid();
            var l = new Leaf() { Id = guid, Xpath = xpath, IsURLRelative = isURLRelative, IsUniqe = isUnique, Type = type, Name = name, URLGrabMethode = URLGrabMethode };
            Tree.Add(l, XpathToModelNode[fatherXpath]);
            XpathToModelNode.Add(xpath, l);
            GuidToModelNode.Add(guid, l);
            return guid;
        }
        public static void Save(string fileName, Model model)
        {
            var sw = new StreamWriter(fileName);
            sw.Write(JsonConvert.SerializeObject(model, Formatting.Indented));
            sw.Close();
        }
        public static Model Load(string fileName)
        {
            var sr = new StreamReader(fileName);
            Model model = JsonConvert.DeserializeObject<Model>(sr.ReadToEnd());
            sr.Close();
            foreach (ModelNode item in model.Tree.GetAll())
            {
                model.XpathToModelNode.Add(item.Xpath, item);
                model.GuidToModelNode.Add(item.Id, item);
            }
            return model;
        }
    }
}
