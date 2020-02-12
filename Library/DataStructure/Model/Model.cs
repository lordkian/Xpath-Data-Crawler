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

        internal readonly Dictionary<string, ModelNode> XpathToModelNode = new Dictionary<string, ModelNode>();
        internal readonly Dictionary<Guid, ModelNode> GuidToModelNode = new Dictionary<Guid, ModelNode>();
    }
}
