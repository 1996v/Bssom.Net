using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Bssom.Serializer.Resolver;
using Xunit;

namespace Bssom.Serializer.Test
{

    public class IDictionaryResolverTest_TypeIsIDictionary
    {
        private static bool TypeIsIDictionary(Type t)
        {
            return IDictionaryResolver.TypeIsDictionary(t, out ConstructorInfo constructor, out bool typeIsGeneric, out Type genericTypeDefinition, out Type genericKeyType, out Type genericValueType);
        }

        [Fact]
        public void NonGenricInterfaceIsIDictionary()
        {
            TypeIsIDictionary(typeof(IDictionary)).IsTrue();
        }

        [Fact]
        public void GenricInterfaceIsIDictionary()
        {
            TypeIsIDictionary(typeof(IDictionary<int, int>)).IsTrue();
            TypeIsIDictionary(typeof(IReadOnlyDictionary<int, int>)).IsTrue();
        }

        interface _IDictionary : IDictionary { }
        interface _IDictionary<K, V> : IDictionary<K, V> { }
        interface _IReadOnlyDictionary<K, V> : IReadOnlyDictionary<K, V> { }

        [Fact]
        public void InheritanceInterfaceIsNotIDictionary()
        {
            TypeIsIDictionary(typeof(_IDictionary)).IsFalse();
            TypeIsIDictionary(typeof(_IDictionary<int, int>)).IsFalse();
            TypeIsIDictionary(typeof(_IReadOnlyDictionary<int, int>)).IsFalse();
        }

        [Fact]
        public void DictionaryIsSpecifiedCapacityCtor()
        {
            IDictionaryResolver.TypeIsDictionary(typeof(Dictionary<int, int>),
                   out ConstructorInfo constructor, out bool typeIsGeneric, out Type genericTypeDefinition, out Type genericKeyType, out Type genericValueType)
                .IsTrue();

            genericKeyType.Is(typeof(int));
            genericValueType.Is(typeof(int));
            constructor.Is(typeof(Dictionary<int, int>).GetConstructor(new Type[] { typeof(int) }));
        }

        [Fact]
        public void SortedListIsSpecifiedCapacityCtor()
        {
            IDictionaryResolver.TypeIsDictionary(typeof(SortedList<int, int>),
                   out ConstructorInfo constructor, out bool typeIsGeneric, out Type genericTypeDefinition, out Type genericKeyType, out Type genericValueType)
                .IsTrue();

            genericKeyType.Is(typeof(int));
            genericValueType.Is(typeof(int));
            constructor.Is(typeof(SortedList<int, int>).GetConstructor(new Type[] { typeof(int) }));
        }

        [Fact]
        public void GenerDictionary_EmptyParasCtor_IsDictionary()
        {
            IDictionaryResolver.TypeIsDictionary(typeof(_GenerDictionary_EmptyParasCtor),
                    out ConstructorInfo constructor, out bool typeIsGeneric, out Type genericTypeDefinition, out Type genericKeyType, out Type genericValueType)
                 .IsTrue();

            genericKeyType.Is(typeof(int));
            genericValueType.Is(typeof(int));
            constructor.GetParameters().Length.Is(0);
        }

        [Fact]
        public void GenerDictionary_SameItemTypeIDictionaryCtor_IsDictionary()
        {
            IDictionaryResolver.TypeIsDictionary(typeof(_GenerDictionary_SameItemTypeIDictionaryCtor),
                    out ConstructorInfo constructor, out bool typeIsGeneric, out Type genericTypeDefinition, out Type genericKeyType, out Type genericValueType)
                 .IsTrue();

            genericKeyType.Is(typeof(int));
            genericValueType.Is(typeof(int));
            constructor.Is(typeof(_GenerDictionary_SameItemTypeIDictionaryCtor).GetConstructor(new Type[] { typeof(Dictionary<int, int>) }));
        }

        [Fact]
        public void GenerDictionary_NonSameItemTypeIDictionaryCtor_IsNotDictionary()
        {
            TypeIsIDictionary(typeof(_GenerDictionary_NonSameItemTypeIDictionaryCtor)).IsFalse();
        }

        [Fact]
        public void GenerDictionary_OneSelfTypeCtor_IsNotCollectionAndNotRecursionOverflow()
        {
            TypeIsIDictionary(typeof(_GenerDictionary_OneSelfTypeCtor)).IsFalse();
        }

        [Fact]
        public void InheritanceIDictionary_IDictionaryCtor_IsDictionray()
        {
            TypeIsIDictionary(typeof(_IDictionary_IDictionaryCtor)).IsTrue();
        }

        [Fact]
        public void InheritanceIDictionary_EmptyParasCtor_IsDictionray()
        {
            TypeIsIDictionary(typeof(_IDictionary_EmptyParasCtor)).IsTrue();
        }
    }

    class _GenerDictionary_EmptyParasCtor : IDictionary<int, int>
    {
        public int this[int key] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public ICollection<int> Keys => throw new NotImplementedException();

