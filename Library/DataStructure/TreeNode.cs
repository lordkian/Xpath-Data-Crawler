using System.Collections.Generic;
using System.Runtime.Serialization;

namespace XpathDataCrawler.DataStructure
{
    [DataContract]
    internal class TreeNode<T>
    {
        [DataMember]
        internal T Data;
        [DataMember]
        internal List<TreeNode<T>> Next = new List<TreeNode<T>>();
        public TreeNode(T data)
        {
            Data = data;
        }
        public override string ToString()
        {
            return Data.ToString();
        }
        /// <summary>
        /// Generate a copy of this instance TreeNode of and return it.
        /// </summary>
        /// <returns>a copy of this instance TreeNode </returns>
        public TreeNode<T> Copy()
        {
            var t = new TreeNode<T>(Data);
            foreach (var item in Next)
                t.Next.Add(item.Copy());
            return t;
        }
    }
}
