using Bssom.Serializer.Formatters;
using System;
using System.Collections;
using System.Collections.Generic;
using Bssom.Serializer.Resolvers;
using Xunit;

namespace Bssom.Serializer.Test
{
    public class ICollectionResolverTest_CustomTypeImpl
    {
        [Fact]
        public void ResolverGetFormatter_CustomTypeImpl_IsDynamicGenerateFormatterType()
        {
            ICollectionResolver.Instance.GetFormatter<_ICollectionCustomImpl_EmptyParasCtor<int>>().IsNotNull();
            ICollectionResolver.Instance.GetFormatter<_ICollectionCustomImpl_EmptyParasCtor<string>>().IsNotNull();
            ICollectionResolver.Instance.GetFormatter<_ICollectionCustomImpl_SameItemCollectionTypeCtor<int>>().IsNotNull();
            ICollectionResolver.Instance.GetFormatter<_ICollectionCustomImpl_SameItemCollectionTypeCtor<string>>().IsNotNull();
            ICollectionResolver.Instance.GetFormatter<List<string>>().IsNotNull();
            ICollectionResolver.Instance.GetFormatter<HashSet<string>>().IsNotNull();
        }

        [Fact]
        public void CustomTypeImpl_EmptyParasCtor_FormatterIsCorrectly()
        {
            var val = new _ICollectionCustomImpl_EmptyParasCtor<int>(1, 2, 3);
            VerifyHelper.VerifyEntityWithArray1(val);

            var val2 = new _ICollectionCustomImpl_EmptyParasCtor<DateTime>(DateTime.Now, DateTime.MaxValue);
            VerifyHelper.VerifyEntityWithArray1(val2);

            var val3 = new _ICollectionCustomImpl_EmptyParasCtor<string>("a", "b");
            VerifyHelper.VerifyEntityWithArray2(val3);
        }

        [Fact]
        public void CustomTypeImpl_SameItemCollectionTypeCtor_FormatterIsCorrectly()
        {
            var val = new _ICollectionCustomImpl_SameItemCollectionTypeCtor<int>(1, 2, 3);
            VerifyHelper.VerifyEntityWithArray1(val);

            var val2 = new _ICollectionCustomImpl_SameItemCollectionTypeCtor<DateTime>(DateTime.Now, DateTime.MaxValue);
            VerifyHelper.VerifyEntityWithArray1(val2);

            var val3 = new _ICollectionCustomImpl_SameItemCollectionTypeCtor<string>("a", "b");
            VerifyHelper.VerifyEntityWithArray2(val3);
        }

        [Fact]
        public void CustomTypeImpl_StandardDateTime_FormatterIsCorrectly()
        {
            var val = new _ICollectionCustomImpl_SameItemCollectionTypeCtor<DateTime>(DateTime.Now, DateTime.MaxValue);
            VerifyHelper.VerifyEntityWithArray1(val, BssomSerializerOptions.Default.WithIsUseStandardDateTime(false));
        }
    }

   public class _ICollectionCustomImpl_EmptyParasCtor<T> : ICollection<T>
    {
        private List<T> _ary;

        public _ICollectionCustomImpl_EmptyParasCtor()
        {
            _ary = new List<T>();
        }

        public _ICollectionCustomImpl_EmptyParasCtor(params T[] ary)
        {
            _ary = new List<T>();
            _ary.AddRange(ary);
        }

        public int Count => _ary.Count;

        public bool IsReadOnly => throw new NotImplementedException();

        public void Add(T item)
        {
            _ary.Add(item);
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(T item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < _ary.Count; i++)
            {
                yield return _ary[i];
            }
        }

        public bool Remove(T item)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

   public class _ICollectionCustomImpl_SameItemCollectionTypeCtor<T> : ICollection<T>
    {
        private List<T> _ary = new List<T>();

        public _ICollectionCustomImpl_SameItemCollectionTypeCtor(params T[] ary)
        {
            _ary.AddRange(ary);
        }

        public int Count => _ary.Count;

        public bool IsReadOnly => throw new NotImplementedException();

        public void Add(T item)
        {
            _ary.Add(item);
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(T item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < _ary.Count; i++)
            {
                yield return _ary[i];
            }
        }

        public bool Remove(T item)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
