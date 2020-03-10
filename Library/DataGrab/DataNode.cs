using System.Collections.Generic;
using XpathDataCrawler.DataStructure.Model;

namespace XpathDataCrawler.DataStructure.DataGrab
{
    internal class DataNode
    {
        internal readonly List<ModelNode> ModelNodes = new List<ModelNode>();
        internal readonly List<string> Datas = new List<string>();
    }
}
