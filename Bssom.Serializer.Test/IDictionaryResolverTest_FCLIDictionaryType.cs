using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Bssom.Serializer.Resolvers;
using Xunit;

namespace Bssom.Serializer.Test
{
    public class IDictionaryResolverTest_FCLIDictionaryType
    {
        [Fact]
        public void ResolverGetFormatter_HashtableType_IsDynamicFormatterType()
        {
            IDictionaryResolver.Instance.GetFormatter<Hashtable>().IsNotNull();
        }

        [Fact]
        public void ResolverGetFormatter_DictionaryType_IsDynamicFormatterType()
        {
            IDictionaryResolver.Instance.GetFormatter<Dictionary<string, string>>().IsNotNull();
        }

        [Fact]
        public void ResolverGetFormatter_ConcurrentDictionaryType_IsDynamicFormatterType()
        {
            IDictionaryResolver.Instance.GetFormatter<ConcurrentDictionary<string, string>>().IsNotNull();
        }

        [Fact]
        public void ResolverGetFormatter_SortedListType_IsDynamicFormatterType()
        {
            IDictionaryResolver.Instance.GetFormatter<SortedList<string, string>>().IsNotNull();
        }


        [Fact]
        public void Hashtable_Map1Type_FormatterIsCorrectly()
        {
            Hashtable val = new Hashtable();
            VerifyHelper.VerifyIDictAndReturnSerializeBytes(val, BssomSerializerOptions.Default.WithIDictionaryIsSerializeMap1Type(true)).IsMap1();

            Hashtable va2 = new Hashtable(RandomHelper.RandomValueWithOutStringEmpty<Dictionary<string, string>>());
            VerifyHelper.VerifyIDictAndReturnSerializeBytes(va2, BssomSerializerOptions.Default.WithIDictionaryIsSerializeMap1Type(true)).IsMap1();

            Hashtable val3 = new Hashtable(RandomHelper.RandomValueWithOutStringEmpty<Dictionary<object, string>>());
            VerifyHelper.VerifyIDictAndReturnSerializeBytes(val3, BssomSerializerOptions.Default.WithIDictionaryIsSerializeMap1Type(true)).IsMap1();
        }

        [Fact]
        public void Hashtable_Map2Type_FormatterIsCorrectly()
        {
            Hashtable val = new Hashtable();
            VerifyHelper.VerifyIDictWithMap2Type(val, BssomSerializerOptions.Default.WithIDictionaryIsSerializeMap1Type(false));

            Hashtable val2 = new Hashtable(RandomHelper.RandomValueWithOutStringEmpty<Dictionary<string, string>>());
            VerifyHelper.VerifyIDictWithMap2Type(val2, BssomSerializerOptions.Default.WithIDictionaryIsSerializeMap1Type(false));

            Hashtable val3 = new Hashtable(RandomHelper.RandomValueWithOutStringEmpty<Dictionary<object, string>>());
            VerifyHelper.VerifyIDictWithMap2Type(val3, BssomSerializerOptions.Default.WithIDictionaryIsSerializeMap1Type(false));
        }

        [Fact]
        public void Dictionary_Map1Type_FormatterIsCorrectly()
        {
            Dictionary<string, string> val = RandomHelper.RandomValueWithOutStringEmpty<Dictionary<string, string>>();
            VerifyHelper.VerifyMap1Type(val, BssomSerializerOptions.Default.WithIDictionaryIsSerializeMap1Type(true));

            Dictionary<object, string> val2 = RandomHelper.RandomValueWithOutStringEmpty<Dictionary<object, string>>();
            VerifyHelper.VerifyMap1Type(val2, BssomSerializerOptions.Default.WithIDictionaryIsSerializeMap1Type(true));

            Dictionary<string, string> val3 = new Dictionary<string, string>();
            VerifyHelper.VerifyMap1Type(val3, BssomSerializerOptions.Default.WithIDictionaryIsSerializeMap1Type(true));
        }

