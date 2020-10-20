using BssomSerializers.Formatters;
using BssomSerializers.Resolver;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Text;
using Xunit;

namespace BssomSerializers.Test
{
    //BuildIn
    public class BuildInResolverTest_BuildIn
    {
        [Fact]
        public void ResolverGetFormatter_StringDictionaryFormatter_CanBeObtainedNormally()
        {
            BuildInResolver.Instance.GetFormatter<StringDictionary>().Is(StringDictionaryFormatter.Instance);
        }
        [Fact]
        public void ResolverGetFormatter_StringBuilderFormatter_CanBeObtainedNormally()
        {
            BuildInResolver.Instance.GetFormatter<StringBuilder>().Is(StringBuilderFormatter.Instance);
        }
        [Fact]
        public void ResolverGetFormatter_BitArrayFormatter_CanBeObtainedNormally()
        {
            BuildInResolver.Instance.GetFormatter<BitArray>().Is(BitArrayFormatter.Instance);
        }
        [Fact]
        public void ResolverGetFormatter_NameValueCollectionFormatter_CanBeObtainedNormally()
        {
            BuildInResolver.Instance.GetFormatter<NameValueCollection>().Is(NameValueCollectionFormatter.Instance);
        }
        [Fact]
        public void ResolverGetFormatter_VersionFormatter_CanBeObtainedNormally()
        {
            BuildInResolver.Instance.GetFormatter<Version>().Is(VersionFormatter.Instance);
        }
        [Fact]
        public void ResolverGetFormatter_UriFormatter_CanBeObtainedNormally()
        {
            BuildInResolver.Instance.GetFormatter<Uri>().Is(UriFormatter.Instance);
        }
        [Fact]
        public void ResolverGetFormatter_TimeSpanFormatter_CanBeObtainedNormally()
        {
            BuildInResolver.Instance.GetFormatter<TimeSpan>().Is(TimeSpanFormatter.Instance);
        }
        [Fact]
        public void ResolverGetFormatter_DBNullFormatter_CanBeObtainedNormally()
        {
            BuildInResolver.Instance.GetFormatter<DBNull>().Is(DBNullFormatter.Instance);
        }
        [Fact]
        public void ResolverGetFormatter_DataTableFormatter_CanBeObtainedNormally()
        {
            BuildInResolver.Instance.GetFormatter<DataTable>().Is(DataTableFormatter.Instance);
        }

        [Fact]
        public void StringDictionaryFormatter_Map1Formatter_IsCorrectly()
        {
            StringDictionary dict = new StringDictionary();
            var values = RandomHelper.RandomValueWithOutStringEmpty<Dictionary<string, string>>();
            foreach (var item in values)
            {
                dict.Add(item.Key, item.Value);
            }

            VerifyHelper.VerifySpecific(dict, BssomSerializerOptions.Default.WithIDictionaryIsSerializeMap1Type(true));
            VerifyHelper.VerifySize(dict, BssomSerializerOptions.Default.WithIDictionaryIsSerializeMap1Type(true));
        }

        [Fact]
        public void StringDictionaryFormatter_Map2Formatter_IsCorrectly()
        {
            StringDictionary dict = new StringDictionary();
            var values = RandomHelper.RandomValueWithOutStringEmpty<Dictionary<string, string>>();
            foreach (var item in values)
            {
                dict.Add(item.Key, item.Value);
            }

            VerifyHelper.VerifySpecific(dict, BssomSerializerOptions.Default.WithIDictionaryIsSerializeMap1Type(false));
            VerifyHelper.VerifySize(dict, BssomSerializerOptions.Default.WithIDictionaryIsSerializeMap1Type(false));
        }

        [Fact]
        public void StringDictionaryFormatter_NullFormatter_IsCorrectly()
        {
            VerifyHelper.VerifyTypeNull<StringDictionary>();
        }

        [Fact]
        public void BitArrayFormatter_IsCorrectly()
        {
            BitArray sd = new BitArray(RandomHelper.RandomValue<bool[]>());
            VerifyHelper.VerifySimpleType(sd);
            VerifyHelper.VerifySize(sd);
            VerifyHelper.VerifyTypeNull<BitArray>();
        }

        [Fact]
        public void NameValueCollectionFormatter_Map1Formatter_IsCorrectly()
        {
            NameValueCollection dict = new NameValueCollection();
            var values = RandomHelper.RandomValueWithOutStringEmpty<Dictionary<string, string>>();
            foreach (var item in values)
            {
                dict.Add(item.Key, item.Value);
            }

            VerifyHelper.VerifySpecific(dict, BssomSerializerOptions.Default.WithIDictionaryIsSerializeMap1Type(true));
            VerifyHelper.VerifySize(dict, BssomSerializerOptions.Default.WithIDictionaryIsSerializeMap1Type(true));
        }

        [Fact]
        public void NameValueCollectionFormatter_Map2Formatter_IsCorrectly()
        {
            NameValueCollection dict = new NameValueCollection();
            var values = RandomHelper.RandomValueWithOutStringEmpty<Dictionary<string, string>>();
            foreach (var item in values)
            {
                dict.Add(item.Key, item.Value);
            }

            VerifyHelper.VerifySpecific(dict, BssomSerializerOptions.Default.WithIDictionaryIsSerializeMap1Type(true));
            VerifyHelper.VerifySize(dict, BssomSerializerOptions.Default.WithIDictionaryIsSerializeMap1Type(true));
        }

        [Fact]
        public void NameValueCollectionFormatter_NullFormatter_IsCorrectly()
        {
            VerifyHelper.VerifyTypeNull<NameValueCollection>();
        }

        [Fact]
        public void VersionFormatter_IsCorrectly()
        {
            Version sd = new Version(3, 4);
            VerifyHelper.VerifySimpleType(sd);
            VerifyHelper.VerifyTypeNull<Version>();
        }

        [Fact]
        public void UriFormatter_IsCorrectly()
        {
            Uri sd = new Uri("http://www.bssom.com");
            VerifyHelper.VerifySimpleType(sd);
            VerifyHelper.VerifyTypeNull<Uri>();
        }

        [Fact]
        public void TimeSpanFormatter_IsCorrectly()
        {
            for (int i = 0; i < 10; i++)
            {
                var values = RandomHelper.RandomValue<TimeSpan>();
                VerifyHelper.VerifySimpleType(values);
            }
        }

        [Fact]
        public void DBNullFormatter_IsCorrectly()
        {
            VerifyHelper.VerifySimpleType(DBNull.Value);
        }

        [Fact]
        public void DataTableFormatter_IsCorrectly()
        {
            DataTable dt = RandomHelper.RandomValue<DataTable>();
            VerifyHelper.VerifySpecific(dt);
            VerifyHelper.VerifyTypeNull<DataTable>();
        }
    }
}
