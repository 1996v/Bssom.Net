//using System.Runtime.CompilerServices;

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Bssom.Serializer.Internal
{
    internal readonly struct Raw64BytesISegment : IMapKeySegment
    {
        private readonly byte[] _us;
        private readonly int _len;
        private readonly int _raw64Count;
        private readonly int _lastRaw64ByteCount;

        public Raw64BytesISegment(byte[] us) : this(us, us.Length)
        {

        }

        public Raw64BytesISegment(byte[] us, int len)
        {
            DEBUG.Assert(len > 0);
            _us = us;
            _len = len;
            _raw64Count = (int)Math.Ceiling((decimal)len / 8);
            _lastRaw64ByteCount = 8 - (_raw64Count * 8 - _len);
        }

        public unsafe ulong this[int i]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (i != _raw64Count - 1 || _lastRaw64ByteCount == 8)
                {
                    return Unsafe.ReadUnaligned<ulong>(ref _us[i * 8]);
                }

                int startPos = (_raw64Count - 1) * 8;
                ulong value1 = 0;
                Unsafe.CopyBlock(ref Unsafe.As<ulong, byte>(ref value1), ref _us[startPos], (uint)_lastRaw64ByteCount);
                return value1;
            }
        }

        public int Length => _raw64Count;

        public int LastValueByteCount => _lastRaw64ByteCount;

        public int DataLen => _len;

        public IEnumerable<ulong> Ts
        {
            get
            {
                for (int i = 0; i < _raw64Count; i++)
                {
                    yield return this[i];
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref ulong GetFirstElementReference(out bool isContiguousMemoryArea)
        {
            isContiguousMemoryArea = true;
            return ref Unsafe.As<byte, ulong>(ref _us[0]);
        }

        internal int Len => _len;
        internal int Raw64Count => _raw64Count;
    }

}
