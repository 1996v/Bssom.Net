using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Bssom.Serializer.Resolver;
using Xunit;

namespace Bssom.Serializer.Test
{
    public class ICollectionResolverTest_TypeIsCollection
    {
        private static bool TypeIsCollection(Type t, out Type itemType)
        {
            return ICollectionResolver.TypeIsCollection(t,
                    out ConstructorInfo constructor,
                    out itemType,
                    out bool isImplGenerIList, out bool IsImplIList, out bool isImplGenerICollec, out bool isImplIReadOnlyList);
        }

        [Fact]
        public void NonGenricInterfaceIsCollection()
        {
            {
                Type itemType;
                TypeIsCollection(typeof(IEnumerable), out itemType).IsTrue();
                itemType.Is(typeof(object));
            }
            {
                Type itemType;
                TypeIsCollection(typeof(ICollection), out itemType).IsTrue();
                itemType.Is(typeof(object));
            }
            {
                Type itemType;
                TypeIsCollection(typeof(IList), out itemType).IsTrue();
                itemType.Is(typeof(object));
            }
        }

        [Fact]
        public void GenricInterfaceIsCollection()
        {
            {
                Type itemType;
                TypeIsCollection(typeof(IEnumerable<int>), out itemType).IsTrue();
                itemType.Is(typeof(int));
            }
            {
                Type itemType;
                TypeIsCollection(typeof(ICollection<int>), out itemType).IsTrue();
                itemType.Is(typeof(int));
            }
            {
                Type itemType;
                TypeIsCollection(typeof(IList<int>), out itemType).IsTrue();
                itemType.Is(typeof(int));
            }
            {
                Type itemType;
                TypeIsCollection(typeof(ISet<int>), out itemType).IsTrue();
                itemType.Is(typeof(int));
            }
            {
                Type itemType;
                TypeIsCollection(typeof(IReadOnlyList<int>), out itemType).IsTrue();
                itemType.Is(typeof(int));
            }
            {
                Type itemType;
                TypeIsCollection(typeof(IReadOnlyCollection<int>), out itemType).IsTrue();
                itemType.Is(typeof(int));
            }
        }

        interface _IEnumerable : IEnumerable { }
        interface _ICollection : ICollection { }
        interface _IList : IList { }
        interface _IEnumerable<T> : IEnumerable<T> { }
        interface _ICollection<T> : ICollection<T> { }
        interface _IList<T> : IList<T> { }
        interface _ISet<T> : ISet<T> { }
        interface _IReadOnlyList<T> : IReadOnlyList<T> { }
        interface _IReadOnlyCollection<T> : IReadOnlyCollection<T> { }

        [Fact]
        public void InheritanceInterfaceIsNotCollection()
        {
            Type itemType;
            TypeIsCollection(typeof(_IEnumerable), out itemType).IsFalse();
            TypeIsCollection(typeof(_ICollection), out itemType).IsFalse();
            TypeIsCollection(typeof(_IList), out itemType).IsFalse();
            TypeIsCollection(typeof(_IEnumerable<int>), out itemType).IsFalse();
            TypeIsCollection(typeof(_ICollection<int>), out itemType).IsFalse();
            TypeIsCollection(typeof(_IList<int>), out itemType).IsFalse();
            TypeIsCollection(typeof(_ISet<int>), out itemType).IsFalse();
            TypeIsCollection(typeof(_IReadOnlyList<int>), out itemType).IsFalse();
            TypeIsCollection(typeof(_IReadOnlyCollection<int>), out itemType).IsFalse();
        }

        [Fact]
        public void ListIsSpecifiedCapacityCtor()
        {
            ICollectionResolver.TypeIsCollection(typeof(List<int>),
                    out ConstructorInfo constructor,
                    out Type itemType,
                    out bool isImplGenerIList, out bool IsImplIList, out bool isImplGenerICollec, out bool isImplIReadOnlyList).IsTrue();

            itemType.Is(typeof(int));
            constructor.Is(typeof(List<int>).GetConstructor(new Type[] { typeof(int) }));
        }

