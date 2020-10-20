using BssomSerializers.Resolver;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Xunit;

namespace BssomSerializers.Test
{

    public class IDictionaryResolverTest_CustomTypeImpl
    {
        [Fact]
        public void ResolverGetFormatter_IDicTypeImplWithOutEmptyParasCtor_IsDynamicGenerateFormatterType()
        {
            IDictionaryResolver.Instance.GetFormatter<_IDictionaryCustomImpl_EmptyParasCtor>().IsNotNull();
        }

        [Fact]
        public void ResolverGetFormatter_IDicTypeImplWithIDictionaryCtor_IsDynamicGenerateFormatterType()
        {
            IDictionaryResolver.Instance.GetFormatter<_IDictionaryCustomImpl_IDictionaryCtor>().IsNotNull();
        }

        [Fact]
        public void ResolverGetFormatter_CustomGenericDictTypeImpl_IsDynamicGenerateFormatterType()
        {
            IDictionaryResolver.Instance.GetFormatter<_IGenericDictionaryCustomImpl_EmptyParasCtor<int, int>>().IsNotNull();
            IDictionaryResolver.Instance.GetFormatter<_IGenericDictionaryCustomImpl_EmptyParasCtor<object, int>>().IsNotNull();
            IDictionaryResolver.Instance.GetFormatter<_IGenericDictionaryCustomImpl_SameDictionaryKeyVakyeTypeCtor<int, int>>().IsNotNull();
            IDictionaryResolver.Instance.GetFormatter<_IGenericDictionaryCustomImpl_SameDictionaryKeyVakyeTypeCtor<object, string>>().IsNotNull();
        }

        [Fact]
        public void CustomGenericDictTypeImpl_EmptyParasCtor_IsEmpty_FormatterMapTypeIsCorrectly()
        {
            var val = new _IGenericDictionaryCustomImpl_EmptyParasCtor<int, int>();
            VerifyHelper.VerifyMap1Type(val, BssomSerializerOptions.Default.WithIDictionaryIsSerializeMap1Type(true));
            VerifyHelper.VerifyIDictWithMap2Type<_IGenericDictionaryCustomImpl_EmptyParasCtor<int, int>, int, int>(val, BssomSerializerOptions.Default.WithIDictionaryIsSerializeMap1Type(false));
        }

        [Fact]
        public void CustomGenericDictTypeImpl_EmptyParasCtor_FormatterMap1TypeIsCorrectly()
        {
            var val = new _IGenericDictionaryCustomImpl_EmptyParasCtor<int, int>(RandomHelper.RandomValue<Dictionary<int, int>>());
            VerifyHelper.VerifyMap1Type(val, BssomSerializerOptions.Default.WithIDictionaryIsSerializeMap1Type(true));

            var val2 = new _IGenericDictionaryCustomImpl_EmptyParasCtor<DateTime, int>(RandomHelper.RandomValue<Dictionary<DateTime, int>>());
            VerifyHelper.VerifyMap1Type(val2, BssomSerializerOptions.Default.WithIDictionaryIsSerializeMap1Type(true));

            var val3 = new _IGenericDictionaryCustomImpl_EmptyParasCtor<object, string>(RandomHelper.RandomValueWithOutStringEmpty<Dictionary<object, string>>());
            VerifyHelper.VerifyMap1Type(val3, BssomSerializerOptions.Default.WithIDictionaryIsSerializeMap1Type(true));
        }

        [Fact]
        public void CustomGenericDictTypeImpl_EmptyParasCtor_FormatterMap2TypeIsCorrectly()
        {
            var val = new _IGenericDictionaryCustomImpl_EmptyParasCtor<int, int>(RandomHelper.RandomValue<Dictionary<int, int>>());
            VerifyHelper.VerifyIDictWithMap2Type<_IGenericDictionaryCustomImpl_EmptyParasCtor<int, int>, int, int>(val, BssomSerializerOptions.Default.WithIDictionaryIsSerializeMap1Type(false));

            var val2 = new _IGenericDictionaryCustomImpl_EmptyParasCtor<DateTime, int>(RandomHelper.RandomValue<Dictionary<DateTime, int>>());
            VerifyHelper.VerifyIDictWithMap2Type<_IGenericDictionaryCustomImpl_EmptyParasCtor<DateTime, int>, DateTime, int>(val2, BssomSerializerOptions.Default.WithIDictionaryIsSerializeMap1Type(false));

            var val3 = new _IGenericDictionaryCustomImpl_EmptyParasCtor<object, string>(RandomHelper.RandomValueWithOutStringEmpty<Dictionary<object, string>>());
            VerifyHelper.VerifyIDictWithMap2Type<_IGenericDictionaryCustomImpl_EmptyParasCtor<object, string>, object, string>(val3, BssomSerializerOptions.Default.WithIDictionaryIsSerializeMap1Type(false));
        }

