using System;
using System.Collections.Generic;
using Bssom.Serializer.Resolver;
using Bssom.Serializer.BssMap;
using Bssom.Serializer.Formatters;
using Xunit;

namespace Bssom.Serializer.Test
{
    public class IDictionaryResolverTest_KeyType
    {
        [Fact]
        public void Map_IsNotSupport_CustomStructKey()
        {
            IDictionaryResolver.Instance.GetFormatter<Dictionary<_CustomStruct, string>>().IsNull();
        }

        [Fact]
        public void Map_IsNotSupport_CustomClassKey()
        {
            IDictionaryResolver.Instance.GetFormatter<Dictionary<_CustomClass, string>>().IsNull();
        }

        [Fact]
        public void MapSupportKeyTypes()
        {
            IDictionaryResolver.Instance.GetFormatter<Dictionary<sbyte, string>>().IsNotNull();
            IDictionaryResolver.Instance.GetFormatter<Dictionary<Int16, string>>().IsNotNull();
            IDictionaryResolver.Instance.GetFormatter<Dictionary<Int32, string>>().IsNotNull();
            IDictionaryResolver.Instance.GetFormatter<Dictionary<Int64, string>>().IsNotNull();
            IDictionaryResolver.Instance.GetFormatter<Dictionary<byte, string>>().IsNotNull();
            IDictionaryResolver.Instance.GetFormatter<Dictionary<UInt16, string>>().IsNotNull();
            IDictionaryResolver.Instance.GetFormatter<Dictionary<UInt32, string>>().IsNotNull();
            IDictionaryResolver.Instance.GetFormatter<Dictionary<UInt64, string>>().IsNotNull();
            IDictionaryResolver.Instance.GetFormatter<Dictionary<Char, string>>().IsNotNull();
            IDictionaryResolver.Instance.GetFormatter<Dictionary<Boolean, string>>().IsNotNull();
            IDictionaryResolver.Instance.GetFormatter<Dictionary<Single, string>>().IsNotNull();
            IDictionaryResolver.Instance.GetFormatter<Dictionary<Double, string>>().IsNotNull();
            IDictionaryResolver.Instance.GetFormatter<Dictionary<string, string>>().IsNotNull();
            IDictionaryResolver.Instance.GetFormatter<Dictionary<Guid, string>>().IsNotNull();
            IDictionaryResolver.Instance.GetFormatter<Dictionary<DateTime, string>>().IsNotNull();
            IDictionaryResolver.Instance.GetFormatter<Dictionary<Decimal, string>>().IsNotNull();
            IDictionaryResolver.Instance.GetFormatter<Dictionary<object, string>>().IsNotNull();
        }

        [Fact]
        public void If_MapTypeIsMap2_Then_DateTimeKey_AlwaysUseNativeDateTimeFormatter()
        {
            var val = new Dictionary<object, int>() { { DateTime.MaxValue, 1 }, { DateTime.Now, 2 }, { DateTime.MinValue, 3 } };
            VerifyHelper.VerifyIDictWithMap2Type<Dictionary<object, int>, object, int>(val, BssomSerializerOptions.Default.WithIsUseStandardDateTime(true).WithIDictionaryIsSerializeMap1Type(false));

            var val2 = new Dictionary<DateTime, int>() { { DateTime.MaxValue, 1 } };
            VerifyHelper.VerifyIDictWithMap2Type<Dictionary<DateTime, int>, DateTime, int>(val2, BssomSerializerOptions.Default.WithIsUseStandardDateTime(true).WithIDictionaryIsSerializeMap1Type(false));
        }

        [Fact]
        public void Map2StringKey_Cannot_Empty()
        {
            var val = new Dictionary<string, int>() { { string.Empty, 1 } };
            VerifyHelper.Throws<BssomSerializationArgumentException>(() => BssomSerializer.Serialize(val, BssomSerializerOptions.Default.WithIDictionaryIsSerializeMap1Type(false)));
        }

        [Fact]
        public void Map1StringKey_Cannot_Empty()
        {
            var val = new Dictionary<string, int>() { { string.Empty, 1 } };
            VerifyHelper.Throws<BssomSerializationArgumentException>(() => BssomSerializer.Serialize(val, BssomSerializerOptions.Default.WithIDictionaryIsSerializeMap1Type(false)));
        }

        [Fact]
        public void MapKeyType_ValuesOfSameWidthTypeIsSame_IsThrow()
        {
            {
                var val = new Dictionary<object, int>()
                {
                    {(int)8, 1},
                    {(uint)8, 1}
                };

                VerifyHelper.Throws<BssomSerializationArgumentException>(
                    () => BssomSerializer.Serialize(val,
                        BssomSerializerOptions.Default.WithIDictionaryIsSerializeMap1Type(false)),
                    e => e.ErrorCode == BssomSerializationArgumentException.SerializationErrorCode.BssomMapKeySame);
            }

            {
                var val = new Dictionary<object, int>()
                {
                    {(char)8, 1},
                    {(short)8, 1}
                };

                VerifyHelper.Throws<BssomSerializationArgumentException>(
                    () => BssomSerializer.Serialize(val,
                        BssomSerializerOptions.Default.WithIDictionaryIsSerializeMap1Type(false)),
                    e => e.ErrorCode == BssomSerializationArgumentException.SerializationErrorCode.BssomMapKeySame);
            }

            {
                var val = new Dictionary<object, int>()
                {
                    { (char)8, 1},
                    { ((char)8).ToString(), 1}
                };

                VerifyHelper.Throws<BssomSerializationArgumentException>(
                    () => BssomSerializer.Serialize(val,
                        BssomSerializerOptions.Default.WithIDictionaryIsSerializeMap1Type(false)),
                    e => e.ErrorCode == BssomSerializationArgumentException.SerializationErrorCode.BssomMapKeySame);
            }
        }

        [Fact]
        public void MapKeyType_AllLegalTypes_IsCorrectly()
        {
            var val = new Dictionary<object, int>() {
                    { (sbyte)sbyte.MinValue,1},
                    { (Int16)Int16.MinValue,1},
                    { (Int32)Int32.MinValue,1},
                    { (Int64)Int64.MinValue,1},
                    { (byte)byte.MaxValue,1},
                    { (UInt16)UInt16.MaxValue,1},
                    { (UInt32)UInt32.MaxValue,1},
                    { (UInt64)UInt64.MaxValue,1},
                    { (Char)'0',1},
                    { RandomHelper.RandomValue<Boolean>(),1},
                    { (Single)Single.MaxValue,1},
                    { (Double)Double.MaxValue,1},
                    { RandomHelper.RandomValueWithOutStringEmpty<string>(),1},
                    { RandomHelper.RandomValue<Guid>(),1},
                    { RandomHelper.RandomValue<DateTime>(),1},
                    { RandomHelper.RandomValue<Decimal>(),1},
                };
            VerifyHelper.VerifyIDictWithMap2Type<Dictionary<object, int>, object, int>(val, BssomSerializerOptions.Default.WithIsUseStandardDateTime(true).WithIDictionaryIsSerializeMap1Type(false));
        }

        struct _CustomStruct { }

        struct _CustomClass { }
    }
}
