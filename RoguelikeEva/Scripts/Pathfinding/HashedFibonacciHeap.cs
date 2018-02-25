using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Vegricht.RoguelikeEva.Components;
using FibonacciHeap;

namespace Vegricht.RoguelikeEva.Pathfinding
{
    public class HashedFibonacciHeap<T, TKey> where TKey : IComparable<TKey>
    {
        FibonacciHeap<T, TKey> Heap;
        Dictionary<T, FibonacciHeapNode<T, TKey>> ObjectToHeapNodeMapping;
        
        public HashedFibonacciHeap(TKey minKeyValue)
        {
            Heap = new FibonacciHeap<T, TKey>(minKeyValue);
            ObjectToHeapNodeMapping = new Dictionary<T, FibonacciHeapNode<T, TKey>>();
        }
        
        public bool IsEmpty()
        {
            return Heap.IsEmpty();
        }
        
        public bool Contains(T data)
        {
            return ObjectToHeapNodeMapping.ContainsKey(data);
        }
        
        public void Insert(T data, TKey priority)
        {
            if (ObjectToHeapNodeMapping.ContainsKey(data))
                throw new ArgumentException("Fibonacci heap can't insert a node it already contains.");

            var node = new FibonacciHeapNode<T, TKey>(data, priority);
            Heap.Insert(node);
            ObjectToHeapNodeMapping.Add(data, node);
        }
        
        public void DecreasePriority(T data, TKey priority)
        {
            Heap.DecreaseKey(ObjectToHeapNodeMapping[data], priority);
        }
        
        public T RemoveMin()
        {
            FibonacciHeapNode<T, TKey> popped = Heap.RemoveMin();
            ObjectToHeapNodeMapping.Remove(popped.Data);

            return popped.Data;
        }
    }
}