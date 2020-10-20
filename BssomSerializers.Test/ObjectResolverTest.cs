using BssomSerializers.Formatters;
using BssomSerializers.Resolver;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Xunit;
namespace BssomSerializers.Test
{
    public class ObjectResolverTest
    {
        [Fact]
        public void ResolverGetFormatter_ObjectTypeFormatter_CanBeObtainedNormally()
        {
            ObjectResolver.Instance.GetFormatter<object>().Is(ObjectFormatter.Instance);
        }

        [Fact]
        public void ObjectType_FormatterValueType_IsNull()
        {
            var buf = BssomSerializer.Serialize<object>(null);
            BssomSerializer.Deserialize<object>(buf, BssomSerializerOptions.Default.WithIsPriorityToDeserializeObjectAsBssomValue(false)).IsNull();
            BssomSerializer.Deserialize<object>(buf, BssomSerializerOptions.Default.WithIsPriorityToDeserializeObjectAsBssomValue(true)).Is(BssomNull.Value);
        }

        [Fact]
        public void ObjectType_FormatterValueType_IsNumber()
        {
            VerifyHelper.ConvertObjectAndVerifyBssomValueType(RandomHelper.RandomValue<sbyte>()).Is(BssomValueType.Number);
            VerifyHelper.ConvertObjectAndVerifyBssomValueType(RandomHelper.RandomValue<Int16>()).Is(BssomValueType.Number);
            VerifyHelper.ConvertObjectAndVerifyBssomValueType(RandomHelper.RandomValue<int>()).Is(BssomValueType.Number);
            VerifyHelper.ConvertObjectAndVerifyBssomValueType(RandomHelper.RandomValue<long>()).Is(BssomValueType.Number);
            VerifyHelper.ConvertObjectAndVerifyBssomValueType(RandomHelper.RandomValue<byte>()).Is(BssomValueType.Number);
            VerifyHelper.ConvertObjectAndVerifyBssomValueType(RandomHelper.RandomValue<UInt16>()).Is(BssomValueType.Number);
            VerifyHelper.ConvertObjectAndVerifyBssomValueType(RandomHelper.RandomValue<UInt32>()).Is(BssomValueType.Number);
            VerifyHelper.ConvertObjectAndVerifyBssomValueType(RandomHelper.RandomValue<ulong>()).Is(BssomValueType.Number);
        }

        [Fact]
        public void ObjectType_FormatterValueType_IsFloat()
        {
            VerifyHelper.ConvertObjectAndVerifyBssomValueType(RandomHelper.RandomValue<float>()).Is(BssomValueType.Float);
            VerifyHelper.ConvertObjectAndVerifyBssomValueType(RandomHelper.RandomValue<double>()).Is(BssomValueType.Float);
        }

        [Fact]
        public void ObjectType_FormatterValueType_IsBoolean()
        {
            VerifyHelper.ConvertObjectAndVerifyBssomValueType(RandomHelper.RandomValue<bool>()).Is(BssomValueType.Boolean);
        }

        [Fact]
        public void ObjectType_FormatterValueType_IsDateTime()
        {
            VerifyHelper.ConvertObjectAndVerifyBssomValueType(RandomHelper.RandomValue<DateTime>()).Is(BssomValueType.DateTime);
        }

        [Fact]
        public void ObjectType_FormatterValueType_IsString()
        {
            VerifyHelper.ConvertObjectAndVerifyBssomValueType(RandomHelper.RandomValue<String>()).Is(BssomValueType.String);
        }

        [Fact]
        public void ObjectType_FormatterValueType_IsMap()
        {
            var dict = new Dictionary<string, string>() { { "a", "1" } };
            VerifyHelper.ConvertObjectAndVerifyBssomMapType<string, string>(dict, BssomSerializerOptions.Default.WithIDictionaryIsSerializeMap1Type(true));

            VerifyHelper.ConvertObjectAndVerifyBssomMapType<string, string>(RandomHelper.RandomValueWithOutStringEmpty<Dictionary<string, string>>(), BssomSerializerOptions.Default.WithIDictionaryIsSerializeMap1Type(true)).Is(BssomValueType.Map);
            VerifyHelper.ConvertObjectAndVerifyBssomMapType<string, string>(RandomHelper.RandomValueWithOutStringEmpty<Dictionary<string, string>>(), BssomSerializerOptions.Default.WithIDictionaryIsSerializeMap1Type(false)).Is(BssomValueType.Map);
        }

        [Fact]
        public void ObjectType_FormatterValueType_IsArray()
        {
            VerifyHelper.ConvertArrayObjectAndVerifyBssomValueType(RandomHelper.RandomValue<List<int>>()).Is(BssomValueType.Array);
            VerifyHelper.ConvertArrayObjectAndVerifyBssomValueType(RandomHelper.RandomValue<List<object>>()).Is(BssomValueType.Array);
        }

        [Fact]
        public void CustomType_ObjectFormatterValueType_IsMap()
        {
            VerifyHelper.ConvertObjectAndVerifyEntity(RandomHelper.RandomValue<_class1>());
            VerifyHelper.ConvertObjectAndVerifyEntity(RandomHelper.RandomValue<_struct1>());
        }

        public class _class1 { public int Age; }
        public struct _struct1 { public int Age; }
    }
}
