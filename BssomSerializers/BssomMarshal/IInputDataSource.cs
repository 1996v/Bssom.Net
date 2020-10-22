//using System.Runtime.CompilerServices;

using System.Collections.Generic;

namespace Bssom.Serializer
{
    /// <summary>
    /// <para>用于在<see cref="BssomFieldMarshaller.IndexOf(IIndexOfInputSource, long)"/>中使用的自定义输入源,可以传递Object类型的MapKey</para>
    /// <para>Used for custom input source used in <see cref="BssomFieldMarshaller.IndexOf(IIndexOfInputSource, long)"/>, you can pass Object type MapKey</para>
    /// </summary>
    public interface IIndexOfInputSource
    {
        /// <summary>
        /// <para>将枚举数前进到集合的下一个元素</para>
        /// <para>Advances the enumerator to the next element of the collection.</para>
        /// </summary>
        /// <returns>如果枚举数成功推进到下一个元素，则为true. true if the enumerator was successfully advanced to the next element</returns>
        bool MoveNext();

        /// <summary>
        /// <para> 获取集合中枚举器当前位置的MapKey</para>
        /// <para> Gets the MapKey in the collection at the current position of the enumerator.</para>
        /// </summary>
        /// <returns>枚举器集合中当前位置的MapKey. The MapKey in the collection at the current position of the enumerator</returns>
        object CurrentMapKey();

        /// <summary>
        /// <para> 获取集合中枚举器当前位置的ArrayIndex</para>
        /// <para> Gets the ArrayIndex in the collection at the current position of the enumerator.</para>
        /// </summary>
        /// <returns>枚举器集合中当前位置的ArrayIndex. The ArrayIndex in the collection at the current position of the enumerator</returns>
        int CurrentArrayIndex();
    }

    public class IndexOfObjectsInputSource : IIndexOfInputSource
    {
        public struct Entry
        {
            /// <summary>
            /// 
            /// </summary>
            public object Value;
            public bool ValueIsMapKey;
        }

        private Entry[] values;

        public IndexOfObjectsInputSource(IEnumerator<Entry> entries)
        {

        }

        public bool MoveNext()
        {
            throw new System.NotImplementedException();
        }

        public object CurrentMapKey()
        {
            throw new System.NotImplementedException();
        }

        public int CurrentArrayIndex()
        {
            throw new System.NotImplementedException();
        }
    }
}
