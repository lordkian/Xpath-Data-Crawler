using Library.Exceptions;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Library.DataStructure
{
    [DataContract]
    public class Tree<T>
    {
        [DataMember]
        private TreeNode<T> Root;
        public T TreeRoot { get { return Root.Data; } }
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
                            item.Next.Add(new TreeNode<T>(data));
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
        public void Remove(T data)
        {
            if (Root == null)
                throw new ItemNotFoundException();
            var list = new List<TreeNode<T>>() { Root };
            while (list.Count > 0)
            {
                foreach (var item in list)
                    foreach (var item2 in item.Next)
                        if (item2.Data.Equals(data))
                        {
                            item.Next.Remove(item2);
                        }

                var next = new List<TreeNode<T>>();
                foreach (var item in list)
                    next.AddRange(item.Next);
                list.Clear();
                list.AddRange(next);
            }
            throw new ItemNotFoundException();
        }
        public List<T> GetLastLine()
        {
            if (Root == null)
                return null;

            var list = new List<TreeNode<T>>() { Root };
            while (list.Count > 0)
            {
                var next = new List<TreeNode<T>>();
                foreach (var item in list)
                    next.AddRange(item.Next);
                if (next.Count == 0)
                {
                    var ret = new List<T>();
                    foreach (var item in list)
                        ret.Add(item.Data);
                    return ret;
                }
                list.Clear();
                list.AddRange(next);
            }
            return null;
        }
        public List<T> GetAll()
        {
            if (Root == null)
                return null;
            var ret = new List<T>();
            var list = new List<TreeNode<T>>() { Root };
            while (list.Count > 0)
            {
                var next = new List<TreeNode<T>>();
                foreach (var item in list)
                    next.AddRange(item.Next);
                foreach (var item in list)
                    ret.Add(item.Data);

                list.Clear();
                list.AddRange(next);
            }
            return ret;
        }
        public void Clear()
        {
            Root = null;
        }
    }
}
