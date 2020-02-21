using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Library.DataStructure.Model
{
    public enum LeafType { Downloadable, Data, FinalData }
    public enum SaveType { XML, JSON, Binary };
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
        public Guid SetRoot(string xpath, Method grabMethode = null, bool isURLRelative = true)
        {
            var guid = Guid.NewGuid();
            Tree.Clear();
            var br = new Branche() { GrabMethode = grabMethode, Id = guid, Xpath = xpath, IsURLRelative = isURLRelative };
            Tree.Add(br, null);
            XpathToModelNode.Add(xpath, br);
            GuidToModelNode.Add(guid, br);
            return guid;
        }
        public Guid AddXpath(Guid fatherGuid, string xpath, int childNumber, Method grabMethode = null, bool isURLRelative = true)
        {
            var guid = Guid.NewGuid();
            var br = new Branche() { GrabMethode = grabMethode, Id = guid, Xpath = xpath, IsURLRelative = isURLRelative };
            Tree.Add(br, GuidToModelNode[fatherGuid]);
            XpathToModelNode.Add(xpath, br);
            GuidToModelNode.Add(guid, br);
            return guid;
        }
        public Guid AddXpath(string fatherXpath, string xpath, int childNumber, Method grabMethode = null, bool isURLRelative = true)
        {
            var guid = Guid.NewGuid();
            var br = new Branche() { GrabMethode = grabMethode, Id = guid, Xpath = xpath, IsURLRelative = isURLRelative };
            Tree.Add(br, XpathToModelNode[fatherXpath]);
            XpathToModelNode.Add(xpath, br);
            GuidToModelNode.Add(guid, br);
            return guid;
        }
        public Guid AddItem(Guid fatherGuid, string xpath, string name, LeafType type, bool isUnique, bool isURLRelative = true)
        {
            var guid = Guid.NewGuid();
            var l = new Leaf() { Id = guid, Xpath = xpath, IsURLRelative = isURLRelative, IsUniqe = isUnique, Type = type, Name = name };
            Tree.Add(l, GuidToModelNode[fatherGuid]);
            XpathToModelNode.Add(xpath, l);
            GuidToModelNode.Add(guid, l);
            return guid;
        }
        public Guid AddItem(string fatherXpath, string xpath, string name, LeafType type, bool isUnique, bool isURLRelative = true)
        {
            var guid = Guid.NewGuid();
            var l = new Leaf() { Id = guid, Xpath = xpath, IsURLRelative = isURLRelative, IsUniqe = isUnique, Type = type, Name = name };
            Tree.Add(l, XpathToModelNode[fatherXpath]);
            XpathToModelNode.Add(xpath, l);
            GuidToModelNode.Add(guid, l);
            return guid;
        }
    }
}
