using System;

namespace com.MovieAssistant.core.DataStructure
{
    public class ModelDataPackage
    {
        public string Name { get; set; }
        public Guid Guid { get; set; }
        public string Xpath { get; set; }
        public string Value { get; set; }
        public ModelDataPackage() { }
        internal ModelDataPackage(LeafModel leafModel)
        {
            Name = leafModel.Name;
            Guid = leafModel.Guid;
            Xpath = leafModel.Xpath;
        }
        internal ModelDataPackage(SubData subData) : this(subData.NodeModel as LeafModel)
        {
            Value = subData.Data;
        }
    }
}