        [Fact]
        public void GenerIListImpl_EmptyParasCtor_IsCollection()
        {
            ICollectionResolver.TypeIsCollection(typeof(_GenerIList_Int_EmptyParasCtor),
                    out ConstructorInfo constructor,
                    out Type itemType,
                    out bool isImplGenerIList, out bool IsImplIList, out bool isImplGenerICollec, out bool isImplIReadOnlyList).IsTrue();

            itemType.Is(typeof(int));
            constructor.GetParameters().Length.Is(0);
            isImplGenerIList.IsTrue();
        }

        [Fact]
        public void GenerIListImpl_SameItemTypeColloctionCtor_IsCollection()
        {
            ICollectionResolver.TypeIsCollection(typeof(_GenerIList_Int_SameItemTypeColloctionCtor),
                    out ConstructorInfo constructor,
                    out Type itemType,
                    out bool isImplGenerIList, out bool IsImplIList, out bool isImplGenerICollec, out bool isImplIReadOnlyList).IsTrue();

            itemType.Is(typeof(int));
            constructor.Is(typeof(_GenerIList_Int_SameItemTypeColloctionCtor).GetConstructor(new Type[] { typeof(List<int>) }));
            isImplGenerIList.IsTrue();
        }

        [Fact]
        public void GenerIListImpl_NonSameItemTypeColloctionCtor_IsNotCollection()
        {
            TypeIsCollection(typeof(_GenerIList_Int_NonSameItemTypeColloctionCtor), out var itemType).IsFalse();
        }

        [Fact]
        public void GenerIListImpl_OneSelfTypeCtor_IsNotCollectionAndNotRecursionOverflow()
        {
            TypeIsCollection(typeof(_GenerIList_Int_OneSelfTypeCtor), out var itemType).IsFalse();
        }

        [Fact]
        public void GenerICollectionImpl_EmptyParasCtor_IsCollection()
        {
            ICollectionResolver.TypeIsCollection(typeof(_GenerICollection_Int_EmptyParasCtor),
                    out ConstructorInfo constructor,
                    out Type itemType,
                    out bool isImplGenerIList, out bool IsImplIList, out bool isImplGenerICollec, out bool isImplIReadOnlyList).IsTrue();

            itemType.Is(typeof(int));
            constructor.GetParameters().Length.Is(0);
            isImplGenerICollec.IsTrue();
        }

        [Fact]
        public void GenerICollectionImpl_SameItemTypeColloctionCtor_IsCollection()
        {
            ICollectionResolver.TypeIsCollection(typeof(_GenerICollection_Int_SameItemTypeColloctionCtor),
                    out ConstructorInfo constructor,
                    out Type itemType,
                    out bool isImplGenerIList, out bool IsImplIList, out bool isImplGenerICollec, out bool isImplIReadOnlyList).IsTrue();

            itemType.Is(typeof(int));
            constructor.Is(typeof(_GenerICollection_Int_SameItemTypeColloctionCtor).GetConstructor(new Type[] { typeof(List<int>) }));
            isImplGenerICollec.IsTrue();
        }

        [Fact]
        public void GenerICollectionImpl_NonSameItemTypeColloctionCtor_IsNotCollection()
        {
            TypeIsCollection(typeof(_GenerICollection_Int_NonSameItemTypeColloctionCtor), out var itemType).IsFalse();
        }

        [Fact]
        public void GenerICollectionImpl_OneSelfTypeCtor_IsNotCollectionAndNotRecursionOverflow()
        {
            TypeIsCollection(typeof(_GenerICollection_Int_OneSelfTypeCtor), out var itemType).IsFalse();
        }

        [Fact]
        public void GenerIEnumerableWithNonGenerICollectionImpl_EmptyParasCtor_IsNotCollection()
        {
            TypeIsCollection(typeof(_GenerIEnumerableWithNonGenerICollection_Int_EmptyParasCtor), out var itemType).IsFalse();
        }

        [Fact]
        public void GenerIEnumerableWithNonGenerICollectionImpl_SameItemTypeColloctionCtor_IsCollection()
        {
            ICollectionResolver.TypeIsCollection(typeof(_GenerIEnumerableWithNonGenerICollection_Int_SameItemTypeColloctionCtor),
                    out ConstructorInfo constructor,
                    out Type itemType,
                    out bool isImplGenerIList, out bool IsImplIList, out bool isImplGenerICollec, out bool isImplIReadOnlyList).IsTrue();

            itemType.Is(typeof(int));
            constructor.Is(typeof(_GenerIEnumerableWithNonGenerICollection_Int_SameItemTypeColloctionCtor).GetConstructor(new Type[] { typeof(List<int>) }));
        }

