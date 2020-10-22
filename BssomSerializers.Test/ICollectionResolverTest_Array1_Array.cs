using Bssom.Serializer.BssomBuffer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using Bssom.Serializer.Formatters;
using Bssom.Serializer.Resolver;
using Xunit;

namespace Bssom.Serializer.Test
{
    public class ICollectionResolverTest_Array1_Array
    {
        [Fact]
        public void ResolverGetFormatter_Int16ArrayFormatter_CanBeObtainedNormally()
        {
            ICollectionResolver.Instance.GetFormatter<Int16[]>().Is(Int16ArrayFormatter.Instance);
        }

        [Fact]
        public void ResolverGetFormatter_Int32ArrayFormatter_CanBeObtainedNormally()
        {
            ICollectionResolver.Instance.GetFormatter<Int32[]>().Is(Int32ArrayFormatter.Instance);
        }

        [Fact]
        public void ResolverGetFormatter_Int64ArrayFormatter_CanBeObtainedNormally()
        {
            ICollectionResolver.Instance.GetFormatter<Int64[]>().Is(Int64ArrayFormatter.Instance);
        }

        [Fact]
        public void ResolverGetFormatter_UInt16ArrayFormatter_CanBeObtainedNormally()
        {
            ICollectionResolver.Instance.GetFormatter<UInt16[]>().Is(UInt16ArrayFormatter.Instance);
        }

        [Fact]
        public void ResolverGetFormatter_UInt32ArrayFormatter_CanBeObtainedNormally()
        {
            ICollectionResolver.Instance.GetFormatter<UInt32[]>().Is(UInt32ArrayFormatter.Instance);
        }

        [Fact]
        public void ResolverGetFormatter_UInt64ArrayFormatter_CanBeObtainedNormally()
        {
            ICollectionResolver.Instance.GetFormatter<UInt64[]>().Is(UInt64ArrayFormatter.Instance);
        }

        [Fact]
        public void ResolverGetFormatter_Float32ArrayFormatter_CanBeObtainedNormally()
        {
            ICollectionResolver.Instance.GetFormatter<Single[]>().Is(Float32ArrayFormatter.Instance);
        }

        [Fact]
        public void ResolverGetFormatter_Float64ArrayFormatter_CanBeObtainedNormally()
        {
            ICollectionResolver.Instance.GetFormatter<Double[]>().Is(Float64ArrayFormatter.Instance);
        }

        [Fact]
        public void ResolverGetFormatter_BooleanArrayFormatter_CanBeObtainedNormally()
        {
            ICollectionResolver.Instance.GetFormatter<bool[]>().Is(BooleanArrayFormatter.Instance);
        }

        [Fact]
        public void ResolverGetFormatter_UInt8ArrayFormatter_CanBeObtainedNormally()
        {
            ICollectionResolver.Instance.GetFormatter<byte[]>().Is(UInt8ArrayFormatter.Instance);
        }

        [Fact]
        public void ResolverGetFormatter_Int8ArrayFormatter_CanBeObtainedNormally()
        {
            ICollectionResolver.Instance.GetFormatter<sbyte[]>().Is(Int8ArrayFormatter.Instance);
        }

        [Fact]
        public void ResolverGetFormatter_CharArrayFormatter_CanBeObtainedNormally()
        {
            ICollectionResolver.Instance.GetFormatter<char[]>().Is(CharArrayFormatter.Instance);
        }

        [Fact]
        public void ResolverGetFormatter_DateTimeArrayFormatter_CanBeObtainedNormally()
        {
            ICollectionResolver.Instance.GetFormatter<DateTime[]>().Is(DateTimeArrayFormatter.Instance);
        }

        [Fact]
        public void ResolverGetFormatter_DecimalArrayFormatter_CanBeObtainedNormally()
        {
            ICollectionResolver.Instance.GetFormatter<Decimal[]>().Is(DecimalArrayFormatter.Instance);
        }

        [Fact]
        public void ResolverGetFormatter_GuidArrayFormatter_CanBeObtainedNormally()
        {
            ICollectionResolver.Instance.GetFormatter<Guid[]>().Is(GuidArrayFormatter.Instance);
        }

        [Fact]
        public void Int8ArrayFormatter_FormatterTypeIsArray1TypeAndFormatterIsCorrectly()
        {
            var val = RandomHelper.RandomValue<sbyte[]>();
            VerifyHelper.VerifyEntityWithArray1(val);
        }

