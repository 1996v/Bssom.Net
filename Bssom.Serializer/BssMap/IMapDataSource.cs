using System.Collections.Generic;
using Bssom.Serializer.Binary;
using Bssom.Serializer.BssMap.KeyResolvers;
using Bssom.Serializer.Internal;
using Bssom.Serializer.BssomBuffer;

namespace Bssom.Serializer.BssMap
{
    internal interface IMapDataSource<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
    {
        int Count { get; }
        long EndPosition { get; }
        BssomReader Reader { get; }
        BssomDeserializeContext Context { get; }
    }
}
