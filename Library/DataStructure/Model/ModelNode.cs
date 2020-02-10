using System;

namespace Library.DataStructure.Model
{
    internal abstract class ModelNode
    {
        public bool IsURLRelative { get; set; }
        public Guid Guid { get; set; }
        public string Xpath { get; set; }
    }
}