        [Fact]
        public void Int16ArrayFormatter_FormatterTypeIsArray1TypeAndFormatterIsCorrectly()
        {
            var val = RandomHelper.RandomValue<Int16[]>();
            VerifyHelper.VerifyEntityWithArray1(val);
        }

        [Fact]
        public void Int32ArrayFormatter_FormatterTypeIsArray1TypeAndFormatterIsCorrectly()
        {
            var val = RandomHelper.RandomValue<Int32[]>();
            VerifyHelper.VerifyEntityWithArray1(val);
        }

        [Fact]
        public void Int64ArrayFormatter_FormatterTypeIsArray1TypeAndFormatterIsCorrectly()
        {
            var val = RandomHelper.RandomValue<Int64[]>();
            VerifyHelper.VerifyEntityWithArray1(val);
        }

        [Fact]
        public void UInt8ArrayFormatter_FormatterTypeIsArray1TypeAndFormatterIsCorrectly()
        {
            var val = RandomHelper.RandomValue<byte[]>();
            VerifyHelper.VerifyEntityWithArray1(val);
        }

        [Fact]
        public void UInt16ArrayFormatter_FormatterTypeIsArray1TypeAndFormatterIsCorrectly()
        {
            var val = RandomHelper.RandomValue<UInt16[]>();
            VerifyHelper.VerifyEntityWithArray1(val);
        }

        [Fact]
        public void UInt32ArrayFormatter_FormatterTypeIsArray1TypeAndFormatterIsCorrectly()
        {
            var val = RandomHelper.RandomValue<UInt32[]>();
            VerifyHelper.VerifyEntityWithArray1(val);
        }

        [Fact]
        public void UInt64ArrayFormatter_FormatterTypeIsArray1TypeAndFormatterIsCorrectly()
        {
            var val = RandomHelper.RandomValue<UInt64[]>();
            VerifyHelper.VerifyEntityWithArray1(val);
        }

        [Fact]
        public void Float32ArrayFormatter_FormatterTypeIsArray1TypeAndFormatterIsCorrectly()
        {
            var val = RandomHelper.RandomValue<Single[]>();
            VerifyHelper.VerifyEntityWithArray1(val);
        }

        [Fact]
        public void Float64ArrayFormatter_FormatterTypeIsArray1TypeAndFormatterIsCorrectly()
        {
            var val = RandomHelper.RandomValue<Double[]>();
            VerifyHelper.VerifyEntityWithArray1(val);
        }

        [Fact]
        public void BooleanArrayFormatter_FormatterTypeIsArray1TypeAndFormatterIsCorrectly()
        {
            var val = RandomHelper.RandomValue<bool[]>();
            VerifyHelper.VerifyEntityWithArray1(val);
        }

        [Fact]
        public void CharArrayFormatter_FormatterTypeIsArray1TypeAndFormatterIsCorrectly()
        {
            var val = RandomHelper.RandomValue<Char[]>();
            VerifyHelper.VerifyEntityWithArray1(val);
        }

        [Fact]
        public void DateTimeArrayFormatter_FormatterTypeIsArray1TypeAndFormatterIsCorrectly()
        {
            var val = RandomHelper.RandomValue<DateTime[]>();
            VerifyHelper.VerifyEntityWithArray1(val,BssomSerializerOptions.Default.WithIsUseStandardDateTime(true));
        }

        [Fact]
        public void NativeDateTimeArrayFormatter_FormatterTypeIsArray1TypeAndFormatterIsCorrectly()
        {
            var val = RandomHelper.RandomValue<DateTime[]>();
            VerifyHelper.VerifyEntityWithArray1(val);
        }

        [Fact]
        public void DecimalArrayFormatter_FormatterTypeIsArray1TypeAndFormatterIsCorrectly()
        {
            var val = RandomHelper.RandomValue<Decimal[]>();
            VerifyHelper.VerifyEntityWithArray1(val);
        }

        [Fact]
        public void GuidArrayFormatter_FormatterTypeIsArray1TypeAndFormatterIsCorrectly()
        {
            var val = RandomHelper.RandomValue<Guid[]>();
            VerifyHelper.VerifyEntityWithArray1(val);
        }
    }
}
