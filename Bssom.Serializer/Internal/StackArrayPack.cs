//using System.Runtime.CompilerServices;

using System.Collections.Generic;

namespace Bssom.Serializer.Internal
{
    internal unsafe struct StackArrayPack<T> : Iterable<T> where T : unmanaged
    {
        private StackArray<T> _stackArray;
        private int _nextPos;

        public StackArrayPack(T* ptr, int len)
        {
            _stackArray = new StackArray<T>(ptr,len);
            _nextPos = 0;
        }

        public Iterable<T> ToUlongs()
        {
            return this;
        }

        public IEnumerable<T> Ts
        {
            get
            {
                for (int i = 0; i < _nextPos; i++)
                {
                    yield return this[i];
                }
            }
        }

        public int Length => _nextPos;

        public T this[int i] { get => _stackArray[i]; set => _stackArray[i] = value; }

        public void RemoveLast()
        {
            _nextPos--;
        }

        public void Add(T val)
        {
            this[_nextPos] = val;
            _nextPos++;
        }

        public ref T GetFirstElementReference(out bool isContiguousMemoryArea)
        {
            return ref _stackArray.GetFirstElementReference(out isContiguousMemoryArea);
        }
    }
}
