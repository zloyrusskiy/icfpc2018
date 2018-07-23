using System;

namespace TraceOptimizer.Utils
{
    public class DSU
    {
        private readonly int[] _parent;
        private readonly int[] _size;

        public DSU(int capacity)
        {
            _parent = new int[capacity];
            _size = new int[capacity];

            Array.Fill(_size, 1);
            _size[0] = 2;

            for (var i = 0; i < capacity; i++)
            {
                _parent[i] = i;
            }
        }

        public int FindSet(int v)
        {
            if (v == _parent[v]) return v;
            return _parent[v] = FindSet(_parent[v]);
        }

        public int SizeOf(int v) => _size[v];

        public bool Union(int a, int b)
        {
            a = FindSet(a);
            b = FindSet(b);
            if (a != b)
            {
                if (_size[a] < _size[b])
                {
                    var t = a;
                    a = b;
                    b = t;
                }
                _parent[b] = a;
                _size[a] += _size[b];
            }

            return false;
        }
    }
}