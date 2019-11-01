using Library.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Library.DataStructure
{
    public class Tree<T>
    {
        private TreeNode<T> Root;
        public void Add(T data, T Father)
        {
            if (Root == null)
                Root = new TreeNode<T>(data);
            else
            {
                var list = new List<TreeNode<T>>() { Root };
                while (list.Count > 0)
                {
                    foreach (var item in list)
                        if (item.Data.Equals(Father))
                        {
                            item.Next.Add(new TreeNode<T>(data) { Father = item });
                            return;
                        }

                    var next = new List<TreeNode<T>>();
                    foreach (var item in list)
                        next.AddRange(item.Next);
                    list.Clear();
                    list.AddRange(next);
                }
                throw new ItemNotFoundException();
            }
        }
    }
    internal class TreeNode<T>
    {
        internal T Data;
        internal TreeNode<T> Father;
        internal List<TreeNode<T>> Next = new List<TreeNode<T>>();
        public TreeNode(T data)
        {
            Data = data;
        }
    }
}
