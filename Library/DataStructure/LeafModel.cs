namespace com.MovieAssistant.core.DataStructure
{
    internal class LeafModel : NodeModel
    {
        public string Content { get; set; }
        public bool IsUniqe { get; set; }
        public string Name { get; set; }
        public LeafType Type { get; set; }
    }
}
