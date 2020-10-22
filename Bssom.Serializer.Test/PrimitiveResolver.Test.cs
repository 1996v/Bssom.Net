using System;
using System.Text;
using Bssom.Serializer.Binary;
using Bssom.Serializer.Formatters;
using Bssom.Serializer.Resolver;
using Xunit;

namespace Bssom.Serializer.Test
{

    public class PrimitiveResolverTest
    {
        [Fact]
        public void ResolverGetFormatter_Int8Formatter_CanBeObtainedNormally()
        {
            PrimitiveResolver.Instance.GetFormatter<sbyte>().Is(Int8Formatter.Instance);
        }
        [Fact]
        public void ResolverGetFormatter_Int16Formatter_CanBeObtainedNormally()
        {
            PrimitiveResolver.Instance.GetFormatter<Int16>().Is(Int16Formatter.Instance);
        }
        [Fact]
        public void ResolverGetFormatter_Int32Formatter_CanBeObtainedNormally()
        {
            PrimitiveResolver.Instance.GetFormatter<Int32>().Is(Int32Formatter.Instance);
        }
        [Fact]
        public void ResolverGetFormatter_Int64Formatter_CanBeObtainedNormally()
        {
            PrimitiveResolver.Instance.GetFormatter<Int64>().Is(Int64Formatter.Instance);
        }
        [Fact]
        public void ResolverGetFormatter_UInt8Formatter_CanBeObtainedNormally()
        {
            PrimitiveResolver.Instance.GetFormatter<byte>().Is(UInt8Formatter.Instance);
        }
        [Fact]
        public void ResolverGetFormatter_UInt16Formatter_CanBeObtainedNormally()
        {
            PrimitiveResolver.Instance.GetFormatter<UInt16>().Is(UInt16Formatter.Instance);
        }
        [Fact]
        public void ResolverGetFormatter_UInt32Formatter_CanBeObtainedNormally()
        {
            PrimitiveResolver.Instance.GetFormatter<UInt32>().Is(UInt32Formatter.Instance);
        }
        [Fact]
        public void ResolverGetFormatter_UInt64Formatter_CanBeObtainedNormally()
        {
            PrimitiveResolver.Instance.GetFormatter<UInt64>().Is(UInt64Formatter.Instance);
        }
        [Fact]
        public void ResolverGetFormatter_Float32Formatter_CanBeObtainedNormally()
        {
            PrimitiveResolver.Instance.GetFormatter<Single>().Is(Float32Formatter.Instance);
        }
        [Fact]
        public void ResolverGetFormatter_Float64Formatter_CanBeObtainedNormally()
        {
            PrimitiveResolver.Instance.GetFormatter<Double>().Is(Float64Formatter.Instance);
        }
        [Fact]
        public void ResolverGetFormatter_BooleanFormatter_CanBeObtainedNormally()
        {
            PrimitiveResolver.Instance.GetFormatter<Boolean>().Is(BooleanFormatter.Instance);
        }
        [Fact]
        public void ResolverGetFormatter_StringFormatter_CanBeObtainedNormally()
        {
            PrimitiveResolver.Instance.GetFormatter<String>().Is(StringFormatter.Instance);
        }
        [Fact]
        public void ResolverGetFormatter_DateTimeFormatter_CanBeObtainedNormally()
        {
            PrimitiveResolver.Instance.GetFormatter<DateTime>().Is(DateTimeFormatter.Instance);
        }
        [Fact]
        public void ResolverGetFormatter_CharFormatter_CanBeObtainedNormally()
        {
            PrimitiveResolver.Instance.GetFormatter<Char>().Is(CharFormatter.Instance);
        }
        [Fact]
        public void ResolverGetFormatter_GuidFormatter_CanBeObtainedNormally()
        {
            PrimitiveResolver.Instance.GetFormatter<Guid>().Is(GuidFormatter.Instance);
        }
        [Fact]
        public void ResolverGetFormatter_DecimalFormatter_CanBeObtainedNormally()
        {
            PrimitiveResolver.Instance.GetFormatter<Decimal>().Is(DecimalFormatter.Instance);
        }

