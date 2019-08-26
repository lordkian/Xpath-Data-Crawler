using System;

namespace com.MovieAssistant.core.DataStructure
{
    internal class NodeModel
    {
        public bool IsURLRelative { get; set; }
        public Guid Guid { get; set; }
        public string Xpath { get; set; }
    }
}
