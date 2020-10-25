using System;
using System.Collections;
using System.Collections.Generic;
using Bssom.Serializer.Resolvers;
using Xunit;

namespace Bssom.Serializer.Test
{
    public class ICollectionResolverTest_Interface
    {
        [Fact]
        public void ResolverGetFormatter_IEnumerableType_IsDynamicGenerateFormatterType()
        {
            ICollectionResolver.Instance.GetFormatter<IEnumerable>().IsNotNull();
        }
        [Fact]
        public void ResolverGetFormatter_ICollectionType_IsDynamicGenerateFormatterType()
        {
            ICollectionResolver.Instance.GetFormatter<ICollection>().IsNotNull();
        }
        [Fact]
        public void ResolverGetFormatter_IListType_IsDynamicGenerateFormatterType()
        {
            ICollectionResolver.Instance.GetFormatter<IList>().IsNotNull();
        }
        [Fact]
        public void ResolverGetFormatter_GenericIEnumerableType_IsDynamicGenerateFormatterType()
        {
            //Array1
            ICollectionResolver.Instance.GetFormatter<IEnumerable<int>>().IsNotNull();

            //Array2
            ICollectionResolver.Instance.GetFormatter<IEnumerable<string>>().IsNotNull();
        }
        [Fact]
        public void ResolverGetFormatter_GenericICollectionType_IsDynamicGenerateFormatterType()
        {
            //Array1
            ICollectionResolver.Instance.GetFormatter<ICollection<int>>().IsNotNull();

            //Array2
            ICollectionResolver.Instance.GetFormatter<ICollection<string>>().IsNotNull();
        }
        [Fact]
        public void ResolverGetFormatter_GenericISetType_IsDynamicGenerateFormatterType()
        {
            //Array1
            ICollectionResolver.Instance.GetFormatter<ISet<int>>().IsNotNull();

            //Array2
            ICollectionResolver.Instance.GetFormatter<ISet<string>>().IsNotNull();
        }
        [Fact]
        public void ResolverGetFormatter_GenericIReadOnlyListType_IsDynamicGenerateFormatterType()
        {
            //Array1
            ICollectionResolver.Instance.GetFormatter<IReadOnlyList<int>>().IsNotNull();

            //Array2
            ICollectionResolver.Instance.GetFormatter<IReadOnlyList<string>>().IsNotNull();
        }
        [Fact]
        public void ResolverGetFormatter_GenericIReadOnlyCollectionType_IsDynamicGenerateFormatterType()
        {
            //Array1
            ICollectionResolver.Instance.GetFormatter<IReadOnlyCollection<int>>().IsNotNull();

            //Array2
            ICollectionResolver.Instance.GetFormatter<IReadOnlyCollection<string>>().IsNotNull();
        }

        [Fact]
        public void IEnumerableType_FormatterIsCorrectlyAndBssomTypeIsAlwaysArray2Type()
        {
            IEnumerable val = RandomHelper.RandomValue<List<int>>();
            VerifyHelper.VerifyEntityWithArray2(val);
        }

        [Fact]
        public void IEnumerableType_DeserializeTypeIsListObject()
        {
            IEnumerable val = RandomHelper.RandomValue<List<int>>();
            var buf = BssomSerializer.Serialize(val);
            BssomSerializer.Deserialize<IEnumerable>(buf).IsType<List<object>>();
        }

        [Fact]
        public void ICollectionType_FormatterIsCorrectlyAndBssomTypeIsAlwaysArray2Type()
        {
            ICollection val = RandomHelper.RandomValue<List<int>>();
            VerifyHelper.VerifyEntityWithArray2(val);
        }

        [Fact]
        public void ICollectionType_DeserializeTypeIsListObject()
        {
            ICollection val = RandomHelper.RandomValue<List<int>>();
            var buf = BssomSerializer.Serialize(val);
            BssomSerializer.Deserialize<ICollection>(buf).IsType<List<object>>();
        }

        [Fact]
        public void IListType_FormatterIsCorrectlyAndBssomTypeIsAlwaysArray2Type()
        {
            IList val = RandomHelper.RandomValue<List<int>>();
            VerifyHelper.VerifyEntityWithArray2(val);
        }

        [Fact]
        public void IListType_DeserializeTypeIsListObject()
        {
            IList val = RandomHelper.RandomValue<List<int>>();
            var buf = BssomSerializer.Serialize(val);
            BssomSerializer.Deserialize<IList>(buf).IsType<List<object>>();
        }

        [Fact]
        public void GenericIEnumerableType_FormatterIsCorrectly()
        {
            //Array1
            IEnumerable<DateTime> val = RandomHelper.RandomValue<List<DateTime>>();
            VerifyHelper.VerifyEntityWithArray1(val);

            //Array2
            IEnumerable<string> val2 = RandomHelper.RandomValue<List<string>>();
            VerifyHelper.VerifyEntityWithArray2(val2);
        }