        [Fact]
        public void Dictionary_Map2Type_FormatterIsCorrectly()
        {
            Dictionary<string, string> val = RandomHelper.RandomValueWithOutStringEmpty<Dictionary<string, string>>();
            VerifyHelper.VerifyIDictWithMap2Type<Dictionary<string, string>,string,string>(val, BssomSerializerOptions.Default.WithIDictionaryIsSerializeMap1Type(false));

            Dictionary<object, string> val2 = RandomHelper.RandomValueWithOutStringEmpty<Dictionary<object, string>>();
            VerifyHelper.VerifyIDictWithMap2Type<Dictionary<object, string>, object, string>(val2, BssomSerializerOptions.Default.WithIDictionaryIsSerializeMap1Type(false));

            Dictionary<string, string> val3 = new Dictionary<string, string>();
            VerifyHelper.VerifyIDictWithMap2Type<Dictionary<string, string>, string, string>(val3, BssomSerializerOptions.Default.WithIDictionaryIsSerializeMap1Type(false));
        }

        [Fact]
        public void ConcurrentDictionary_Map1Type_FormatterIsCorrectly()
        {
            ConcurrentDictionary<string, string> val = new ConcurrentDictionary<string, string>(RandomHelper.RandomValueWithOutStringEmpty<Dictionary<string, string>>());
            VerifyHelper.VerifyIDictAndReturnSerializeBytes<ConcurrentDictionary<string, string>,string,string>(val, BssomSerializerOptions.Default.WithIDictionaryIsSerializeMap1Type(true)).IsMap1();

            ConcurrentDictionary<object, string> val2 = new ConcurrentDictionary<object, string>(RandomHelper.RandomValueWithOutStringEmpty<Dictionary<object, string>>());
            VerifyHelper.VerifyIDictAndReturnSerializeBytes<ConcurrentDictionary<object, string>, object, string>(val2, BssomSerializerOptions.Default.WithIDictionaryIsSerializeMap1Type(true)).IsMap1();

            ConcurrentDictionary<string, string> val3 = new ConcurrentDictionary<string, string>();
            VerifyHelper.VerifyIDictAndReturnSerializeBytes<ConcurrentDictionary<string, string>, string, string>(val3, BssomSerializerOptions.Default.WithIDictionaryIsSerializeMap1Type(true)).IsMap1();
        }

        [Fact]
        public void ConcurrentDictionary_Map2Type_FormatterIsCorrectly()
        {
            ConcurrentDictionary<string, string> val = new ConcurrentDictionary<string, string>(RandomHelper.RandomValueWithOutStringEmpty<Dictionary<string, string>>());
            VerifyHelper.VerifyIDictWithMap2Type<ConcurrentDictionary<string, string>, string, string>(val, BssomSerializerOptions.Default.WithIDictionaryIsSerializeMap1Type(false));

            ConcurrentDictionary<object, string> val2 = new ConcurrentDictionary<object, string>(RandomHelper.RandomValueWithOutStringEmpty<Dictionary<object, string>>());
            VerifyHelper.VerifyIDictWithMap2Type<ConcurrentDictionary<object, string>, object, string>(val2, BssomSerializerOptions.Default.WithIDictionaryIsSerializeMap1Type(false));

            ConcurrentDictionary<string, string> val3 = new ConcurrentDictionary<string, string>();
            VerifyHelper.VerifyIDictWithMap2Type<ConcurrentDictionary<string, string>, string, string>(val3, BssomSerializerOptions.Default.WithIDictionaryIsSerializeMap1Type(false));
        }

        [Fact]
        public void SortedList_Map1Type_FormatterIsCorrectly()
        {
            SortedList<string, string> val = new SortedList<string, string>(RandomHelper.RandomValueWithOutStringEmpty<Dictionary<string, string>>());
            VerifyHelper.VerifyMap1Type(val, BssomSerializerOptions.Default.WithIDictionaryIsSerializeMap1Type(true));

            SortedList<string, string> val2 = new SortedList<string, string>();
            VerifyHelper.VerifyMap1Type(val2, BssomSerializerOptions.Default.WithIDictionaryIsSerializeMap1Type(true));

        }

        [Fact]
        public void SortedList_Map2Type_FormatterIsCorrectly()
        {
            SortedList<string, string> val = new SortedList<string, string>(RandomHelper.RandomValueWithOutStringEmpty<Dictionary<string, string>>());
            VerifyHelper.VerifyIDictWithMap2Type<SortedList<string, string>, string, string>(val, BssomSerializerOptions.Default.WithIDictionaryIsSerializeMap1Type(false));

            SortedList<string, string> val2 = new SortedList<string, string>();
            VerifyHelper.VerifyIDictWithMap2Type<SortedList<string, string>, string, string>(val, BssomSerializerOptions.Default.WithIDictionaryIsSerializeMap1Type(false));
        }
    }
}
