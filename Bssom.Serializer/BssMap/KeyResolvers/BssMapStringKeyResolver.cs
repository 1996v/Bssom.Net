//using System.Runtime.CompilerServices;

using Bssom.Serializer.Binary;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Bssom.Serializer.Internal;

namespace Bssom.Serializer.BssMap.KeyResolvers
{
    internal class BssMapStringKeyResolver : IBssMapKeyResolver<string>
    {
        public static BssMapStringKeyResolver Insance = new BssMapStringKeyResolver();

        public byte KeyType => BssomType.StringCode;

        public bool KeyIsNativeType => false;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Raw64BytesISegment GetMap1KeySegment(string key)
        {
            BssMapKeyResolverProvider.VertyBssMapStringKey(key);
            return new Raw64BytesISegment(UTF8Encoding.UTF8.GetBytes(key));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Raw64BytesISegment GetMap1KeySegment(object key)
        {
            return GetMap1KeySegment((string)key);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public UInt64BytesISegment GetMap2KeySegment(string key)
        {
            BssMapKeyResolverProvider.VertyBssMapStringKey(key);
            return new UInt64BytesISegment(UTF8Encoding.UTF8.GetBytes(key));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public UInt64BytesISegment GetMap2KeySegment(object key)
        {
            return GetMap2KeySegment((string)key);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe string ReadMap2Key(Iterable<byte> key)
        {
            ref byte refl = ref key.GetFirstElementReference(out bool isContiguousMemoryArea);
            fixed (byte* ptr = &refl)
            {
                return new string((sbyte*)ptr, 0, key.Length, Encoding.UTF8);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        object IBssMapKeyResolver.ReadMap2Key(Iterable<byte> key)
        {
            return ReadMap2Key(key);
        }
    }
}
