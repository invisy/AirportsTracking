using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Structures
{
    class PriorityQueue<Tkey, Tval> where Tkey : IComparable
    {
        MyList<KeyValuePair<Tkey, Tval>> heap;
        public PriorityQueue(int size = 3)
        {
            heap = new MyList<KeyValuePair<Tkey, Tval>>(3);
        }

        public void Enqueue(Tkey index , Tval value)
        {
            int curr = Count();
            heap.Add(new KeyValuePair<Tkey, Tval>(index, value));
            int parent = (curr - 1) / 2;

            while (curr > 0 && heap[curr].CompareTo(heap[parent]) < 0)
            {
                KeyValuePair<Tkey, Tval> temp = heap[parent];
                heap[parent] = heap[curr];
                heap[curr] = temp;
                curr = parent;
                parent = (curr - 1) / 2;
            }
        }

        public Tval Dequeue()
        {
            Tval result = heap[0].value;
            int last_index = Count()-1;
            KeyValuePair<Tkey, Tval> last_elem = heap[last_index];
            heap[0] = last_elem;
            heap.Remove(last_index);
            int curr = 0;
            int child;
            while (true)
            {
                if (curr * 2 + 2 < Count())
                {
                    if (heap[curr * 2 + 1].CompareTo(heap[curr * 2 + 2]) < 0)
                    {
                        if (heap[curr * 2 + 1].CompareTo(heap[curr]) < 0)
                            child = curr * 2 + 1;
                        else break;
                    }
                    else
                    {
                        if (heap[curr * 2 + 2].CompareTo(heap[curr]) < 0)
                            child = curr * 2 + 2;
                        else break;
                    }
                }
                else if (curr * 2 + 1 < Count())
                {
                    if (heap[curr * 2 + 1].CompareTo(heap[curr]) < 0)
                        child = curr * 2 + 1;
                    else break;
                }
                else break;

                KeyValuePair<Tkey, Tval> temp = heap[curr];
                heap[curr] = heap[child];
                heap[child] = temp;
                curr = child;
            }

            return result;
        }

        public Tval Peek()
        {
            if (Count() > 0)
            {
                return heap[0].value;
            }
            else
                return default(Tval);
        }

        public int Count()
        {
            return this.heap.Count();
        }
    }
}
