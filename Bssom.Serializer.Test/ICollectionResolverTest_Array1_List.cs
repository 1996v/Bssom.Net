using System;
using System.Collections.Generic;
using Bssom.Serializer.Formatters;
using Bssom.Serializer.Resolver;
using Xunit;

namespace Bssom.Serializer.Test
{

    public class ICollectionResolverTest_Array1_List
    {
        [Fact]
        public void ResolverGetFormatter_Int16ListFormatter_CanBeObtainedNormally()
        {
            ICollectionResolver.Instance.GetFormatter<List<Int16>>().Is(Int16ListFormatter.Instance);
        }

        [Fact]
        public void ResolverGetFormatter_Int32ListFormatter_CanBeObtainedNormally()
        {
            ICollectionResolver.Instance.GetFormatter<List<Int32>>().Is(Int32ListFormatter.Instance);
        }

        [Fact]
        public void ResolverGetFormatter_Int64ListFormatter_CanBeObtainedNormally()
        {
            ICollectionResolver.Instance.GetFormatter<List<Int64>>().Is(Int64ListFormatter.Instance);
        }

        [Fact]
        public void ResolverGetFormatter_UInt16ListFormatter_CanBeObtainedNormally()
        {
            ICollectionResolver.Instance.GetFormatter<List<UInt16>>().Is(UInt16ListFormatter.Instance);
        }

        [Fact]
        public void ResolverGetFormatter_UInt32ListFormatter_CanBeObtainedNormally()
        {
            ICollectionResolver.Instance.GetFormatter<List<UInt32>>().Is(UInt32ListFormatter.Instance);
        }

        [Fact]
        public void ResolverGetFormatter_UInt64ListFormatter_CanBeObtainedNormally()
        {
            ICollectionResolver.Instance.GetFormatter<List<UInt64>>().Is(UInt64ListFormatter.Instance);
        }

        [Fact]
        public void ResolverGetFormatter_Float32ListFormatter_CanBeObtainedNormally()
        {
            ICollectionResolver.Instance.GetFormatter<List<Single>>().Is(Float32ListFormatter.Instance);
        }

        [Fact]
        public void ResolverGetFormatter_Float64ListFormatter_CanBeObtainedNormally()
        {
            ICollectionResolver.Instance.GetFormatter<List<Double>>().Is(Float64ListFormatter.Instance);
        }

        [Fact]
        public void ResolverGetFormatter_BooleanListFormatter_CanBeObtainedNormally()
        {
            ICollectionResolver.Instance.GetFormatter<List<bool>>().Is(BooleanListFormatter.Instance);
        }

        [Fact]
        public void ResolverGetFormatter_UInt8ListFormatter_CanBeObtainedNormally()
        {
            ICollectionResolver.Instance.GetFormatter<List<byte>>().Is(UInt8ListFormatter.Instance);
        }

        [Fact]
        public void ResolverGetFormatter_Int8ListFormatter_CanBeObtainedNormally()
        {
            ICollectionResolver.Instance.GetFormatter<List<sbyte>>().Is(Int8ListFormatter.Instance);
        }

        [Fact]
        public void ResolverGetFormatter_CharListFormatter_CanBeObtainedNormally()
        {
            ICollectionResolver.Instance.GetFormatter<List<char>>().Is(CharListFormatter.Instance);
        }

        [Fact]
        public void ResolverGetFormatter_DateTimeListFormatter_CanBeObtainedNormally()
        {
            ICollectionResolver.Instance.GetFormatter<List<DateTime>>().Is(DateTimeListFormatter.Instance);
        }

        [Fact]
        public void ResolverGetFormatter_DecimalListFormatter_CanBeObtainedNormally()
        {
            ICollectionResolver.Instance.GetFormatter<List<Decimal>>().Is(DecimalListFormatter.Instance);
        }

        [Fact]
        public void ResolverGetFormatter_GuidListFormatter_CanBeObtainedNormally()
        {
            ICollectionResolver.Instance.GetFormatter<List<Guid>>().Is(GuidListFormatter.Instance);
        }