        [Fact]
        public void CustomGenericDictTypeImpl_SameDictionaryKeyVakyeTypeCtor_IsEmpty_FormatterMapTypeIsCorrectly()
        {
            var val = new _IGenericDictionaryCustomImpl_SameDictionaryKeyVakyeTypeCtor<int, int>(new Dictionary<int, int>());
            VerifyHelper.VerifyMap1Type(val, BssomSerializerOptions.Default.WithIDictionaryIsSerializeMap1Type(true));
            VerifyHelper.VerifyIDictWithMap2Type<_IGenericDictionaryCustomImpl_SameDictionaryKeyVakyeTypeCtor<int, int>, int, int>(val, BssomSerializerOptions.Default.WithIDictionaryIsSerializeMap1Type(false));
        }

        [Fact]
        public void CustomGenericDictTypeImpl_SameDictionaryKeyVakyeTypeCtor_FormatterMap1TypeIsCorrectly()
        {
            var val = new _IGenericDictionaryCustomImpl_SameDictionaryKeyVakyeTypeCtor<int, int>(RandomHelper.RandomValue<Dictionary<int, int>>());
            VerifyHelper.VerifyMap1Type(val, BssomSerializerOptions.Default.WithIDictionaryIsSerializeMap1Type(true));

            var val2 = new _IGenericDictionaryCustomImpl_SameDictionaryKeyVakyeTypeCtor<DateTime, int>(RandomHelper.RandomValue<Dictionary<DateTime, int>>());
            VerifyHelper.VerifyMap1Type(val2, BssomSerializerOptions.Default.WithIDictionaryIsSerializeMap1Type(true));

            var val3 = new _IGenericDictionaryCustomImpl_SameDictionaryKeyVakyeTypeCtor<object, string>(RandomHelper.RandomValueWithOutStringEmpty<Dictionary<object, string>>());
            VerifyHelper.VerifyMap1Type(val3, BssomSerializerOptions.Default.WithIDictionaryIsSerializeMap1Type(true));
        }

        [Fact]
        public void CustomGenericDictTypeImpl_SameDictionaryKeyVakyeTypeCtor_FormatterMap2TypeIsCorrectly()
        {
            var val = new _IGenericDictionaryCustomImpl_SameDictionaryKeyVakyeTypeCtor<int, int>(RandomHelper.RandomValue<Dictionary<int, int>>());
            VerifyHelper.VerifyIDictWithMap2Type<_IGenericDictionaryCustomImpl_SameDictionaryKeyVakyeTypeCtor<int, int>, int, int>(val, BssomSerializerOptions.Default.WithIDictionaryIsSerializeMap1Type(false));

            var val2 = new _IGenericDictionaryCustomImpl_SameDictionaryKeyVakyeTypeCtor<DateTime, int>(RandomHelper.RandomValue<Dictionary<DateTime, int>>());
            VerifyHelper.VerifyIDictWithMap2Type<_IGenericDictionaryCustomImpl_SameDictionaryKeyVakyeTypeCtor<DateTime, int>, DateTime, int>(val2, BssomSerializerOptions.Default.WithIDictionaryIsSerializeMap1Type(false));

            var val3 = new _IGenericDictionaryCustomImpl_SameDictionaryKeyVakyeTypeCtor<object, string>(RandomHelper.RandomValueWithOutStringEmpty<Dictionary<object, string>>());
            VerifyHelper.VerifyIDictWithMap2Type<_IGenericDictionaryCustomImpl_SameDictionaryKeyVakyeTypeCtor<object, string>, object, string>(val3, BssomSerializerOptions.Default.WithIDictionaryIsSerializeMap1Type(false));
        }


        [Fact]
        public void DictTypeImpl_EmptyParasCtor_IsEmpty_FormatterMap2TypeIsCorrectly()
        {
            var val = new _IDictionaryCustomImpl_EmptyParasCtor();
            VerifyHelper.VerifyIDictWithMap2Type(val, BssomSerializerOptions.Default.WithIDictionaryIsSerializeMap1Type(false));
        }

        [Fact]
        public void DictTypeImpl_EmptyParasCtor_FormatterMap2TypeIsCorrectly()
        {
            var val = new _IDictionaryCustomImpl_EmptyParasCtor();
            for (int i = 0; i < 33; i++)
            {
                val.Add(i, RandomHelper.RandomValue<object>());
            }
            VerifyHelper.VerifyIDictWithMap2Type(val, BssomSerializerOptions.Default.WithIDictionaryIsSerializeMap1Type(false));
        }

        [Fact]
        public void DictTypeImpl_IIDictionaryCtor_IsEmpty_FormatterMap2TypeIsCorrectly()
        {
            var val = new _IDictionaryCustomImpl_IDictionaryCtor(new Dictionary<int,int>());
            VerifyHelper.VerifyIDictWithMap2Type(val, BssomSerializerOptions.Default.WithIDictionaryIsSerializeMap1Type(false));
        }

