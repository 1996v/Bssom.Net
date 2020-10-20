//using System.Runtime.CompilerServices;

using System.Collections;
using System.Collections.Generic;

namespace BssomSerializers.Internal
{
    internal struct ArrayPack<T> : IEnumerable<T>
    {
        private T[] _array;
        private int _nextPos;

        public int NextPos => _nextPos;

        public T this[int item] { get => _array[item];set => _array[item] = value; }

        public ArrayPack(T[] array)
        {
            _array = array;
            _nextPos = 0;
        }

        public ArrayPack(int size)
        {
            _array = new T[size];
            _nextPos = 0;
        }

        public void Add(T t)
        {
            _array[_nextPos] = t;
            _nextPos++;
        }

        public void RollbackOne()
        {
            _nextPos--;
        }

        public T[] GetArray() => _array;

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < _nextPos; i++)
            {
                yield return _array[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
