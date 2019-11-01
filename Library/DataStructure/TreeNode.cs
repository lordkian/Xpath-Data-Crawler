using System.Collections.Generic;

namespace Library.DataStructure
{
    internal class TreeNode<T>
    {
        internal T Data;
        internal TreeNode<T> Father;
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