        [Fact]
        public void DictTypeImpl_IIDictionaryCtor_FormatterMap2TypeIsCorrectly()
        {
            var val = new _IDictionaryCustomImpl_IDictionaryCtor(RandomHelper.RandomValue<Dictionary<int, int>>());
            VerifyHelper.VerifyIDictWithMap2Type(val, BssomSerializerOptions.Default.WithIDictionaryIsSerializeMap1Type(false));
        }
    }

    public class _IGenericDictionaryCustomImpl_EmptyParasCtor<K, V> : IDictionary<K, V>
    {
        private Dictionary<K, V> _dict;

        public _IGenericDictionaryCustomImpl_EmptyParasCtor()
        {
            _dict = new Dictionary<K, V>();
        }

        public _IGenericDictionaryCustomImpl_EmptyParasCtor(Dictionary<K, V> dict)
        {
            _dict = dict;
        }

        public V this[K key] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public ICollection<K> Keys => throw new NotImplementedException();

        public ICollection<V> Values => throw new NotImplementedException();

        public int Count => _dict.Count;

        public bool IsReadOnly => throw new NotImplementedException();

        public void Add(K key, V value)
        {
            _dict.Add(key, value);
        }

        public void Add(KeyValuePair<K, V> item)
        {
            _dict.Add(item.Key, item.Value);
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(KeyValuePair<K, V> item)
        {
            throw new NotImplementedException();
        }

        public bool ContainsKey(K key)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(KeyValuePair<K, V>[] array, int arrayIndex)
        {
            ((IDictionary<K, V>)_dict).CopyTo(array, arrayIndex);
        }

        public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
        {
            return _dict.GetEnumerator();
        }

        public bool Remove(K key)
        {
            throw new NotImplementedException();
        }

        public bool Remove(KeyValuePair<K, V> item)
        {
            throw new NotImplementedException();
        }

        public bool TryGetValue(K key,  out V value)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class _IGenericDictionaryCustomImpl_SameDictionaryKeyVakyeTypeCtor<K, V> : IReadOnlyDictionary<K, V>
    {
        private Dictionary<K, V> _dict = new Dictionary<K, V>();

        public _IGenericDictionaryCustomImpl_SameDictionaryKeyVakyeTypeCtor(Dictionary<K, V> dict)
        {
            _dict = dict;
        }

        public V this[K key] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public int Count => _dict.Count;

        public bool IsReadOnly => throw new NotImplementedException();

        public IEnumerable<K> Keys => throw new NotImplementedException();

        public IEnumerable<V> Values => throw new NotImplementedException();

        public void Add(K key, V value)
        {
            _dict.Add(key, value);
        }

        public void Add(KeyValuePair<K, V> item)
        {
            _dict.Add(item.Key, item.Value);
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(KeyValuePair<K, V> item)
        {
            throw new NotImplementedException();
        }

        public bool ContainsKey(K key)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(KeyValuePair<K, V>[] array, int arrayIndex)
        {
            ((IDictionary<K, V>)_dict).CopyTo(array, arrayIndex);
        }

        public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
        {
            return _dict.GetEnumerator();
        }

        public bool Remove(K key)
        {
            throw new NotImplementedException();
        }

        public bool Remove(KeyValuePair<K, V> item)
        {
            throw new NotImplementedException();
        }

        public bool TryGetValue(K key,out V value)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class _IDictionaryCustomImpl_EmptyParasCtor : IDictionary
    {
        private IDictionary _dict;

        public _IDictionaryCustomImpl_EmptyParasCtor()
        {
            _dict = new Hashtable();
        }

        public object this[object key] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public bool IsFixedSize => throw new NotImplementedException();

        public bool IsReadOnly => throw new NotImplementedException();

        public ICollection Keys => throw new NotImplementedException();

        public ICollection Values => throw new NotImplementedException();

        public int Count => _dict.Count;

        public bool IsSynchronized => throw new NotImplementedException();

        public object SyncRoot => throw new NotImplementedException();

        public void Add(object key, object value)
        {
            _dict.Add(key, value);
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(object key)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public IDictionaryEnumerator GetEnumerator()
        {
            return _dict.GetEnumerator();
        }

        public void Remove(object key)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class _IDictionaryCustomImpl_IDictionaryCtor : IDictionary
    {
        private IDictionary _dict;


        public _IDictionaryCustomImpl_IDictionaryCtor(IDictionary dict)
        {
            _dict = dict;
        }

        public object this[object key] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public bool IsFixedSize => throw new NotImplementedException();

        public bool IsReadOnly => throw new NotImplementedException();

        public ICollection Keys => throw new NotImplementedException();

        public ICollection Values => throw new NotImplementedException();

        public int Count => _dict.Count;

        public bool IsSynchronized => throw new NotImplementedException();

        public object SyncRoot => throw new NotImplementedException();

        public void Add(object key, object value)
        {
            _dict.Add(key, value);
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(object key)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public IDictionaryEnumerator GetEnumerator()
        {
            return _dict.GetEnumerator();
        }

        public void Remove(object key)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
