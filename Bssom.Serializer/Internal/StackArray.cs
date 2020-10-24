//using System.Runtime.CompilerServices;

using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Bssom.Serializer.Internal
{
    internal unsafe struct StackArray<T> : Iterable<T> where T : unmanaged
    {
        private T* _ptr;
        private int _len;

        public StackArray(T* ptr, int len)
        {
            _ptr = ptr;
            _len = len;
        }

        public Iterable<T> ToISegment()
        {
            return this;
        }

        public ref T GetFirstElementReference(out bool isContiguousMemoryArea)
        {
            isContiguousMemoryArea = true;
            return ref Unsafe.AsRef<T>(_ptr);
        }

        public IEnumerable<T> Ts
        {
            get
            {
                for (int i = 0; i < _len; i++)
                {
                    yield return this[i];
                }
            }
        }

        public int Length => _len;

        public T this[int i] { get=> _ptr[i];  set => _ptr[i] = value; } 
    }
}
