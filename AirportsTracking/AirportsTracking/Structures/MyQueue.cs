using System;

namespace Structures
{
    class Node<T>
    {
        public T data;
        public Node<T> next;
    }

    class MyQueue<T>
    {
        Node<T> tail = null;
        Node<T> head = null;
        int count = 0;

        public T Dequeue()
        {
            T data = tail.data;
            tail = tail.next;
            count--;
            return data;
        }
        public void Enqueue(T data)
        {
            Node<T> new_elem = new Node<T>();
            new_elem.data = data;
            new_elem.next = head;
            head = new_elem;

            if (count == 0)
                tail = new_elem;
            count++;
        }
        public T Peek()
        {
            if (count == 0)
                return default(T);
            else
                return tail.data;
        }
    }
}
