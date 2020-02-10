using System.Runtime.Serialization;

namespace Library.DataStructure.Model
{
    [DataContract]
    public class Leaf
    {
        [DataMember]
        public bool IsUniqe { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public LeafType Type { get; set; }
    }
}
