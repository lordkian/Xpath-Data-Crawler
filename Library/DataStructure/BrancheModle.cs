using com.MovieAssistant.core.Exceptions;
using System;

namespace Library.DataStructure
{
    internal class BrancheModle : NodeModel
    {
        public NodeModel[] Next { get; set; }
        public string[] PostDataXpath { get; set; }
        public BrancheModle(int nextLength)
        {
            Next = new NodeModel[nextLength];
        }
        public void Append(NodeModel NodeModel)
        {
            var next = Next;
            Array.Resize(ref next, next.Length + 1);
            next[next.Length - 1] = NodeModel;
            Next = next;
        }
        public void AddToNext(NodeModel NodeModel)
        {
            for (int i = 0; i < Next.Length; i++)
            {
                if (Next[i] == null)
                {
                    Next[i] = NodeModel;
                    return;
                }
            }
            throw new ArrayIsFull();
        }
    }
}
