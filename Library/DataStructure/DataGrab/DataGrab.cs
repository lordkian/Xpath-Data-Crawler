using System;
using System.Collections.Generic;
using System.Text;
using Library.DataStructure.Model;

namespace Library.DataStructure.DataGrab
{
    public class DataGrab
    {
        readonly Model.Model model;
        DataNode root;
        public DataGrab(Model.Model model)
        {
            this.model = model;
            root = new DataNode();
            root.ModelNodes.Add(model.Root);
        }

    }
}
