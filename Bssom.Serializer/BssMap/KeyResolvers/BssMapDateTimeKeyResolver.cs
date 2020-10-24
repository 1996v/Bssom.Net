//using System.Runtime.CompilerServices;

using System;
using System.Runtime.CompilerServices;
using Bssom.Serializer.Binary;
using Bssom.Serializer.Internal;

namespace Bssom.Serializer.BssMap.KeyResolvers
{
    internal class BssMapDateTimeKeyResolver : IBssMapKeyResolver<DateTime>
    {
        public static BssMapDateTimeKeyResolver Insance = new BssMapDateTimeKeyResolver();

        public byte KeyType => NativeBssomType.DateTimeCode;

        public bool KeyIsNativeType => true;

        public Raw64BytesISegment GetMap1KeySegment(DateTime key)
        {
            byte[] bs = new byte[BssomBinaryPrimitives.NativeDateTimeSize];
            Unsafe.WriteUnaligned(ref bs[0], key);
            return new Raw64BytesISegment(bs);
        }

        public Raw64BytesISegment GetMap1KeySegment(object key)
        {
            return GetMap1KeySegment((DateTime)key);
        }

        public UInt64BytesISegment GetMap2KeySegment(DateTime key)
        {
            byte[] bs = new byte[BssomBinaryPrimitives.NativeDateTimeSize];
            BssomBinaryPrimitives.WriteNativeDateTime(ref bs[0], key);
            return new UInt64BytesISegment(bs);
        }

        public UInt64BytesISegment GetMap2KeySegment(object key)
        {
            return GetMap2KeySegment((DateTime)key);
        }

        public unsafe DateTime ReadMap2Key(Iterable<byte> key)
        {
            return BssomBinaryPrimitives.ReadNativeDateTime(ref key.GetFirstElementReference(out bool isContiguousMemoryArea));
        }

        object IBssMapKeyResolver.ReadMap2Key(Iterable<byte> key)
        {
            return ReadMap2Key(key);
        }
    }
}
