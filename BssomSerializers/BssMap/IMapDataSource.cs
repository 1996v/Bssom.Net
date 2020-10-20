using System.Collections.Generic;
using BssomSerializers.Binary;
using BssomSerializers.BssMap.KeyResolvers;
using BssomSerializers.Internal;
using BssomSerializers.BssomBuffer;

namespace BssomSerializers.BssMap
{
    internal interface IMapDataSource<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
    {
        int Count { get; }
        long EndPosition { get; }
        BssomReader Reader { get; }
        BssomDeserializeContext Context { get; }
    }
}
