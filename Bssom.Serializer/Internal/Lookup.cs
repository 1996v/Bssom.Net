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
            this.groupings = new Dictionary<TKey, IGrouping<TKey, TElement>>();
            foreach (var item in elements)
            {
                groupings.Add(item.Key, item);
            }
        }

        public IEnumerable<TElement> this[TKey key]
        {
            get
            {
                return this.groupings[key];
            }
        }

        public int Count
        {
            get
            {
                return this.groupings.Count;
            }
        }

        public bool Contains(TKey key)
        {
            return this.groupings.ContainsKey(key);
        }

        public IEnumerator<IGrouping<TKey, TElement>> GetEnumerator()
        {
            return this.groupings.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.groupings.Values.GetEnumerator();
        }
    }
}