        [Theory]
        [InlineData(-10)]
        [InlineData(-120)]
        [InlineData(10)]
        [InlineData(0.000006)]
        [InlineData(sbyte.MaxValue)]
        [InlineData(sbyte.MinValue)]
        public void Int8Formatter_Formatter_IsCorrectly(sbyte value)
        {
            VerifyHelper.VerifySimpleType(value, BssomBinaryPrimitives.BuildInTypeCodeSize + BssomBinaryPrimitives.Int8Size);
        }

        [Theory]
        [InlineData(-10)]
        [InlineData(-120)]
        [InlineData(10)]
        [InlineData(0.000006)]
        [InlineData(Int16.MaxValue)]
        [InlineData(Int16.MinValue)]
        public void Int16Formatter_Formatter_IsCorrectly(Int16 value)
        {
            VerifyHelper.VerifySimpleType(value, BssomBinaryPrimitives.BuildInTypeCodeSize + BssomBinaryPrimitives.Int16Size);
        }

        [Theory]
        [InlineData(-10)]
        [InlineData(-120)]
        [InlineData(10)]
        [InlineData(0.000006)]
        [InlineData(Int32.MaxValue)]
        [InlineData(Int32.MinValue)]
        public void Int32Formatter_Formatter_IsCorrectly(Int32 value)
        {
            VerifyHelper.VerifySimpleType(value, BssomBinaryPrimitives.BuildInTypeCodeSize + BssomBinaryPrimitives.Int32Size);
        }

        [Theory]
        [InlineData(-10)]
        [InlineData(-120)]
        [InlineData(10)]
        [InlineData(0.000006)]
        [InlineData(Int64.MaxValue)]
        [InlineData(Int64.MinValue)]
        public void Int64Formatter_Formatter_IsCorrectly(Int64 value)
        {
            VerifyHelper.VerifySimpleType(value, BssomBinaryPrimitives.BuildInTypeCodeSize + BssomBinaryPrimitives.Int64Size);
        }

        [Theory]
        [InlineData(10)]
        [InlineData(0.000006)]
        [InlineData(byte.MaxValue)]
        [InlineData(byte.MinValue)]
        public void UInt8Formatter_Formatter_IsCorrectly(byte value)
        {
            VerifyHelper.VerifySimpleType(value, BssomBinaryPrimitives.BuildInTypeCodeSize + BssomBinaryPrimitives.UInt8Size);
        }

        [Theory]
        [InlineData(10)]
        [InlineData(0.000006)]
        [InlineData(UInt16.MaxValue)]
        [InlineData(UInt16.MinValue)]
        public void UInt16Formatter_Formatter_IsCorrectly(UInt16 value)
        {
            VerifyHelper.VerifySimpleType(value, BssomBinaryPrimitives.BuildInTypeCodeSize + BssomBinaryPrimitives.UInt16Size);
        }

        [Theory]
        [InlineData(10)]
        [InlineData(0.000006)]
        [InlineData(UInt32.MaxValue)]
        [InlineData(UInt32.MinValue)]
        public void UInt32Formatter_Formatter_IsCorrectly(UInt32 value)
        {
            VerifyHelper.VerifySimpleType(value, BssomBinaryPrimitives.BuildInTypeCodeSize + BssomBinaryPrimitives.UInt32Size);
        }

        [Theory]
        [InlineData(10)]
        [InlineData(0.000006)]
        [InlineData(UInt64.MaxValue)]
        [InlineData(UInt64.MinValue)]
        public void UInt64Formatter_Formatter_IsCorrectly(UInt64 value)
        {
            VerifyHelper.VerifySimpleType(value, BssomBinaryPrimitives.BuildInTypeCodeSize + BssomBinaryPrimitives.UInt64Size);
        }

        [Theory]
        [InlineData(-10)]
        [InlineData(-120)]
        [InlineData(10)]
        [InlineData(0.000006)]
        [InlineData(0.333)]
        [InlineData(0.1)]
        [InlineData(4.1234234234533)]
        [InlineData(Single.MaxValue)]
        [InlineData(Single.MinValue)]
        public void Float32Formatter_Formatter_IsCorrectly(Single value)
        {
            VerifyHelper.VerifySimpleType(value, BssomBinaryPrimitives.BuildInTypeCodeSize + BssomBinaryPrimitives.Float32Size);
        }

