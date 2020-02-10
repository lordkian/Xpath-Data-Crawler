using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Library.DataStructure
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
    }
}
