﻿using System.Collections.Generic;
using System.Runtime.Serialization;
using XpathDataCrawler.DataStructure.Exceptions;

namespace XpathDataCrawler.DataStructure
{
    [DataContract]
    public class Tree<T>
    {
        [DataMember]
        private TreeNode<T> Root;
        public T TreeRoot { get { return Root.Data; } }
        /// <summary>
        /// Adds a data to the tree
        /// </summary>
        /// <param name="data">The data that needs to be added</param>
        /// <param name="father">Father of the data that needs to be added</param>
        public void Add(T data, T father)
        {
            ///see if tree is empty
            if (Root == null)
                Root = new TreeNode<T>(data);
            else
            {
                ///while loop for all items
                var list = new List<TreeNode<T>>() { Root };
                while (list.Count > 0)
                {
                    ///search for the father 
                    foreach (var item in list)
                        if (item.Data.Equals(father))
                        {
                            item.Next.Add(new TreeNode<T>(data));
                            return;
                        }
                    /// save the next row as current row
                    var next = new List<TreeNode<T>>();
                    foreach (var item in list)
                        next.AddRange(item.Next);
                    list.Clear();
                    list.AddRange(next);
                }
                ///if is exits from while then the father is not found
                throw new ItemNotFoundException();
            }
        }
        /// <summary>
        /// Removes an element from tree
        /// </summary>
        /// <param name="data">The data that needs to be removed</param>
        public void Remove(T data)
        {
            /// see if tree is empty
            if (Root == null)
                throw new ItemNotFoundException();
            ///while loop for all items
            var list = new List<TreeNode<T>>() { Root };
            while (list.Count > 0)
            {
                ///search for the father 
                TreeNode<T> father = null, son = null;
                foreach (var item in list)
                    foreach (var item2 in item.Next)
                        if (item2.Data.Equals(data))
                        {
                            father = item;
                            son = item2;
                        }
                if (father != null)
                {
                    father.Next.Remove(son);
                    return;
                }
                /// save the next row as current row
                var next = new List<TreeNode<T>>();
                foreach (var item in list)
                    next.AddRange(item.Next);
                list.Clear();
                list.AddRange(next);
            }
            ///if is exits from while then the data is not found
            throw new ItemNotFoundException();
        }
        /// <summary>
        /// Get the last line of the tree. attention! it's not all item that doesn't have child.
        /// </summary>
        /// <returns>the last line of the tree</returns>
        public List<T> GetLastLine()
        {
            /// see if tree is empty
            if (Root == null)
                return null;
            /// itarate throgh all items
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
                /// save the next row as current row
                list.Clear();
                list.AddRange(next);
            }
            /// just for compiler warning. it must not exit while at all
            return null;
        }
        /// <summary>
        /// get all of items
        /// </summary>
        /// <returns>all data in tree</returns>
        public List<T> GetAll()
        {
            /// see if tree is empty
            if (Root == null)
                return null;
            /// itarate throgh all items
            var ret = new List<T>();
            var list = new List<TreeNode<T>>() { Root };
            while (list.Count > 0)
            {
                var next = new List<TreeNode<T>>();
                foreach (var item in list)
                    next.AddRange(item.Next);
                foreach (var item in list)
                    ret.Add(item.Data);
                /// save the next row as current row
                list.Clear();
                list.AddRange(next);
            }
            return ret;
        }
        /// <summary>
        /// get the child of a data
        /// </summary>
        /// <param name="father">Father of the data that needs to be found</param>
        /// <returns>child of input</returns>
        public List<T> GetChildren(T father)
        {
            /// see if tree is empty
            if (Root == null)
                throw new ItemNotFoundException();
            /// itarate throgh all items
            var list = new List<TreeNode<T>>() { Root };
            while (list.Count > 0)
            {
                ///search for the father 
                foreach (var item in list)
                    if (item.Data.Equals(father))
                    {
                        var res = new List<T>();
                        foreach (var item2 in item.Next)
                            res.Add(item2.Data);
                        return res;
                    }
                /// save the next row as current row
                var next = new List<TreeNode<T>>();
                foreach (var item in list)
                    next.AddRange(item.Next);
                list.Clear();
                list.AddRange(next);
            }
            ///if is exits from while then the father is not found
            throw new ItemNotFoundException();
        }
        /// <summary>
        /// Clear the tree
        /// </summary>
        public void Clear()
        {
            Root = null;
        }
        /// <summary>
        /// genarate a secondary tree with the same structure but with some of the data
        /// </summary>
        /// <param name="RootData">root of the new tree</param>
        /// <returns>the new tree</returns>
        public Tree<T> GetSubTree(T RootData)
        {
            /// see if tree is empty
            if (Root == null)
                throw new EmptyTreeExeption();
            else
            {
                ///while loop for all items
                var list = new List<TreeNode<T>>() { Root };
                while (list.Count > 0)
                {
                    ///search for the root of new tree 
                    foreach (var item in list)
                        if (item.Data.Equals(RootData))
                            return new Tree<T>() { Root = item.Copy() };

                    /// save the next row as current row
                    var next = new List<TreeNode<T>>();
                    foreach (var item in list)
                        next.AddRange(item.Next);
                    list.Clear();
                    list.AddRange(next);
                }
                ///if is exits from while then the root of new tree is not found
                throw new ItemNotFoundException();
            }
        }
    }
}
