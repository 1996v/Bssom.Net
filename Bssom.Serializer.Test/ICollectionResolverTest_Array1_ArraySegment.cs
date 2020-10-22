using System;
using Bssom.Serializer.Formatters;
using Bssom.Serializer.Resolver;
using Xunit;

namespace Bssom.Serializer.Test
{

    public class ICollectionResolverTest_Array1_ArraySegment
    {
        [Fact]
        public void ResolverGetFormatter_Int16ArraySegmentFormatter_CanBeObtainedNormally()
        {
            ICollectionResolver.Instance.GetFormatter<ArraySegment<Int16>>().Is(Int16ArraySegmentFormatter.Instance);
        }

        [Fact]
        public void ResolverGetFormatter_Int32ArraySegmentFormatter_CanBeObtainedNormally()
        {
            ICollectionResolver.Instance.GetFormatter<ArraySegment<Int32>>().Is(Int32ArraySegmentFormatter.Instance);
        }

        [Fact]
        public void ResolverGetFormatter_Int64ArraySegmentFormatter_CanBeObtainedNormally()
        {
            ICollectionResolver.Instance.GetFormatter<ArraySegment<Int64>>().Is(Int64ArraySegmentFormatter.Instance);
        }

        [Fact]
        public void ResolverGetFormatter_UInt16ArraySegmentFormatter_CanBeObtainedNormally()
        {
            ICollectionResolver.Instance.GetFormatter<ArraySegment<UInt16>>().Is(UInt16ArraySegmentFormatter.Instance);
        }

        [Fact]
        public void ResolverGetFormatter_UInt32ArraySegmentFormatter_CanBeObtainedNormally()
        {
            ICollectionResolver.Instance.GetFormatter<ArraySegment<UInt32>>().Is(UInt32ArraySegmentFormatter.Instance);
        }

        [Fact]
        public void ResolverGetFormatter_UInt64ArraySegmentFormatter_CanBeObtainedNormally()
        {
            ICollectionResolver.Instance.GetFormatter<ArraySegment<UInt64>>().Is(UInt64ArraySegmentFormatter.Instance);
        }

        [Fact]
        public void ResolverGetFormatter_Float32ArraySegmentFormatter_CanBeObtainedNormally()
        {
            ICollectionResolver.Instance.GetFormatter<ArraySegment<Single>>().Is(Float32ArraySegmentFormatter.Instance);
        }

        [Fact]
        public void ResolverGetFormatter_Float64ArraySegmentFormatter_CanBeObtainedNormally()
        {
            ICollectionResolver.Instance.GetFormatter<ArraySegment<Double>>().Is(Float64ArraySegmentFormatter.Instance);
        }

        [Fact]
        public void ResolverGetFormatter_BooleanArraySegmentFormatter_CanBeObtainedNormally()
        {
            ICollectionResolver.Instance.GetFormatter<ArraySegment<bool>>().Is(BooleanArraySegmentFormatter.Instance);
        }

        [Fact]
        public void ResolverGetFormatter_UInt8ArraySegmentFormatter_CanBeObtainedNormally()
        {
            ICollectionResolver.Instance.GetFormatter<ArraySegment<byte>>().Is(UInt8ArraySegmentFormatter.Instance);
        }

        [Fact]
        public void ResolverGetFormatter_Int8ArraySegmentFormatter_CanBeObtainedNormally()
        {
            ICollectionResolver.Instance.GetFormatter<ArraySegment<sbyte>>().Is(Int8ArraySegmentFormatter.Instance);
        }

        [Fact]
        public void ResolverGetFormatter_CharArraySegmentFormatter_CanBeObtainedNormally()
        {
            ICollectionResolver.Instance.GetFormatter<ArraySegment<char>>().Is(CharArraySegmentFormatter.Instance);
        }

        [Fact]
        public void ResolverGetFormatter_DateTimeArraySegmentFormatter_CanBeObtainedNormally()
        {
            ICollectionResolver.Instance.GetFormatter<ArraySegment<DateTime>>().Is(DateTimeArraySegmentFormatter.Instance);
        }

        [Fact]
        public void ResolverGetFormatter_DecimalArraySegmentFormatter_CanBeObtainedNormally()
        {
            ICollectionResolver.Instance.GetFormatter<ArraySegment<Decimal>>().Is(DecimalArraySegmentFormatter.Instance);
        }

        [Fact]
        public void ResolverGetFormatter_GuidArraySegmentFormatter_CanBeObtainedNormally()
        {
            ICollectionResolver.Instance.GetFormatter<ArraySegment<Guid>>().Is(GuidArraySegmentFormatter.Instance);
        }

