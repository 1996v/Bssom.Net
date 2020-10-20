using BssomSerializers.Formatters;
using BssomSerializers.Resolver;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xunit;

namespace BssomSerializers.Test
{

    public class IDictionaryResolverTest_Interface
    {
        [Fact]
        public void ResolverGetFormatter_IDictionaryFormatter_CanBeObtainedNormally()
        {
            IDictionaryResolver.Instance.GetFormatter<IDictionary>().Is(IDictionaryFormatter.Instance);
        }

        [Fact]
        public void ResolverGetFormatter_IGenericDictionaryType_IsDynamicFormatterType()
        {
            IDictionaryResolver.Instance.GetFormatter<IDictionary<string, string>>().IsNotNull();
        }

        [Fact]
        public void ResolverGetFormatter_IReadOnlyDictionaryType_IsDynamicFormatterType()
        {
            IDictionaryResolver.Instance.GetFormatter<IReadOnlyDictionary<string, string>>().IsNotNull();
        }

        [Fact]
        public void IDictionaryType_DeserializeTypeIsGenericDictionaryObject()
        {
            IDictionary val = RandomHelper.RandomValue<Dictionary<int, int>>();
            var buf = BssomSerializer.Serialize(val);
            BssomSerializer.Deserialize<IDictionary>(buf).IsType<Dictionary<object, object>>();
        }

        [Fact]
        public void IGenericDictionaryType_DeserializeTypeIsGenericDictionary()
        {
            IDictionary<int, int> val = RandomHelper.RandomValue<Dictionary<int, int>>();
            var buf = BssomSerializer.Serialize(val);
            BssomSerializer.Deserialize<IDictionary<int, int>>(buf).IsType<Dictionary<int, int>>();
        }

        [Fact]
        public void IReadOnlyDictionaryType_DeserializeTypeIsReadOnlyDictionaryDictionary()
        {
            IReadOnlyDictionary<int, int> val = RandomHelper.RandomValue<Dictionary<int, int>>();
            var buf = BssomSerializer.Serialize(val);
            BssomSerializer.Deserialize<IReadOnlyDictionary<int, int>>(buf).IsType<ReadOnlyDictionary<int, int>>();
        }

        [Fact]
        public void IDictionaryFormatter_Map1Type_FormatterIsCorrectly()
        {
            IDictionary val = RandomHelper.RandomValueWithOutStringEmpty<Dictionary<string, string>>();
            VerifyHelper.VerifyIDictAndReturnSerializeBytes(val, BssomSerializerOptions.Default.WithIDictionaryIsSerializeMap1Type(true)).IsMap1();

            IDictionary val2 = RandomHelper.RandomValueWithOutStringEmpty<Dictionary<object, string>>();
            VerifyHelper.VerifyIDictAndReturnSerializeBytes(val2, BssomSerializerOptions.Default.WithIDictionaryIsSerializeMap1Type(true)).IsMap1();
        }

        [Fact]
        public void IDictionaryFormatter_Map2Type_FormatterIsCorrectly()
        {
            IDictionary val = RandomHelper.RandomValueWithOutStringEmpty<Dictionary<string, string>>();
            VerifyHelper.VerifyIDictWithMap2Type(val, BssomSerializerOptions.Default.WithIDictionaryIsSerializeMap1Type(false));

            IDictionary val2 = RandomHelper.RandomValueWithOutStringEmpty<Dictionary<object, string>>();
            VerifyHelper.VerifyIDictWithMap2Type(val2, BssomSerializerOptions.Default.WithIDictionaryIsSerializeMap1Type(false));
        }

        [Fact]
        public void IGenericDictionaryFormatter_Map1Type_FormatterIsCorrectly()
        {
            IDictionary<string, string> val = RandomHelper.RandomValueWithOutStringEmpty<Dictionary<string, string>>();
            VerifyHelper.VerifyMap1Type(val, BssomSerializerOptions.Default.WithIDictionaryIsSerializeMap1Type(true));

            IDictionary<object, string> val2 = RandomHelper.RandomValueWithOutStringEmpty<Dictionary<object, string>>();
            VerifyHelper.VerifyMap1Type(val2, BssomSerializerOptions.Default.WithIDictionaryIsSerializeMap1Type(true));
        }

        [Fact]
        public void IGenericDictionaryFormatter_Map2Type_FormatterIsCorrectly()
        {
            IDictionary<string, string> val = RandomHelper.RandomValueWithOutStringEmpty<Dictionary<string, string>>();
            VerifyHelper.VerifyIDictWithMap2Type<IDictionary<string, string>, string, string>(val, BssomSerializerOptions.Default.WithIDictionaryIsSerializeMap1Type(false));

            IDictionary<object, string> val2 = RandomHelper.RandomValueWithOutStringEmpty<Dictionary<object, string>>();
            VerifyHelper.VerifyIDictWithMap2Type<IDictionary<object, string>, object, string>(val2, BssomSerializerOptions.Default.WithIDictionaryIsSerializeMap1Type(false));
        }

        [Fact]
        public void IReadOnlyDictionaryFormatter_Map1Type_FormatterIsCorrectly()
        {
            IReadOnlyDictionary<string, string> val = RandomHelper.RandomValueWithOutStringEmpty<Dictionary<string, string>>();
            VerifyHelper.VerifyMap1Type(val, BssomSerializerOptions.Default.WithIDictionaryIsSerializeMap1Type(true));

            IReadOnlyDictionary<object, string> val2 = RandomHelper.RandomValueWithOutStringEmpty<Dictionary<object, string>>();
            VerifyHelper.VerifyMap1Type(val2, BssomSerializerOptions.Default.WithIDictionaryIsSerializeMap1Type(true));
        }

        [Fact]
        public void IReadOnlyDictionaryFormatter_Map2Type_FormatterIsCorrectly()
        {
            IReadOnlyDictionary<string, string> val = RandomHelper.RandomValueWithOutStringEmpty<Dictionary<string, string>>();
            VerifyHelper.VerifyIDictWithMap2Type<IReadOnlyDictionary<string, string>,string,string>(val, BssomSerializerOptions.Default.WithIDictionaryIsSerializeMap1Type(false));

            IReadOnlyDictionary<object, string> val2 = RandomHelper.RandomValueWithOutStringEmpty<Dictionary<object, string>>();
            VerifyHelper.VerifyIDictWithMap2Type<IReadOnlyDictionary<object, string>, object, string>(val2, BssomSerializerOptions.Default.WithIDictionaryIsSerializeMap1Type(false));
        }
    }
}
