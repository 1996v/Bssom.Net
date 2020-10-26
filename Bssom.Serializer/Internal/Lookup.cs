//using System.Runtime.CompilerServices;

using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Bssom.Serializer.Internal
{
    internal class Lookup<TKey, TElement> : ILookup<TKey, TElement>
    {
        private readonly Dictionary<TKey, IGrouping<TKey, TElement>> groupings;

        public Lookup(IEnumerable<IGrouping<TKey, TElement>> elements)
        {
            groupings = new Dictionary<TKey, IGrouping<TKey, TElement>>();
            foreach (IGrouping<TKey, TElement> item in elements)
            {
                groupings.Add(item.Key, item);
            }
        }

        public IEnumerable<TElement> this[TKey key] => groupings[key];

        public int Count => groupings.Count;

        public bool Contains(TKey key)
        {
            return groupings.ContainsKey(key);
        }

        public IEnumerator<IGrouping<TKey, TElement>> GetEnumerator()
        {
            return groupings.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return groupings.Values.GetEnumerator();
        }
    }
}