        [Theory]
        [InlineData(-10)]
        [InlineData(-120)]
        [InlineData(10)]
        [InlineData(0.000006)]
        [InlineData(0.333)]
        [InlineData(0.1)]
        [InlineData(4.1234234234533)]
        [InlineData(Double.MaxValue)]
        [InlineData(Double.MinValue)]
        public void Float64Formatter_Formatter_IsCorrectly(Double value)
        {
            VerifyHelper.VerifySimpleType(value, BssomBinaryPrimitives.BuildInTypeCodeSize + BssomBinaryPrimitives.Float64Size);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void BooleanFormatter_Formatter_IsCorrectly(Boolean value)
        {
            VerifyHelper.VerifySimpleType(value, BssomBinaryPrimitives.BuildInTypeCodeSize + BssomBinaryPrimitives.BooleanSize);
        }

        [Fact]
        public void StringFormatter_Formatter_IsCorrectly()
        {
            for (int i = 0; i < 10; i++)
            {
                var value = RandomHelper.RandomValue<string>();
                int size = BssomBinaryPrimitives.BuildInTypeCodeSize + BssomBinaryPrimitives.StringSize(value);
                VerifyHelper.VerifySimpleType(value, size);
            }
            {
                int size = BssomBinaryPrimitives.BuildInTypeCodeSize + BssomBinaryPrimitives.StringSize(string.Empty);
                VerifyHelper.VerifySimpleType(string.Empty, size);
            }
        }

        [Fact]
        public void StringFormatter_NullFormatter_IsCorrectly()
        {
            VerifyHelper.VerifyTypeNull<string>();
        }

        [Fact]
        public void DateTimeFormatter_StandardFormatter_IsCorrectly()
        {
            for (int i = 0; i < 10; i++)
            {
                var value = RandomHelper.RandomValue<DateTime>();
                VerifyHelper.VerifySimpleType(value,BssomSerializerOptions.Default.WithIsUseStandardDateTime(true));
            }
        }

        [Fact]
        public void DateTimeFormatter_NativeFormatter_IsCorrectly()
        {
            for (int i = 0; i < 10; i++)
            {
                var value = RandomHelper.RandomValue<DateTime>();
                int size = BssomBinaryPrimitives.NativeTypeCodeSize + BssomBinaryPrimitives.NativeDateTimeSize;
                VerifyHelper.VerifySimpleType(value, size, BssomSerializerOptions.Default.WithIsUseStandardDateTime(false));
            }
        }

        [Theory]
        [InlineData(10)]
        [InlineData(' ')]
        [InlineData('s')]
        [InlineData('บบ')]
        [InlineData('\u9930')]
        [InlineData(Char.MaxValue)]
        [InlineData(Char.MinValue)]
        public void CharFormatter_Formatter_IsCorrectly(Char value)
        {
            VerifyHelper.VerifySimpleType(value, BssomBinaryPrimitives.NativeTypeCodeSize + BssomBinaryPrimitives.CharSize);
        }


        [Fact]
        public void GuidFormatter_NativeFormatter_IsCorrectly()
        {
            for (int i = 0; i < 10; i++)
            {
                var value = RandomHelper.RandomValue<Guid>();
                int size = BssomBinaryPrimitives.NativeTypeCodeSize + BssomBinaryPrimitives.GuidSize;
                VerifyHelper.VerifySimpleType(value, size);
            }
        }

        [Fact]
        public void DecimalFormatter_Formatter_IsCorrectly()
        {
            for (int i = 0; i < 10; i++)
            {
                var value = RandomHelper.RandomValue<decimal>();
                int size = BssomBinaryPrimitives.NativeTypeCodeSize + BssomBinaryPrimitives.DecimalSize;
                VerifyHelper.VerifySimpleType(value, size);
            }
        }
    }
}