        [Fact]
        public void GenerIEnumerableWithNonGenerICollectionImpl_NonSameItemTypeColloctionCtor_IsNotCollection()
        {
            TypeIsCollection(typeof(_GenerIEnumerableWithNonGenerICollection_Int_NonSameItemTypeColloctionCtor), out var itemType).IsFalse();
        }

        [Fact]
        public void GenerIEnumerableWithNonGenerICollectionImpl_OneSelfTypeCtor_IsNotCollectionAndNotRecursionOverflow()
        {
            TypeIsCollection(typeof(_GenerIEnumerableWithNonGenerICollection_Int_OneSelfTypeCtor), out var itemType).IsFalse();
        }

        [Fact]
        public void IListImpl_EmptyParasCtor_IsCollection()
        {
            ICollectionResolver.TypeIsCollection(typeof(_IList_EmptyParasCtor),
                    out ConstructorInfo constructor,
                    out Type itemType,
                    out bool isImplGenerIList, out bool IsImplIList, out bool isImplGenerICollec, out bool isImplIReadOnlyList).IsTrue();

            itemType.Is(typeof(object));
            constructor.GetParameters().Length.Is(0);
        }

        [Fact]
        public void IListImpl_ObjectItemTypeColloctionCtor_IsCollection()
        {
            ICollectionResolver.TypeIsCollection(typeof(_IList_ObjectItemTypeColloctionCtor),
                    out ConstructorInfo constructor,
                    out Type itemType,
                    out bool isImplGenerIList, out bool IsImplIList, out bool isImplGenerICollec, out bool isImplIReadOnlyList).IsTrue();

            itemType.Is(typeof(object));
            constructor.Is(typeof(_IList_ObjectItemTypeColloctionCtor).GetConstructor(new Type[] { typeof(List<object>) }));
        }

        [Fact]
        public void IListImpl_NonObjectItemTypeColloctionCtor_IsNotCollectionAndNotRecursionOverflow()
        {
            TypeIsCollection(typeof(_IList_NonObjectItemTypeColloctionCtor), out var itemType).IsFalse();
        }

        [Fact]
        public void IListImpl_OneSelfTypeCtor_IsNotCollectionAndNotRecursionOverflow()
        {
            TypeIsCollection(typeof(_IList_OneSelfTypeCtor), out var itemType).IsFalse();
        }

        [Fact]
        public void ICollectionImpl_EmptyParasCtor_IsNotCollection()
        {
            TypeIsCollection(typeof(_ICollection_EmptyParasCtor), out var itemType).IsFalse();
        }

        [Fact]
        public void ICollectionImpl_ObjectItemTypeColloctionCtor_IsCollection()
        {
            ICollectionResolver.TypeIsCollection(typeof(_ICollection_ObjectItemTypeColloctionCtor),
                    out ConstructorInfo constructor,
                    out Type itemType,
                    out bool isImplGenerIList, out bool IsImplIList, out bool isImplGenerICollec, out bool isImplIReadOnlyList).IsTrue();

            itemType.Is(typeof(object));
            constructor.Is(typeof(_ICollection_ObjectItemTypeColloctionCtor).GetConstructor(new Type[] { typeof(List<object>) }));
        }

        [Fact]
        public void ICollectionImpl_NonObjectItemTypeColloctionCtor_IsNotCollectionAndNotRecursionOverflow()
        {
            TypeIsCollection(typeof(_ICollection_NonObjectItemTypeColloctionCtor), out var itemType).IsFalse();
        }

        [Fact]
        public void ICollectionImpl_OneSelfTypeCtor_IsNotCollectionAndNotRecursionOverflow()
        {
            TypeIsCollection(typeof(_ICollection_OneSelfTypeCtor), out var itemType).IsFalse();
        }
    }


    class _GenerIList_Int_EmptyParasCtor : IList<int>
    {
        public int this[int index] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public int Count => throw new NotImplementedException();

        public bool IsReadOnly => throw new NotImplementedException();

