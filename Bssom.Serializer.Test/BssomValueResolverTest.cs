using System;
using System.Collections.Generic;
using Bssom.Serializer.Formatters;
using Bssom.Serializer.Resolver;
using Xunit;
namespace Bssom.Serializer.Test
{
    public class BssomValueResolverTest
    {
        [Fact]
        public void ResolverGetFormatter_BssomValueFormatter_CanBeObtainedNormally()
        {
            BssomValueResolver.Instance.GetFormatter<BssomValue>().Is(BssomValueFormatter.Instance);
        }

        [Fact]
        public void ResolverGetFormatter_BssomNumberFormatter_CanBeObtainedNormally()
        {
            BssomValueResolver.Instance.GetFormatter<BssomNumber>().Is(BssomNumberFormatter.Instance);
        }

        [Fact]
        public void ResolverGetFormatter_BssomNullFormatter_CanBeObtainedNormally()
        {
            BssomValueResolver.Instance.GetFormatter<BssomNull>().Is(BssomNullFormatter.Instance);
        }

        [Fact]
        public void ResolverGetFormatter_BssomFloatFormatter_CanBeObtainedNormally()
        {
            BssomValueResolver.Instance.GetFormatter<BssomFloat>().Is(BssomFloatFormatter.Instance);
        }

        [Fact]
        public void ResolverGetFormatter_BssomArrayFormatter_CanBeObtainedNormally()
        {
            BssomValueResolver.Instance.GetFormatter<BssomArray>().Is(BssomArrayFormatter.Instance);
        }

        [Fact]
        public void ResolverGetFormatter_BssomBooleanFormatter_CanBeObtainedNormally()
        {
            BssomValueResolver.Instance.GetFormatter<BssomBoolean>().Is(BssomBooleanFormatter.Instance);
        }

        [Fact]
        public void ResolverGetFormatter_BssomCharFormatter_CanBeObtainedNormally()
        {
            BssomValueResolver.Instance.GetFormatter<BssomChar>().Is(BssomCharFormatter.Instance);
        }

        [Fact]
        public void ResolverGetFormatter_BssomDateTimeFormatter_CanBeObtainedNormally()
        {
            BssomValueResolver.Instance.GetFormatter<BssomDateTime>().Is(BssomDateTimeFormatter.Instance);
        }
        [Fact]
        public void ResolverGetFormatter_BssomDecimalFormatter_CanBeObtainedNormally()
        {
            BssomValueResolver.Instance.GetFormatter<BssomDecimal>().Is(BssomDecimalFormatter.Instance);
        }
        [Fact]
        public void ResolverGetFormatter_BssomGuidFormatter_CanBeObtainedNormally()
        {
            BssomValueResolver.Instance.GetFormatter<BssomGuid>().Is(BssomGuidFormatter.Instance);
        }
        [Fact]
        public void ResolverGetFormatter_BssomMapFormatter_CanBeObtainedNormally()
        {
            BssomValueResolver.Instance.GetFormatter<BssomMap>().Is(BssomMapFormatter.Instance);
        }
        [Fact]
        public void ResolverGetFormatter_BssomStringFormatter_CanBeObtainedNormally()
        {
            BssomValueResolver.Instance.GetFormatter<BssomString>().Is(BssomStringFormatter.Instance);
        }

        [Fact]
        public void BssomNumber_AllBssomNumberType_Formatter_Is_Correctly()
        {
            VerifyHelper.ConvertAndVerifyBssomValueType(new BssomNumber(RandomHelper.RandomValue<sbyte>())).Is(BssomValueType.Number);
            VerifyHelper.ConvertAndVerifyBssomValueType(new BssomNumber(RandomHelper.RandomValue<sbyte>())).Is(BssomValueType.Number);
            VerifyHelper.ConvertAndVerifyBssomValueType(new BssomNumber(RandomHelper.RandomValue<Int16>())).Is(BssomValueType.Number);
            VerifyHelper.ConvertAndVerifyBssomValueType(new BssomNumber(RandomHelper.RandomValue<int>())).Is(BssomValueType.Number);
            VerifyHelper.ConvertAndVerifyBssomValueType(new BssomNumber(RandomHelper.RandomValue<long>())).Is(BssomValueType.Number);
            VerifyHelper.ConvertAndVerifyBssomValueType(new BssomNumber(RandomHelper.RandomValue<byte>())).Is(BssomValueType.Number);
            VerifyHelper.ConvertAndVerifyBssomValueType(new BssomNumber(RandomHelper.RandomValue<UInt16>())).Is(BssomValueType.Number);
            VerifyHelper.ConvertAndVerifyBssomValueType(new BssomNumber(RandomHelper.RandomValue<UInt32>())).Is(BssomValueType.Number);
            VerifyHelper.ConvertAndVerifyBssomValueType(new BssomNumber(RandomHelper.RandomValue<ulong>())).Is(BssomValueType.Number);

        }

