using Bssom.Serializer.BssMap;
using System;
using System.Linq;
using Bssom.Serializer.Formatters;
using Bssom.Serializer.Resolver;
using Xunit;

namespace Bssom.Serializer.Test
{
    public class BuildInResolverTest_SpecialGenericType
    {
        [Fact]
        public void ResolverGetFormatter_LazyFormatter_CanBeObtainedNormally()
        {
            BuildInResolver.Instance.GetFormatter<Lazy<int>>().IsNotNull();
            BuildInResolver.Instance.GetFormatter<Lazy<string>>().IsNotNull();
            BuildInResolver.Instance.GetFormatter<Lazy<DateTime>>().IsNotNull();
        }

        [Fact]
        public void LazyFormatter_Formatter_IsCorrectly()
        {
            var lz = new Lazy<int>(() => 100);
            VerifyHelper.VerifySpecific(lz);
            VerifyHelper.VerifyTypeNull<Lazy<int>>();

            var lz2 = new Lazy<string>(() => "");
            VerifyHelper.VerifySpecific(lz2);
            VerifyHelper.VerifyTypeNull<Lazy<string>>();
        }

        [Fact]
        public void ResolverGetFormatter_IGroupFormatter_CanBeObtainedNormally()
        {
            BuildInResolver.Instance.GetFormatter<IGrouping<int, long>>().IsNotNull();
        }

        [Fact]
        public void IGroupFormatter_Formatter_IsCorrectly()
        {
            var ary = RandomHelper.RandomValue<int[]>();
            var first = ary.GroupBy(e => e).First();

            VerifyHelper.VerifySpecific(first);
            VerifyHelper.VerifyTypeNull<IGrouping<int, int>>();
        }

        [Fact]
        public void ResolverGetFormatter_ILookupFormatter_CanBeObtainedNormally()
        {
            BuildInResolver.Instance.GetFormatter<ILookup<int, int>>().IsNotNull();
        }

        [Fact]
        public void ILookupFormatter_Formatter_IsCorrectly()
        {
            var ary = RandomHelper.RandomValue<int[]>();
            var lookup = new Internal.Lookup<int, int>(ary.GroupBy(e => e));

            VerifyHelper.VerifySpecific<int>(lookup);
            VerifyHelper.VerifyTypeNull<IGrouping<int, int>>();
        }

        [Fact]
        public void ResolverGetFormatter_AnonymousTypeFormatter_CanBeObtainedNormally()
        {
            var testData = new { A = 600, CA = true, BD = new { Num = 7, TC = 17 }, EF = "oaoaoaoa" };
            var t = testData.GetType();
            BuildInResolver.Instance.GetFormatterWithVerify(t).IsType(typeof(AnonymousTypeFormatter<>).MakeGenericType(t));
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void AnonymousTypeFormatter_Formatter_IsCorrectly(bool isSerializeMap1Type)
        {
            var option = BssomSerializerOptions.Default.WithIDictionaryIsSerializeMap1Type(isSerializeMap1Type);
            var testData = new { A = 600, CA = true, BD = new { Num = 7, TC = 17 }, EF = "oaoaoaoa" };

            var buf = BssomSerializer.Serialize(testData, option);
            BssomSerializer.Size(testData, option).Is(buf.Length);
            var map = (BssomMap)BssomSerializer.Deserialize<object>(buf, option);
            map.Count.Is(4);
            map["A"].Is(600);
            map["CA"].Is(true);
            map["BD"].IsType<BssomMap>();
            var bmap = (BssomMap)map["BD"];
            bmap.Count.Is(2);
            bmap["Num"].Is(7);
            bmap["TC"].Is(17);
            map["EF"].Is("oaoaoaoa");
        }


    }
}
