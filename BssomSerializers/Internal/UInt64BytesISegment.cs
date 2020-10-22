//using System.Runtime.CompilerServices;

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Bssom.Serializer.Binary;
using Bssom.Serializer.Internal;

namespace Bssom.Serializer.BssMap
{
    internal readonly struct UInt64BytesISegment : IMapKeySegment
    {
        private readonly byte[] _us;
        private readonly int _len;
        private readonly int _uint64Count;
        private readonly int _lastUInt64ByteCount;

        public UInt64BytesISegment(byte[] us) : this(us, us.Length)
        {

        }

        public UInt64BytesISegment(byte[] us, int len)
        {
            DEBUG.Assert(len > 0);
            _us = us;
            _len = len;
            _uint64Count = (int)Math.Ceiling((decimal)len / 8);
            _lastUInt64ByteCount = 8 - (_uint64Count * 8 - _len);
        }

        public unsafe ulong this[int i]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (i != _uint64Count - 1 || _lastUInt64ByteCount == 8)
                    return BssomBinaryPrimitives.ReadUInt64LittleEndian(ref _us[i * 8]);

                int startPos = (_uint64Count - 1) * 8;
                ulong value1 = 0;
                Unsafe.CopyBlock(ref Unsafe.As<ulong, byte>(ref value1), ref _us[startPos], (uint)_lastUInt64ByteCount);
                return BssomBinaryPrimitives.ReadRawUInt64LittleEndian(value1);
            }
        }

        public int Length => _uint64Count;

        public int LastValueByteCount => _lastUInt64ByteCount;

        public IEnumerable<ulong> Ts
        {
            get
            {
                for (int i = 0; i < _uint64Count; i++)
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
        internal int UInt64Count => _uint64Count;
     
    }


}
