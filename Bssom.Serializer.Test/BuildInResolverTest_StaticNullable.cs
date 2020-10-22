using Bssom.Serializer.BssMap;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Text;
using Bssom.Serializer.Binary;
using Bssom.Serializer.Formatters;
using Bssom.Serializer.Resolver;
using Xunit;

namespace Bssom.Serializer.Test
{
    public class BuildInResolverTest_StaticNullable
    {
        [Fact]
        public void ResolverGetFormatter_StaticNullableInt8Formatter_CanBeObtainedNormally()
        {
            BuildInResolver.Instance.GetFormatter<sbyte?>().Is(StaticNullableInt8Formatter.Instance);
        }

        [Fact]
        public void ResolverGetFormatter_StaticNullableInt16Formatter_CanBeObtainedNormally()
        {
            BuildInResolver.Instance.GetFormatter<Int16?>().Is(StaticNullableInt16Formatter.Instance);
        }

        [Fact]
        public void ResolverGetFormatter_StaticNullableInt32Formatter_CanBeObtainedNormally()
        {
            BuildInResolver.Instance.GetFormatter<Int32?>().Is(StaticNullableInt32Formatter.Instance);
        }

        [Fact]
        public void ResolverGetFormatter_StaticNullableInt64Formatter_CanBeObtainedNormally()
        {
            BuildInResolver.Instance.GetFormatter<Int64?>().Is(StaticNullableInt64Formatter.Instance);
        }

        [Fact]
        public void ResolverGetFormatter_StaticNullableUInt8Formatter_CanBeObtainedNormally()
        {
            BuildInResolver.Instance.GetFormatter<byte?>().Is(StaticNullableUInt8Formatter.Instance);
        }

        [Fact]
        public void ResolverGetFormatter_StaticNullableUInt16Formatter_CanBeObtainedNormally()
        {
            BuildInResolver.Instance.GetFormatter<UInt16?>().Is(StaticNullableUInt16Formatter.Instance);
        }

        [Fact]
        public void ResolverGetFormatter_StaticNullableUInt32Formatter_CanBeObtainedNormally()
        {
            BuildInResolver.Instance.GetFormatter<UInt32?>().Is(StaticNullableUInt32Formatter.Instance);
        }

        [Fact]
        public void ResolverGetFormatter_StaticNullableUInt64Formatter_CanBeObtainedNormally()
        {
            BuildInResolver.Instance.GetFormatter<UInt64?>().Is(StaticNullableUInt64Formatter.Instance);
        }

        [Fact]
        public void ResolverGetFormatter_StaticNullableFloat32Formatter_CanBeObtainedNormally()
        {
            BuildInResolver.Instance.GetFormatter<Single?>().Is(StaticNullableFloat32Formatter.Instance);
        }

        [Fact]
        public void ResolverGetFormatter_StaticNullableFloat64Formatter_CanBeObtainedNormally()
        {
            BuildInResolver.Instance.GetFormatter<Double?>().Is(StaticNullableFloat64Formatter.Instance);
        }

        [Fact]
        public void ResolverGetFormatter_StaticNullableBooleanFormatter_CanBeObtainedNormally()
        {
            BuildInResolver.Instance.GetFormatter<bool?>().Is(StaticNullableBooleanFormatter.Instance);
        }

        [Fact]
        public void ResolverGetFormatter_StaticNullableCharFormatter_CanBeObtainedNormally()
        {
            BuildInResolver.Instance.GetFormatter<char?>().Is(StaticNullableCharFormatter.Instance);
        }

        [Fact]
        public void ResolverGetFormatter_StaticNullableGuidFormatter_CanBeObtainedNormally()
        {
            BuildInResolver.Instance.GetFormatter<Guid?>().Is(StaticNullableGuidFormatter.Instance);
        }

        [Fact]
        public void ResolverGetFormatter_StaticNullableDecimalFormatter_CanBeObtainedNormally()
        {
            BuildInResolver.Instance.GetFormatter<Decimal?>().Is(StaticNullableDecimalFormatter.Instance);
        }


        [Fact]
        public void StaticNullableInt8Formatter_Formatter_IsCorrectly()
        {
            VerifyHelper.VerifySimpleType<sbyte>(default, BssomBinaryPrimitives.BuildInTypeCodeSize + BssomBinaryPrimitives.Int8Size);
            VerifyHelper.VerifyNullableIsNull<sbyte>();
        }

