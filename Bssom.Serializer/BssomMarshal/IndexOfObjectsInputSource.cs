//using System.Runtime.CompilerServices;

using System.Collections.Generic;

namespace Bssom.Serializer
{
    /// <summary>
    /// <para>通用的IndexOf访问字段输入接口实现</para>
    /// <para>Implementation of general IndexOf access field input interface</para>
    /// </summary>
    public class IndexOfObjectsInputSource : IIndexOfInputSource
    {
        public struct Entry
        {
            /// <summary>
            /// input value
            /// </summary>
            public object Value;
            /// <summary>
            /// True if the value to be entered is used to express access to the Map
            /// </summary>
            public bool ValueIsMapKey;
        }

        private IEnumerator<Entry> entries;

        /// <summary>
        /// <para>传递一组可迭代的值来构造当前实例</para>
        /// <para>Pass a enumerator of iteratable values to construct the current instance</para>
        /// </summary>
        /// <param name="entries"></param>
        public IndexOfObjectsInputSource(IEnumerator<Entry> entries)
        {
            this.entries = entries;
        }

        public bool MoveNext()
        {
            return entries.MoveNext();
        }

        public object CurrentMapKey()
        {
            var entry = entries.Current;
            if (!entry.ValueIsMapKey)
                BssomSerializationOperationException.InputTypeAndDataIsInconsistent(entry.Value.ToString(), "Map");
            return entry.Value;
        }

        public int CurrentArrayIndex()
        {
            var entry = entries.Current;
            if (entry.ValueIsMapKey)
                BssomSerializationOperationException.InputTypeAndDataIsInconsistent(entry.Value.ToString(), "Array");
            return (int)entry.Value;
        }

        public void Reset()
        {
            entries.Reset();
        }
    }
}