        public void Add(int item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(int item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(int[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<int> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public int IndexOf(int item)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, int item)
        {
            throw new NotImplementedException();
        }

        public bool Remove(int item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }

    class _GenerIList_Int_SameItemTypeColloctionCtor : IList<int>
    {
        public _GenerIList_Int_SameItemTypeColloctionCtor(List<int> colloction) { }

        public int this[int index] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public int Count => throw new NotImplementedException();

        public bool IsReadOnly => throw new NotImplementedException();

        public void Add(int item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(int item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(int[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<int> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public int IndexOf(int item)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, int item)
        {
            throw new NotImplementedException();
        }

        public bool Remove(int item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }

    class _GenerIList_Int_NonSameItemTypeColloctionCtor : IList<int>
    {
        public _GenerIList_Int_NonSameItemTypeColloctionCtor(List<long> colloction) { }

        public int this[int index] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public int Count => throw new NotImplementedException();

        public bool IsReadOnly => throw new NotImplementedException();

        public void Add(int item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(int item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(int[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<int> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public int IndexOf(int item)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, int item)
        {
            throw new NotImplementedException();
        }

        public bool Remove(int item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }

    class _GenerIList_Int_OneSelfTypeCtor : IList<int>
    {
        public _GenerIList_Int_OneSelfTypeCtor(_GenerIList_Int_OneSelfTypeCtor colloction) { }

        public int this[int index] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public int Count => throw new NotImplementedException();

        public bool IsReadOnly => throw new NotImplementedException();

        public void Add(int item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(int item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(int[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<int> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public int IndexOf(int item)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, int item)
        {
            throw new NotImplementedException();
        }

        public bool Remove(int item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }

    class _GenerICollection_Int_EmptyParasCtor : ICollection<int>
    {
        public int Count => throw new NotImplementedException();

        public bool IsReadOnly => throw new NotImplementedException();

        public void Add(int item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(int item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(int[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<int> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public bool Remove(int item)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }

    class _GenerICollection_Int_SameItemTypeColloctionCtor : ICollection<int>
    {
        public _GenerICollection_Int_SameItemTypeColloctionCtor(List<int> colloction) { }

        public int Count => throw new NotImplementedException();

        public bool IsReadOnly => throw new NotImplementedException();

        public void Add(int item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(int item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(int[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<int> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public bool Remove(int item)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }

    class _GenerICollection_Int_NonSameItemTypeColloctionCtor : ICollection<int>
    {
        public _GenerICollection_Int_NonSameItemTypeColloctionCtor(List<long> colloction) { }

        public int Count => throw new NotImplementedException();

        public bool IsReadOnly => throw new NotImplementedException();

        public void Add(int item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(int item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(int[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<int> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public bool Remove(int item)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }

    class _GenerICollection_Int_OneSelfTypeCtor : ICollection<int>
    {
        public _GenerICollection_Int_OneSelfTypeCtor(_GenerICollection_Int_OneSelfTypeCtor colloction) { }

        public int Count => throw new NotImplementedException();

        public bool IsReadOnly => throw new NotImplementedException();

        public void Add(int item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(int item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(int[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<int> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public bool Remove(int item)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }

    class _GenerIEnumerableWithNonGenerICollection_Int_EmptyParasCtor : IEnumerable<int>, ICollection
    {
        public int Count => throw new NotImplementedException();

        public bool IsSynchronized => throw new NotImplementedException();

        public object SyncRoot => throw new NotImplementedException();

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<int> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }

    class _GenerIEnumerableWithNonGenerICollection_Int_SameItemTypeColloctionCtor : IEnumerable<int>, ICollection
    {
        public _GenerIEnumerableWithNonGenerICollection_Int_SameItemTypeColloctionCtor(List<int> colloction) { }

        public int Count => throw new NotImplementedException();

        public bool IsSynchronized => throw new NotImplementedException();

        public object SyncRoot => throw new NotImplementedException();

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<int> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }

    class _GenerIEnumerableWithNonGenerICollection_Int_NonSameItemTypeColloctionCtor : IEnumerable<int>, ICollection
    {
        public _GenerIEnumerableWithNonGenerICollection_Int_NonSameItemTypeColloctionCtor(List<long> colloction) { }

        public int Count => throw new NotImplementedException();

        public bool IsSynchronized => throw new NotImplementedException();

        public object SyncRoot => throw new NotImplementedException();

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<int> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }

    class _GenerIEnumerableWithNonGenerICollection_Int_OneSelfTypeCtor : IEnumerable<int>, ICollection
    {
        public _GenerIEnumerableWithNonGenerICollection_Int_OneSelfTypeCtor(_GenerIEnumerableWithNonGenerICollection_Int_OneSelfTypeCtor colloction) { }

        public int Count => throw new NotImplementedException();

        public bool IsSynchronized => throw new NotImplementedException();

        public object SyncRoot => throw new NotImplementedException();

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<int> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }

    class _IList_EmptyParasCtor : IList
    {
        public object this[int index] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public bool IsFixedSize => throw new NotImplementedException();

        public bool IsReadOnly => throw new NotImplementedException();

        public int Count => throw new NotImplementedException();

        public bool IsSynchronized => throw new NotImplementedException();

        public object SyncRoot => throw new NotImplementedException();

        public int Add(object value)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(object value)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public int IndexOf(object value)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, object value)
        {
            throw new NotImplementedException();
        }

        public void Remove(object value)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }
    }

    class _IList_ObjectItemTypeColloctionCtor : IList
    {
        public _IList_ObjectItemTypeColloctionCtor(List<object> colloction) { }

        public object this[int index] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public bool IsFixedSize => throw new NotImplementedException();

        public bool IsReadOnly => throw new NotImplementedException();

        public int Count => throw new NotImplementedException();

        public bool IsSynchronized => throw new NotImplementedException();

        public object SyncRoot => throw new NotImplementedException();

        public int Add(object value)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(object value)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public int IndexOf(object value)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, object value)
        {
            throw new NotImplementedException();
        }

        public void Remove(object value)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }
    }

    class _IList_NonObjectItemTypeColloctionCtor : IList
    {
        public _IList_NonObjectItemTypeColloctionCtor(List<int> colloction) { }

        public object this[int index] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public bool IsFixedSize => throw new NotImplementedException();

        public bool IsReadOnly => throw new NotImplementedException();

        public int Count => throw new NotImplementedException();

        public bool IsSynchronized => throw new NotImplementedException();

        public object SyncRoot => throw new NotImplementedException();

        public int Add(object value)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(object value)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public int IndexOf(object value)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, object value)
        {
            throw new NotImplementedException();
        }

        public void Remove(object value)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }
    }

    class _IList_OneSelfTypeCtor : IList
    {
        public _IList_OneSelfTypeCtor(_IList_OneSelfTypeCtor colloction) { }

        public object this[int index] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public bool IsFixedSize => throw new NotImplementedException();

        public bool IsReadOnly => throw new NotImplementedException();

        public int Count => throw new NotImplementedException();

        public bool IsSynchronized => throw new NotImplementedException();

        public object SyncRoot => throw new NotImplementedException();

        public int Add(object value)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(object value)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public int IndexOf(object value)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, object value)
        {
            throw new NotImplementedException();
        }

        public void Remove(object value)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }
    }

    class _ICollection_EmptyParasCtor : ICollection
    {
        public int Count => throw new NotImplementedException();

        public bool IsSynchronized => throw new NotImplementedException();

        public object SyncRoot => throw new NotImplementedException();

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }

    class _ICollection_ObjectItemTypeColloctionCtor : ICollection
    {
        public _ICollection_ObjectItemTypeColloctionCtor(List<object> colloction) { }

        public int Count => throw new NotImplementedException();

        public bool IsSynchronized => throw new NotImplementedException();

        public object SyncRoot => throw new NotImplementedException();

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }

    class _ICollection_NonObjectItemTypeColloctionCtor : ICollection
    {
        public _ICollection_NonObjectItemTypeColloctionCtor(List<int> colloction) { }

        public int Count => throw new NotImplementedException();

        public bool IsSynchronized => throw new NotImplementedException();

        public object SyncRoot => throw new NotImplementedException();

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }

    class _ICollection_OneSelfTypeCtor : ICollection
    {
        public _ICollection_OneSelfTypeCtor(_ICollection_OneSelfTypeCtor colloction) { }

        public int Count => throw new NotImplementedException();

        public bool IsSynchronized => throw new NotImplementedException();

        public object SyncRoot => throw new NotImplementedException();

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