        [Fact]
        public void Int8ListFormatter_FormatterTypeIsArray1TypeAndFormatterIsCorrectly()
        {
            var val = RandomHelper.RandomValue<List<sbyte>>();
            VerifyHelper.VerifyEntityWithArray1(val);
        }

        [Fact]
        public void Int16ListFormatter_FormatterTypeIsArray1TypeAndFormatterIsCorrectly()
        {
            var val = RandomHelper.RandomValue<List<Int16>>();
            VerifyHelper.VerifyEntityWithArray1(val);
        }

        [Fact]
        public void Int32ListFormatter_FormatterTypeIsArray1TypeAndFormatterIsCorrectly()
        {
            var val = RandomHelper.RandomValue<List<Int32>>();
            VerifyHelper.VerifyEntityWithArray1(val);
        }

        [Fact]
        public void Int64ListFormatter_FormatterTypeIsArray1TypeAndFormatterIsCorrectly()
        {
            var val = RandomHelper.RandomValue<List<Int64>>();
            VerifyHelper.VerifyEntityWithArray1(val);
        }

        [Fact]
        public void UInt8ListFormatter_FormatterTypeIsArray1TypeAndFormatterIsCorrectly()
        {
            var val = RandomHelper.RandomValue<List<byte>>();
            VerifyHelper.VerifyEntityWithArray1(val);
        }

        [Fact]
        public void UInt16ListFormatter_FormatterTypeIsArray1TypeAndFormatterIsCorrectly()
        {
            var val = RandomHelper.RandomValue<List<UInt16>>();
            VerifyHelper.VerifyEntityWithArray1(val);
        }

        [Fact]
        public void UInt32ListFormatter_FormatterTypeIsArray1TypeAndFormatterIsCorrectly()
        {
            var val = RandomHelper.RandomValue<List<UInt32>>();
            VerifyHelper.VerifyEntityWithArray1(val);
        }

        [Fact]
        public void UInt64ListFormatter_FormatterTypeIsArray1TypeAndFormatterIsCorrectly()
        {
            var val = RandomHelper.RandomValue<List<UInt64>>();
            VerifyHelper.VerifyEntityWithArray1(val);
        }

        [Fact]
        public void Float32ListFormatter_FormatterTypeIsArray1TypeAndFormatterIsCorrectly()
        {
            var val = RandomHelper.RandomValue<List<Single>>();
            VerifyHelper.VerifyEntityWithArray1(val);
        }

        [Fact]
        public void Float64ListFormatter_FormatterTypeIsArray1TypeAndFormatterIsCorrectly()
        {
            var val = RandomHelper.RandomValue<List<Double>>();
            VerifyHelper.VerifyEntityWithArray1(val);
        }

        [Fact]
        public void BooleanListFormatter_FormatterTypeIsArray1TypeAndFormatterIsCorrectly()
        {
            var val = RandomHelper.RandomValue<List<bool>>();
            VerifyHelper.VerifyEntityWithArray1(val);
        }

        [Fact]
        public void CharListFormatter_FormatterTypeIsArray1TypeAndFormatterIsCorrectly()
        {
            var val = RandomHelper.RandomValue<List<Char>>();
            VerifyHelper.VerifyEntityWithArray1(val);
        }

        [Fact]
        public void DateTimeListFormatter_FormatterTypeIsArray1TypeAndFormatterIsCorrectly()
        {
            var val = RandomHelper.RandomValue<List<DateTime>>();
            VerifyHelper.VerifyEntityWithArray1(val);
        }

        [Fact]
        public void DecimalListFormatter_FormatterTypeIsArray1TypeAndFormatterIsCorrectly()
        {
            var val = RandomHelper.RandomValue<List<Decimal>>();
            VerifyHelper.VerifyEntityWithArray1(val);
        }

        [Fact]
        public void GuidListFormatter_FormatterTypeIsArray1TypeAndFormatterIsCorrectly()
        {
            var val = RandomHelper.RandomValue<List<Guid>>();
            VerifyHelper.VerifyEntityWithArray1(val);
        }
    }
}
