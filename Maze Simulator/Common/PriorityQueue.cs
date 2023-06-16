using System;
using System.Collections.Generic;

namespace Maze_Simulator.Common
{
    public class PriorityQueue<T>
    {
        private readonly List<Node> queue = new();

        private int heapSize = -1;

        private readonly bool isMinPriorityQueue;

        public PriorityQueue(bool isMinPriorityQueue)
        {
            this.isMinPriorityQueue = isMinPriorityQueue;
        }

        public int Count => queue.Count;

        public void Enqueue(T obj, int priority)
        {
            Node node = new()
            {
                Priority = priority,
                Value = obj
            };
            queue.Add(node);
            heapSize++;

            if (isMinPriorityQueue)
            {
                BuildHeapMin(heapSize);
            }
            else
            {
                BuildHeapMax(heapSize);
            }
        }

        public T Dequeue()
        {
            if (heapSize > -1)
            {
                var returnVal = queue[0].Value;
                queue[0] = queue[heapSize];
                queue.RemoveAt(heapSize);
                heapSize--;

                if (isMinPriorityQueue)
                {
                    MinHeapify(0);
                }
                else
                {
                    MaxHeapify(0);
                }

                return returnVal;
            }
            else
            {
                throw new Exception("Queue is empty");
            }
        }

        public T Peek()
        {
            return heapSize > -1 ? queue[0].Value : throw new Exception("Queue is empty");
        }

        public void UpdatePriority(T obj, int priority)
        {
            for (int i = 0; i <= heapSize; i++)
            {
                Node node = queue[i];
                if (ReferenceEquals(node.Value, obj))
                {
                    node.Priority = priority;
                    if (isMinPriorityQueue)
                    {
                        BuildHeapMin(i);
                        MinHeapify(i);
                    }
                    else
                    {
                        BuildHeapMax(i);
                        MaxHeapify(i);
                    }
                }
            }
        }

        public bool Contains(T obj)
        {
            foreach (Node node in queue)
            {
                if (ReferenceEquals(node.Value, obj))
                {
                    return true;
                }
            }

            return false;
        }

        private void BuildHeapMax(int i)
        {
            while (i >= 0 && queue[(i - 1) / 2].Priority < queue[i].Priority)
            {
                Swap(i, (i - 1) / 2);
                i = (i - 1) / 2;
            }
        }

        private void BuildHeapMin(int i)
        {
            while (i >= 0 && queue[(i - 1) / 2].Priority > queue[i].Priority)
            {
                Swap(i, (i - 1) / 2);
                i = (i - 1) / 2;
            }
        }

        private void MaxHeapify(int i)
        {
            int left = ChildL(i);
            int right = ChildR(i);

            int heighst = i;

            if (left <= heapSize && queue[heighst].Priority < queue[left].Priority)
            {
                heighst = left;
            }

            if (right <= heapSize && queue[heighst].Priority < queue[right].Priority)
            {
                heighst = right;
            }

            if (heighst != i)
            {
                Swap(heighst, i);
                MaxHeapify(heighst);
            }
        }

        private void MinHeapify(int i)
        {
            int left = ChildL(i);
            int right = ChildR(i);

            int lowest = i;

            if (left <= heapSize && queue[lowest].Priority > queue[left].Priority)
            {
                lowest = left;
            }

            if (right <= heapSize && queue[lowest].Priority > queue[right].Priority)
            {
                lowest = right;
            }

            if (lowest != i)
            {
                Swap(lowest, i);
                MinHeapify(lowest);
            }
        }

        private void Swap(int i, int j)
        {
            var temp = queue[i];
            queue[i] = queue[j];
            queue[j] = temp;
        }

        private int ChildL(int i)
        {
            return (i * 2) + 1;
        }

        private int ChildR(int i)
        {
            return (i * 2) + 2;
        }

        private class Node
        {
            public int Priority { get; set; }

            public T Value { get; set; }
        }
    }
}