        [Fact]
        public void Int8ArraySegmentFormatter_FormatterTypeIsArray1TypeAndFormatterIsCorrectly()
        {
            var val = RandomHelper.RandomValue<ArraySegment<sbyte>>();
            VerifyHelper.VerifyEntityWithArray1(val);
        }

        [Fact]
        public void Int16ArraySegmentFormatter_FormatterTypeIsArray1TypeAndFormatterIsCorrectly()
        {
            var val = RandomHelper.RandomValue<ArraySegment<Int16>>();
            VerifyHelper.VerifyEntityWithArray1(val);
        }

        [Fact]
        public void Int32ArraySegmentFormatter_FormatterTypeIsArray1TypeAndFormatterIsCorrectly()
        {
            var val = RandomHelper.RandomValue<ArraySegment<Int32>>();
            VerifyHelper.VerifyEntityWithArray1(val);
        }

        [Fact]
        public void Int64ArraySegmentFormatter_FormatterTypeIsArray1TypeAndFormatterIsCorrectly()
        {
            var val = RandomHelper.RandomValue<ArraySegment<Int64>>();
            VerifyHelper.VerifyEntityWithArray1(val);
        }

        [Fact]
        public void UInt8ArraySegmentFormatter_FormatterTypeIsArray1TypeAndFormatterIsCorrectly()
        {
            var val = RandomHelper.RandomValue<ArraySegment<byte>>();
            VerifyHelper.VerifyEntityWithArray1(val);
        }

        [Fact]
        public void UInt16ArraySegmentFormatter_FormatterTypeIsArray1TypeAndFormatterIsCorrectly()
        {
            var val = RandomHelper.RandomValue<ArraySegment<UInt16>>();
            VerifyHelper.VerifyEntityWithArray1(val);
        }

        [Fact]
        public void UInt32ArraySegmentFormatter_FormatterTypeIsArray1TypeAndFormatterIsCorrectly()
        {
            var val = RandomHelper.RandomValue<ArraySegment<UInt32>>();
            VerifyHelper.VerifyEntityWithArray1(val);
        }

        [Fact]
        public void UInt64ArraySegmentFormatter_FormatterTypeIsArray1TypeAndFormatterIsCorrectly()
        {
            var val = RandomHelper.RandomValue<ArraySegment<UInt64>>();
            VerifyHelper.VerifyEntityWithArray1(val);
        }

        [Fact]
        public void Float32ArraySegmentFormatter_FormatterTypeIsArray1TypeAndFormatterIsCorrectly()
        {
            var val = RandomHelper.RandomValue<ArraySegment<Single>>();
            VerifyHelper.VerifyEntityWithArray1(val);
        }

        [Fact]
        public void Float64ArraySegmentFormatter_FormatterTypeIsArray1TypeAndFormatterIsCorrectly()
        {
            var val = RandomHelper.RandomValue<ArraySegment<Double>>();
            VerifyHelper.VerifyEntityWithArray1(val);
        }

        [Fact]
        public void BooleanArraySegmentFormatter_FormatterTypeIsArray1TypeAndFormatterIsCorrectly()
        {
            var val = RandomHelper.RandomValue<ArraySegment<bool>>();
            VerifyHelper.VerifyEntityWithArray1(val);
        }

        [Fact]
        public void CharArraySegmentFormatter_FormatterTypeIsArray1TypeAndFormatterIsCorrectly()
        {
            var val = RandomHelper.RandomValue<ArraySegment<Char>>();
            VerifyHelper.VerifyEntityWithArray1(val);
        }

        [Fact]
        public void DateTimeArraySegmentFormatter_FormatterTypeIsArray1TypeAndFormatterIsCorrectly()
        {
            var val = RandomHelper.RandomValue<ArraySegment<DateTime>>();
            VerifyHelper.VerifyEntityWithArray1(val);
        }

        [Fact]
        public void DecimalArraySegmentFormatter_FormatterTypeIsArray1TypeAndFormatterIsCorrectly()
        {
            var val = RandomHelper.RandomValue<ArraySegment<Decimal>>();
            VerifyHelper.VerifyEntityWithArray1(val);
        }

        [Fact]
        public void GuidArraySegmentFormatter_FormatterTypeIsArray1TypeAndFormatterIsCorrectly()
        {
            var val = RandomHelper.RandomValue<ArraySegment<Guid>>();
            VerifyHelper.VerifyEntityWithArray1(val);
        }
    }
}
