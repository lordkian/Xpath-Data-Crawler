using System;
using System.Runtime.Serialization;

namespace XpathDataCrawler.DataStructure.Model
{
    [DataContract]
    internal abstract class ModelNode
    {
        [DataMember]
        internal bool IsURLRelative { get; set; }
        [DataMember]
        internal Guid Id { get; set; }
        [DataMember]
        internal string Xpath { get; set; }
        [DataMember]
        internal Method URLGrabMethode;
    }
}
