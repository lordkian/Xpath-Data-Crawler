using Library.DataStructure.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Library.DataStructure.DataGrab
{
    internal class DataNode
    {
        internal ModelNode Father;
        internal readonly List<ModelNode> ModelNodes = new List<ModelNode>();
        internal readonly List<string> Datas = new List<string>();
    }
}