        public ICollection<int> Values => throw new NotImplementedException();

        public int Count => throw new NotImplementedException();

        public bool IsReadOnly => throw new NotImplementedException();

        public void Add(int key, int value)
        {
            throw new NotImplementedException();
        }

        public void Add(KeyValuePair<int, int> item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(KeyValuePair<int, int> item)
        {
            throw new NotImplementedException();
        }

        public bool ContainsKey(int key)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(KeyValuePair<int, int>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<KeyValuePair<int, int>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public bool Remove(int key)
        {
            throw new NotImplementedException();
        }

        public bool Remove(KeyValuePair<int, int> item)
        {
            throw new NotImplementedException();
        }

        public bool TryGetValue(int key,out int value)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }

    class _GenerDictionary_SameItemTypeIDictionaryCtor : IDictionary<int, int>
    {
        public _GenerDictionary_SameItemTypeIDictionaryCtor(Dictionary<int, int> dict) { }

        public int this[int key] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public ICollection<int> Keys => throw new NotImplementedException();

        public ICollection<int> Values => throw new NotImplementedException();

        public int Count => throw new NotImplementedException();

        public bool IsReadOnly => throw new NotImplementedException();

        public void Add(int key, int value)
        {
            throw new NotImplementedException();
        }

        public void Add(KeyValuePair<int, int> item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(KeyValuePair<int, int> item)
        {
            throw new NotImplementedException();
        }

        public bool ContainsKey(int key)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(KeyValuePair<int, int>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<KeyValuePair<int, int>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public bool Remove(int key)
        {
            throw new NotImplementedException();
        }

        public bool Remove(KeyValuePair<int, int> item)
        {
            throw new NotImplementedException();
        }

        public bool TryGetValue(int key, out int value)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }

    class _GenerDictionary_NonSameItemTypeIDictionaryCtor : IDictionary<int, int>
    {
        public _GenerDictionary_NonSameItemTypeIDictionaryCtor(Dictionary<long, int> dict) { }

        public int this[int key] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public ICollection<int> Keys => throw new NotImplementedException();

        public ICollection<int> Values => throw new NotImplementedException();

        public int Count => throw new NotImplementedException();

        public bool IsReadOnly => throw new NotImplementedException();

        public void Add(int key, int value)
        {
            throw new NotImplementedException();
        }

        public void Add(KeyValuePair<int, int> item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(KeyValuePair<int, int> item)
        {
            throw new NotImplementedException();
        }

        public bool ContainsKey(int key)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(KeyValuePair<int, int>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<KeyValuePair<int, int>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public bool Remove(int key)
        {
            throw new NotImplementedException();
        }

        public bool Remove(KeyValuePair<int, int> item)
        {
            throw new NotImplementedException();
        }

        public bool TryGetValue(int key, out int value)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }

    class _GenerDictionary_OneSelfTypeCtor : IDictionary<int, int>
    {
        public _GenerDictionary_OneSelfTypeCtor(_GenerDictionary_OneSelfTypeCtor dict) { }

        public int this[int key] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public ICollection<int> Keys => throw new NotImplementedException();

        public ICollection<int> Values => throw new NotImplementedException();

        public int Count => throw new NotImplementedException();

        public bool IsReadOnly => throw new NotImplementedException();

        public void Add(int key, int value)
        {
            throw new NotImplementedException();
        }

        public void Add(KeyValuePair<int, int> item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(KeyValuePair<int, int> item)
        {
            throw new NotImplementedException();
        }

        public bool ContainsKey(int key)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(KeyValuePair<int, int>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<KeyValuePair<int, int>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public bool Remove(int key)
        {
            throw new NotImplementedException();
        }

        public bool Remove(KeyValuePair<int, int> item)
        {
            throw new NotImplementedException();
        }

        public bool TryGetValue(int key,  out int value)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }

    class _IDictionary_IDictionaryCtor : IDictionary
    {
        public _IDictionary_IDictionaryCtor(IDictionary dict) { }

        public object this[object key] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public bool IsFixedSize => throw new NotImplementedException();

        public bool IsReadOnly => throw new NotImplementedException();

        public ICollection Keys => throw new NotImplementedException();

        public ICollection Values => throw new NotImplementedException();

        public int Count => throw new NotImplementedException();

        public bool IsSynchronized => throw new NotImplementedException();

        public object SyncRoot => throw new NotImplementedException();

        public void Add(object key, object value)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public void Remove(object key)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }

    class _IDictionary_EmptyParasCtor : IDictionary
    {
        public object this[object key] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public bool IsFixedSize => throw new NotImplementedException();

        public bool IsReadOnly => throw new NotImplementedException();

        public ICollection Keys => throw new NotImplementedException();

        public ICollection Values => throw new NotImplementedException();

        public int Count => throw new NotImplementedException();

        public bool IsSynchronized => throw new NotImplementedException();

        public object SyncRoot => throw new NotImplementedException();

        public void Add(object key, object value)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public void Remove(object key)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
