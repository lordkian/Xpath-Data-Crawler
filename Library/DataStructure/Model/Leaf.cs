using System.Runtime.Serialization;

namespace XpathDataCrawler.DataStructure.Model
{
    [DataContract]
    internal class Leaf : ModelNode
    {
        [DataMember]
        public bool IsUniqe { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public LeafType Type { get; set; }
    }
}