        [Fact]
        public void GenericIEnumerableType_DeserializeTypeIsGenericList()
        {
            IEnumerable<int> val = RandomHelper.RandomValue<List<int>>();
            var buf = BssomSerializer.Serialize(val);
            BssomSerializer.Deserialize<IEnumerable<int>>(buf).IsType<List<int>>();
        }

        [Fact]
        public void GenericICollectionType_FormatterIsCorrectly()
        {
            //Array1
            ICollection<DateTime> val = RandomHelper.RandomValue<List<DateTime>>();
            VerifyHelper.VerifyEntityWithArray1(val);

            //Array2
            ICollection<string> val2 = RandomHelper.RandomValue<List<string>>();
            VerifyHelper.VerifyEntityWithArray2(val2);
        }

        [Fact]
        public void GenericICollectionType_DeserializeTypeIsGenericList()
        {
            ICollection<int> val = RandomHelper.RandomValue<List<int>>();
            var buf = BssomSerializer.Serialize(val);
            BssomSerializer.Deserialize<ICollection<int>>(buf).IsType<List<int>>();
        }

        [Fact]
        public void GenericISetType_FormatterIsCorrectly()
        {
            //Array1
            ISet<DateTime> val = RandomHelper.RandomValue<HashSet<DateTime>>();
            VerifyHelper.VerifyEntityWithArray1(val);

            //Array2
            ISet<string> val2 = RandomHelper.RandomValue<HashSet<string>>();
            VerifyHelper.VerifyEntityWithArray2(val2);
        }

        [Fact]
        public void GenericISetType_DeserializeTypeIsHashSet()
        {
            ISet<int> val = RandomHelper.RandomValue<HashSet<int>>();
            var buf = BssomSerializer.Serialize(val);
            BssomSerializer.Deserialize<ISet<int>>(buf).IsType<HashSet<int>>();
        }

        [Fact]
        public void GenericIReadOnlyListType_FormatterIsCorrectly()
        {
            //Array1
            IReadOnlyList<DateTime> val = RandomHelper.RandomValue<List<DateTime>>();
            VerifyHelper.VerifyEntityWithArray1(val);

            //Array2
            IReadOnlyList<string> val2 = RandomHelper.RandomValue<List<string>>();
            VerifyHelper.VerifyEntityWithArray2(val2);
        }

        [Fact]
        public void GenericIReadOnlyListType_DeserializeTypeIsGenericList()
        {
            IReadOnlyList<int> val = RandomHelper.RandomValue<List<int>>();
            var buf = BssomSerializer.Serialize(val);
            BssomSerializer.Deserialize<IReadOnlyList<int>>(buf).IsType<List<int>>();
        }

        [Fact]
        public void GenericIReadOnlyCollection_FormatterIsCorrectly()
        {
            //Array1
            IReadOnlyCollection<DateTime> val = RandomHelper.RandomValue<List<DateTime>>();
            VerifyHelper.VerifyEntityWithArray1(val);

            //Array2
            IReadOnlyCollection<string> val2 = RandomHelper.RandomValue<List<string>>();
            VerifyHelper.VerifyEntityWithArray2(val2);
        }

        [Fact]
        public void GenericIReadOnlyCollection_DeserializeTypeIsGenericList()
        {
            IReadOnlyCollection<int> val = RandomHelper.RandomValue<List<int>>();
            var buf = BssomSerializer.Serialize(val);
            BssomSerializer.Deserialize<IReadOnlyCollection<int>>(buf).IsType<List<int>>();
        }

        [Fact]
        public void IEnumerableType_CustomImpl_WithOutCountProperty_FormatterIsCorrectly()
        {
            //Array1
            {
                IEnumerable<int> val = new _IEnumerableCustomImpl<int>(1, 2, 3);
                VerifyHelper.VerifyEntityWithArray1(val);
                IEnumerable val2 = val;
                VerifyHelper.VerifyEntityWithArray2(val2);
            }
            {
                IEnumerable<DateTime> val = new _IEnumerableCustomImpl<DateTime>(DateTime.Now, DateTime.MaxValue);
                VerifyHelper.VerifyEntityWithArray1(val);
                IEnumerable val2 = val;
                VerifyHelper.VerifyEntityWithArray2(val2);
            }

            //Array2
            {
                IEnumerable<string> val = new _IEnumerableCustomImpl<string>("a", "b", "c");
                VerifyHelper.VerifyEntityWithArray2(val);
                IEnumerable val2 = val;
                VerifyHelper.VerifyEntityWithArray2(val2);
            }
        }
    }

    class _IEnumerableCustomImpl<T> : IEnumerable<T>
    {
        private T[] _ary;

        public _IEnumerableCustomImpl(params T[] ary)
        {
            _ary = ary;
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < _ary.Length; i++)
            {
                yield return _ary[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
