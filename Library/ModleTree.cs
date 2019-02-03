using System;
using System.Collections.Generic;
using System.Text;

namespace com.MovieAssistant.core
{
    public class ModleTree
    {
        private ModleNode root;
        public string SearchEng;
        public string SiteNmae { get; set; }
        public string BaseURL { get; set; }

        public ModleTree() : this(0) { }
        public ModleTree(int firstNodeLength)
        {
            root = new BrancheModle(firstNodeLength);
        }
        public void SaveJSON(string path)
        {
            throw new NotImplementedException();
        }
        public void SaveXML(string path)
        {
            throw new NotImplementedException();
        }
        public void Load(string path)
        {
            throw new NotImplementedException();
        }
    }
    public abstract class ModleNode
    {
        private string xpath;

        public string Xpath
        {
            get { return xpath; }
            set { xpath = value; }
        }
    }
    public class BrancheModle : ModleNode
    {
        private ModleNode[] next;

        public ModleNode[] Next
        {
            get { return next; }
        }

        public BrancheModle() : this(0) { }
        public BrancheModle(int nextLength)
        {
            next = new ModleNode[nextLength];
        }
        public void Append(ModleNode modleNode)
        {
            Array.Resize(ref next, next.Length + 1);
            next[next.Length - 1] = modleNode;
        }
    }
    public class LeafModle : ModleNode
    {
        public LeafType Type { get; set; }
    }
    public enum LeafType { downloadable, data }
}
