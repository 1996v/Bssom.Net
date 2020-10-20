//using System.Runtime.CompilerServices;

using BssomSerializers.Internal;

namespace BssomSerializers.BssMap.KeyResolvers
{
    internal interface IBssMapKeyResolver
    {
        Raw64BytesISegment GetMap1KeySegment(object key);
        UInt64BytesISegment GetMap2KeySegment(object key);
        object ReadMap2Key(IIteration<byte> keyBytes);
        byte KeyType { get; }
        bool KeyIsNativeType { get; }
    }

    internal interface IBssMapKeyResolver<TKey> : IBssMapKeyResolver
    {
        Raw64BytesISegment GetMap1KeySegment(TKey key);
        UInt64BytesISegment GetMap2KeySegment(TKey key);
        new TKey ReadMap2Key(IIteration<byte> key);
    }
}
