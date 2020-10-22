using Bssom.Serializer.Internal;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Bssom.Serializer.BssMap.KeyResolvers;

namespace Bssom.Serializer
{
    /// <summary>
    /// Represents a BSSOM map.
    /// </summary>
    public class BssomMap : BssomValue, IDictionary<object, object>, IEnumerable<KeyValuePair<BssomValue, object>>
    {
        private Dictionary<object, object> _dict;
        /// <summary>
        /// Initializes a new instance of the <see cref="BssomMap"/> class.
        /// </summary>
        public BssomMap()
        {
            _dict = new Dictionary<object, object>();
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="BssomMap"/> class that is empty, has the specified initial capacity
        /// </summary>
        public BssomMap(int capacity)
        {
            _dict = new Dictionary<object, object>(capacity);
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="BssomMap"/> class that contains elements copied from the specified IEnumerable
        /// </summary>
        /// <param name="ienums"></param>
        public BssomMap(IEnumerable<KeyValuePair<object, object>> ienums)
        {
            if (ienums.TryGetICollectionCount(out int count))
                _dict = new Dictionary<object, object>(count);
            else
                _dict = new Dictionary<object, object>();
            foreach (var item in ienums)
            {
                BssMapKeyResolverProvider.VertyBssMapKeyType(item.Key);
                _dict.Add(item.Key, item.Value);
            }
        }

        internal BssomMap(Dictionary<object, object> dict)
        {
            _dict = dict;
        }

        /// <summary>
        /// Bssom value Type
        /// </summary>
        public override BssomValueType ValueType => BssomValueType.Map;
        /// <summary>
        /// Raw value
        /// </summary>
        public override object RawValue => _dict;
        /// <summary>
        /// Map count
        /// </summary>
        public int Count => _dict.Count;

        /// <summary>
        /// <para>将指定的键和值添加到Map</para>
        /// <para>Adds the specified key and value to the Map</para>
        /// </summary>
        public void Add(object key, object value)
        {
            if (key is BssomValue bsVal)
            {
                switch (bsVal.ValueType)
                {
                    case BssomValueType.Number:
                    case BssomValueType.String:
                    case BssomValueType.Guid:
                    case BssomValueType.Boolean:
                    case BssomValueType.Char:
                    case BssomValueType.DateTime:
                    case BssomValueType.Decimal:
                    case BssomValueType.Float:
                        {
                            _dict.Add(key, value);
                            return;
                        }
                    default:
                        throw BssomSerializationTypeFormatterException.BssomMapKeyUnsupportedType(key.GetType());
                }
            }

            BssMapKeyResolverProvider.VertyBssMapKeyType(key);
            _dict.Add(key, value);
        }

        /// <summary>
        /// <para>将指定的键值对添加到Map</para>
        /// <para>Adds the specified KeyValuePair to the map</para>
        /// </summary>
        public void Add(KeyValuePair<object, object> item) => this.Add(item.Key, item.Value);

        /// <summary>
        /// <para>从map中删除具有指定键的元素</para>
        /// <para> Removes the element with the specified key from the map</para>
        /// </summary>
        public bool Remove(object key)
        {
            return _dict.Remove(key);
        }

        /// <summary>
        /// <para>获取与指定键关联的值</para>
        /// <para>Gets the value associated with the specified key.</para>
        /// </summary>
        public bool TryGetValue(object key, out object value)
        {
            return _dict.TryGetValue(key, out value);
        }

        /// <summary>
        /// <para>获取或设置具有指定键的元素</para>
        /// <para> Gets or sets the element with the specified key.</para>
        /// </summary>
        public object this[object key]
        {
            get
            {
                return _dict[key];
            }
            set => Add(key, value);
        }

        /// <summary>
        /// <para>确定map是否包含具有指定键的元素</para>
        /// <para>Determines whether the map  contains an elementwith the specified key.</para>
        /// </summary>
        public bool ContainsKey(object key)
        {
            return _dict.ContainsKey(key);
        }

        /// <summary>
        /// <para>从map中删除所有键和值</para>
        /// <para>removes all keys and values from map</para>
        /// </summary>
        public void Clear() => _dict.Clear();

        /// <summary>
        /// Get map
        /// </summary>
        /// <returns></returns>
        public IDictionary<object, object> GetMap() => _dict;

        public IEnumerator<KeyValuePair<object, object>> GetEnumerator() => _dict.GetEnumerator();
        ICollection<object> IDictionary<object, object>.Keys => _dict.Keys;
        ICollection<object> IDictionary<object, object>.Values => _dict.Values;
        bool ICollection<KeyValuePair<object, object>>.IsReadOnly => false;
        bool ICollection<KeyValuePair<object, object>>.Remove(KeyValuePair<object, object> item) => _dict.Remove(item);
        bool ICollection<KeyValuePair<object, object>>.Contains(KeyValuePair<object, object> item) => _dict.Contains(item);
        void ICollection<KeyValuePair<object, object>>.CopyTo(KeyValuePair<object, object>[] array, int arrayIndex) => ((IDictionary<object, object>)_dict).CopyTo(array, arrayIndex);
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        IEnumerator<KeyValuePair<BssomValue, object>> IEnumerable<KeyValuePair<BssomValue, object>>.GetEnumerator()
        {
            foreach (var item in _dict)
            {
                if (!(item.Key is BssomValue val))
                    val = BssomValue.Create(item.Key);
                yield return new KeyValuePair<BssomValue, object>(val, item.Value);
            }
        }
    }
}
