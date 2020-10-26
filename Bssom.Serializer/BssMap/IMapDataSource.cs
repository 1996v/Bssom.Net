using System.Collections.Generic;

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