        [Fact]
        public void BssomNull_Formatter_Is_Correctly()
        {
            VerifyHelper.ConvertAndVerifyBssomValueType(BssomNull.Value).Is(BssomValueType.Null);
        }

        [Fact]
        public void BssomFloat_AllBssomFloatType_Formatter_Is_Correctly()
        {
            VerifyHelper.ConvertAndVerifyBssomValueType(new BssomFloat(RandomHelper.RandomValue<Single>())).Is(BssomValueType.Float);
            VerifyHelper.ConvertAndVerifyBssomValueType(new BssomFloat(RandomHelper.RandomValue<Double>())).Is(BssomValueType.Float);
        }

        [Fact]
        public void BssomBoolean_Formatter_Is_Correctly()
        {
            VerifyHelper.ConvertAndVerifyBssomValueType(new BssomBoolean(RandomHelper.RandomValue<bool>())).Is(BssomValueType.Boolean);
        }

        [Fact]
        public void BssomChar_Formatter_Is_Correctly()
        {
            VerifyHelper.ConvertAndVerifyBssomValueType(new BssomChar(RandomHelper.RandomValue<Char>())).Is(BssomValueType.Char);
        }

        [Fact]
        public void BssomDateTime_Formatter_Is_Correctly()
        {
            VerifyHelper.ConvertAndVerifyBssomValueType(new BssomDateTime(RandomHelper.RandomValue<DateTime>())).Is(BssomValueType.DateTime);
        }

        [Fact]
        public void BssomDecimal_Formatter_Is_Correctly()
        {
            VerifyHelper.ConvertAndVerifyBssomValueType(new BssomDecimal(RandomHelper.RandomValue<Decimal>())).Is(BssomValueType.Decimal);
        }

        [Fact]
        public void BssomGuid_Formatter_Is_Correctly()
        {
            VerifyHelper.ConvertAndVerifyBssomValueType(new BssomGuid(RandomHelper.RandomValue<Guid>())).Is(BssomValueType.Guid);
        }

        [Fact]
        public void BssomString_Formatter_Is_Correctly()
        {
            VerifyHelper.ConvertAndVerifyBssomValueType(new BssomString(RandomHelper.RandomValue<String>())).Is(BssomValueType.String);
        }

        [Fact]
        public void BssomMap_Formatter_Is_Correctly()
        {
            VerifyHelper.VerifyMap(new BssomMap(RandomHelper.RandomValue<Dictionary<object, object>>()));
        }

        [Fact]
        public void BssomArray_Formatter_Is_Correctly()
        {
            VerifyHelper.VerifyArray(new BssomArray(RandomHelper.RandomValue<List<object>>()));
        
            VerifyHelper.VerifyArray(new BssomArray(RandomHelper.RandomValue<List<bool>>()));
            VerifyHelper.VerifyArray(new BssomArray(RandomHelper.RandomValue<List<sbyte>>()));
            VerifyHelper.VerifyArray(new BssomArray(RandomHelper.RandomValue<List<Int16>>()));
            VerifyHelper.VerifyArray(new BssomArray(RandomHelper.RandomValue<List<int>>()));
            VerifyHelper.VerifyArray(new BssomArray(RandomHelper.RandomValue<List<long>>()));
            VerifyHelper.VerifyArray(new BssomArray(RandomHelper.RandomValue<List<byte>>()));
            VerifyHelper.VerifyArray(new BssomArray(RandomHelper.RandomValue<List<UInt16>>()));
            VerifyHelper.VerifyArray(new BssomArray(RandomHelper.RandomValue<List<UInt32>>()));
            VerifyHelper.VerifyArray(new BssomArray(RandomHelper.RandomValue<List<ulong>>()));
            VerifyHelper.VerifyArray(new BssomArray(RandomHelper.RandomValue<List<float>>()));
            VerifyHelper.VerifyArray(new BssomArray(RandomHelper.RandomValue<List<double>>()));
            VerifyHelper.VerifyArray(new BssomArray(RandomHelper.RandomValue<List<DateTime>>()));
            VerifyHelper.VerifyArray(new BssomArray(RandomHelper.RandomValue<List<char>>()));
            VerifyHelper.VerifyArray(new BssomArray(RandomHelper.RandomValue<List<decimal>>()));
            VerifyHelper.VerifyArray(new BssomArray(RandomHelper.RandomValue<List<Guid>>()));
        }
    }
}
