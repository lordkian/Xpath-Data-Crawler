using System;
using System.Runtime.Serialization;

namespace Library.DataStructure.Model
{
    [DataContract]
    internal abstract class ModelNode
    {
        [DataMember]
        public bool IsURLRelative { get; set; }
        [DataMember]
        public Guid Guid { get; set; }
        [DataMember]
        public string Xpath { get; set; }
    }
}
