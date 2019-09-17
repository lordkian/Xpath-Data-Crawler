using System.Collections.Generic;

namespace com.MovieAssistant.core.DataStructure
{
    internal class NodePackage
    {
        public Node[] SubDatas { get; set; }
        public List<NodePackage> NextSubDataPackage { get; set; } = new List<NodePackage>();
    }
}