        [Fact]
        public void StaticNullableInt16Formatter_Formatter_IsCorrectly()
        {
            VerifyHelper.VerifySimpleType<Int16>(default, BssomBinaryPrimitives.BuildInTypeCodeSize + BssomBinaryPrimitives.Int16Size);
            VerifyHelper.VerifyNullableIsNull<Int16>();
        }

        [Fact]
        public void StaticNullableInt32Formatter_Formatter_IsCorrectly()
        {
            VerifyHelper.VerifySimpleType<Int32>(default, BssomBinaryPrimitives.BuildInTypeCodeSize + BssomBinaryPrimitives.Int32Size);
            VerifyHelper.VerifyNullableIsNull<Int32>();
        }

        [Fact]
        public void StaticNullableInt64Formatter_Formatter_IsCorrectly()
        {
            VerifyHelper.VerifySimpleType<Int64>(default, BssomBinaryPrimitives.BuildInTypeCodeSize + BssomBinaryPrimitives.Int64Size);
            VerifyHelper.VerifyNullableIsNull<Int64>();
        }

        [Fact]
        public void StaticNullableUInt8Formatter_Formatter_IsCorrectly()
        {
            VerifyHelper.VerifySimpleType<byte>(default, BssomBinaryPrimitives.BuildInTypeCodeSize + BssomBinaryPrimitives.UInt8Size);
            VerifyHelper.VerifyNullableIsNull<byte>();
        }

        [Fact]
        public void StaticNullableUInt16Formatter_Formatter_IsCorrectly()
        {
            VerifyHelper.VerifySimpleType<UInt16>(default, BssomBinaryPrimitives.BuildInTypeCodeSize + BssomBinaryPrimitives.UInt16Size);
            VerifyHelper.VerifyNullableIsNull<UInt16>();
        }

        [Fact]
        public void StaticNullableUInt32Formatter_Formatter_IsCorrectly()
        {
            VerifyHelper.VerifySimpleType<UInt32>(default, BssomBinaryPrimitives.BuildInTypeCodeSize + BssomBinaryPrimitives.UInt32Size);
            VerifyHelper.VerifyNullableIsNull<UInt32>();
        }

        [Fact]
        public void StaticNullableUInt64Formatter_Formatter_IsCorrectly()
        {
            VerifyHelper.VerifySimpleType<UInt64>(default, BssomBinaryPrimitives.BuildInTypeCodeSize + BssomBinaryPrimitives.UInt64Size);
            VerifyHelper.VerifyNullableIsNull<UInt64>();
        }

        [Fact]
        public void StaticNullableFloat32Formatter_Formatter_IsCorrectly()
        {
            VerifyHelper.VerifySimpleType<Single>(default, BssomBinaryPrimitives.BuildInTypeCodeSize + BssomBinaryPrimitives.Float32Size);
            VerifyHelper.VerifyNullableIsNull<Single>();
        }

        [Fact]
        public void StaticNullableFloat64Formatter_Formatter_IsCorrectly()
        {
            VerifyHelper.VerifySimpleType<Double>(default, BssomBinaryPrimitives.BuildInTypeCodeSize + BssomBinaryPrimitives.Float64Size);
            VerifyHelper.VerifyNullableIsNull<Double>();
        }

        [Fact]
        public void StaticNullableBooleanFormatter_Formatter_IsCorrectly()
        {
            VerifyHelper.VerifySimpleType<Boolean>(default, BssomBinaryPrimitives.BuildInTypeCodeSize + BssomBinaryPrimitives.BooleanSize);
            VerifyHelper.VerifyNullableIsNull<Boolean>();
        }

        [Fact]
        public void StaticNullableCharFormatter_Formatter_IsCorrectly()
        {
            VerifyHelper.VerifySimpleType<Char>(default, BssomBinaryPrimitives.NativeTypeCodeSize + BssomBinaryPrimitives.CharSize);
            VerifyHelper.VerifyNullableIsNull<Char>();
        }


        [Fact]
        public void StaticNullableGuidFormatter_NativeFormatter_IsCorrectly()
        {
            VerifyHelper.VerifySimpleType<Guid>(default, BssomBinaryPrimitives.NativeTypeCodeSize + BssomBinaryPrimitives.GuidSize);
            VerifyHelper.VerifyNullableIsNull<Guid>();
        }

        [Fact]
        public void StaticNullableDecimalFormatter_Formatter_IsCorrectly()
        {
            VerifyHelper.VerifySimpleType<decimal>(default, BssomBinaryPrimitives.NativeTypeCodeSize + BssomBinaryPrimitives.DecimalSize);
            VerifyHelper.VerifyNullableIsNull<decimal>();
        }
    }
}
