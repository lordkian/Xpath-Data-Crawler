using System;
using System.Collections.Generic;
using Library.DataStructure;

namespace ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Tree<int> tree = new Tree<int>();
            tree.Add(0, 0);
            tree.Add(1, 0);
            tree.Add(2, 0);
            tree.Add(3, 2);
            tree.Add(4, 2);
            var l1 = tree.GetLastLine();
            var l2 = tree.GetAll();
        }
    }
}
